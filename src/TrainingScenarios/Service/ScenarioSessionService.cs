using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AIInstructor.src.TrainingScenarios.DTO;
using AIInstructor.src.TrainingScenarios.Entity;
using AIInstructor.src.TrainingScenarios.Repository;

namespace AIInstructor.src.TrainingScenarios.Service
{
    public class ScenarioSessionService : IScenarioSessionService
    {
        private readonly IScenarioSessionRepository sessionRepository;
        private readonly IScenarioMessageRepository messageRepository;
        private readonly ITrainingScenarioRepository trainingScenarioRepository;
        private readonly IScenarioEvaluationRepository evaluationRepository;
        private readonly IGamificationService gamificationService;
        private readonly IOpenAIChatClient openAIChatClient;
        private readonly IMapper mapper;
        private readonly ILogger<ScenarioSessionService> logger;
        private readonly IOptions<OpenAIOptions> openAIOptions;

        public ScenarioSessionService(
            IScenarioSessionRepository sessionRepository,
            IScenarioMessageRepository messageRepository,
            ITrainingScenarioRepository trainingScenarioRepository,
            IScenarioEvaluationRepository evaluationRepository,
            IGamificationService gamificationService,
            IOpenAIChatClient openAIChatClient,
            IMapper mapper,
            ILogger<ScenarioSessionService> logger,
            IOptions<OpenAIOptions> openAIOptions)
        {
            this.sessionRepository = sessionRepository;
            this.messageRepository = messageRepository;
            this.trainingScenarioRepository = trainingScenarioRepository;
            this.evaluationRepository = evaluationRepository;
            this.gamificationService = gamificationService;
            this.openAIChatClient = openAIChatClient;
            this.mapper = mapper;
            this.logger = logger;
            this.openAIOptions = openAIOptions;
        }

        public async Task<ScenarioSessionDetailDto> GetSessionAsync(Guid sessionId)
        {
            var session = await sessionRepository.GetSessionWithDetailsAsync(sessionId);
            if (session == null)
            {
                throw new KeyNotFoundException($"Senaryo oturumu bulunamadı: {sessionId}");
            }

            return mapper.Map<ScenarioSessionDetailDto>(session);
        }

        public async Task<ScenarioTurnResponseDto> StartSessionAsync(Guid scenarioId, StartScenarioSessionRequest request, CancellationToken cancellationToken = default)
        {
            var scenario = await trainingScenarioRepository.GetByIdAsync(scenarioId);
            if (scenario == null)
            {
                throw new KeyNotFoundException($"Senaryo bulunamadı: {scenarioId}");
            }

            var language = string.IsNullOrWhiteSpace(request.Language) ? scenario.Language : request.Language!;
            var systemPrompt = BuildSystemPrompt(scenario, language, request.CustomContext);

            var session = new ScenarioSession
            {
                ScenarioId = scenario.Id,
                StudentId = request.StudentId,
                Language = language,
                MaxTurns = scenario.InteractionRounds,
                CurrentTurn = 0,
                IsCompleted = false,
                SystemPrompt = systemPrompt
            };

            await sessionRepository.AddAsync(session);
            await sessionRepository.SaveChangesAsync();

            var tutorTurn = await GenerateTutorTurnAsync(session, scenario, cancellationToken);
            var tutorMessage = CreateTutorMessageEntity(session, tutorTurn, 1);
            await messageRepository.AddAsync(tutorMessage);
            session.Messages.Add(tutorMessage);
            await messageRepository.SaveChangesAsync();

            var response = new ScenarioTurnResponseDto
            {
                SessionId = session.Id,
                ScenarioId = scenario.Id,
                CurrentTurn = session.CurrentTurn,
                MaxTurns = session.MaxTurns,
                TutorMessage = mapper.Map<ScenarioMessageDto>(tutorMessage),
                TutorTurn = ToTutorTurnDto(tutorTurn),
                Messages = mapper.Map<IReadOnlyCollection<ScenarioMessageDto>>(session.Messages.OrderBy(m => m.Sequence).ToList()),
                IsCompleted = session.IsCompleted
            };

            return response;
        }

        public async Task<ScenarioTurnResponseDto> SubmitTurnAsync(Guid sessionId, SubmitScenarioTurnRequest request, CancellationToken cancellationToken = default)
        {
            var session = await sessionRepository.GetSessionWithDetailsAsync(sessionId);
            if (session == null)
            {
                throw new KeyNotFoundException($"Senaryo oturumu bulunamadı: {sessionId}");
            }

            if (session.StudentId != request.StudentId)
            {
                throw new InvalidOperationException("Bu oturum başka bir öğrenciye aittir.");
            }

            if (session.IsCompleted)
            {
                throw new InvalidOperationException("Bu oturum zaten tamamlanmıştır.");
            }

            var scenario = session.Scenario ?? await trainingScenarioRepository.GetByIdAsync(session.ScenarioId)
                ?? throw new KeyNotFoundException($"Senaryo bulunamadı: {session.ScenarioId}");

            var nextSequence = session.Messages.Any() ? session.Messages.Max(m => m.Sequence) + 1 : 1;
            var studentMessage = new ScenarioMessage
            {
                SessionId = session.Id,
                Sender = ScenarioMessageSender.Student,
                Content = request.StudentMessage,
                Sequence = nextSequence,
                SentAt = DateTime.UtcNow
            };

            await messageRepository.AddAsync(studentMessage);
            session.Messages.Add(studentMessage);

            session.CurrentTurn += 1;
            sessionRepository.Update(session);
            await messageRepository.SaveChangesAsync();

            var tutorTurn = await GenerateTutorTurnAsync(session, scenario, cancellationToken);

            var tutorMessage = CreateTutorMessageEntity(session, tutorTurn, studentMessage.Sequence + 1);
            await messageRepository.AddAsync(tutorMessage);
            session.Messages.Add(tutorMessage);

            var shouldComplete = request.ForceComplete || session.CurrentTurn >= session.MaxTurns;

            ScenarioEvaluation? evaluation = null;
            GamificationProfileDto? gamification = null;

            if (shouldComplete)
            {
                session.IsCompleted = true;
                session.CompletedAt = DateTime.UtcNow;
                sessionRepository.Update(session);

                evaluation = await EvaluateScenarioAsync(session, scenario, cancellationToken);
                await evaluationRepository.AddAsync(evaluation);

                await messageRepository.SaveChangesAsync();

                gamification = await gamificationService.ApplyScenarioResultAsync(session.StudentId, evaluation, scenario);
            }
            else
            {
                await messageRepository.SaveChangesAsync();
            }

            var orderedMessages = session.Messages.OrderBy(m => m.Sequence).ToList();

            var response = new ScenarioTurnResponseDto
            {
                SessionId = session.Id,
                ScenarioId = session.ScenarioId,
                CurrentTurn = session.CurrentTurn,
                MaxTurns = session.MaxTurns,
                TutorMessage = mapper.Map<ScenarioMessageDto>(tutorMessage),
                TutorTurn = ToTutorTurnDto(tutorTurn),
                Messages = mapper.Map<IReadOnlyCollection<ScenarioMessageDto>>(orderedMessages),
                IsCompleted = session.IsCompleted,
                Evaluation = evaluation != null ? mapper.Map<ScenarioEvaluationDto>(evaluation) : null,
                Gamification = gamification
            };

            return response;
        }

        private string BuildSystemPrompt(TrainingScenario scenario, string language, IDictionary<string, string>? customContext)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Sen bir turizm eğitmeni ve müşteri simülasyonu yöneticisisin.");
            builder.AppendLine("Öğrenci, otelcilik bağlamında müşteri ile iletişim kurmayı öğreniyor.");
            builder.AppendLine("Yanıtların " + language + " dilinde olmalıdır.");
            builder.AppendLine();
            builder.AppendLine("Senaryonun detayları:");
            builder.AppendLine($"- Başlık: {scenario.Title}");
            builder.AppendLine($"- Açıklama: {scenario.Description}");
            if (!string.IsNullOrWhiteSpace(scenario.CustomerProfile))
            {
                builder.AppendLine($"- Müşteri Profili: {scenario.CustomerProfile}");
            }

            if (!string.IsNullOrWhiteSpace(scenario.LearningObjectives))
            {
                builder.AppendLine($"- Öğrenme Hedefleri: {scenario.LearningObjectives}");
            }

            if (!string.IsNullOrWhiteSpace(scenario.SuccessCriteria))
            {
                builder.AppendLine($"- Başarı Kriterleri: {scenario.SuccessCriteria}");
            }

            builder.AppendLine();
            builder.AppendLine("Kurallar:");
            builder.AppendLine("1. Her yanıtta önce müşteri rolündeki diyalog cümlesini üret.");
            builder.AppendLine("2. Ardından öğrenciye yönelik kısa bir yönlendirme ve gerektiğinde ipuçları ver.");
            builder.AppendLine("3. Müşteri diyalogları doğal ve gerçekçi olmalı, her turda yeni bir bilgi eklenmeli.");
            builder.AppendLine("4. Öğrencinin performansını gözlemle ve gerektiğinde düzeltici geri bildirim sağla.");
            builder.AppendLine("5. Yanıtlarını JSON formatında üret ki uygulama detayları ayrıştırabilsin.");

            if (customContext != null && customContext.Any())
            {
                builder.AppendLine();
                builder.AppendLine("Ek bağlam:");
                foreach (var pair in customContext)
                {
                    builder.AppendLine($"- {pair.Key}: {pair.Value}");
                }
            }

            builder.AppendLine();
            builder.AppendLine("JSON yapısı şu alanları içermelidir: customer_dialogue, tutor_guidance, coaching_tips, vocabulary_suggestions, next_expected_action.");

            return builder.ToString();
        }

        private object BuildTutorResponseFormat()
        {
            return new
            {
                type = "json_schema",
                json_schema = new
                {
                    name = "scenario_turn",
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,
                        properties = new Dictionary<string, object>
                        {
                            ["customer_dialogue"] = new Dictionary<string, object>
                            {
                                ["type"] = "string",
                                ["description"] = "Müşteri rolündeki diyalog cümlesi."
                            },
                            ["tutor_guidance"] = new Dictionary<string, object>
                            {
                                ["type"] = "string",
                                ["description"] = "Öğrenciye verilen yönlendirme ve geri bildirim."
                            },
                            ["coaching_tips"] = new Dictionary<string, object>
                            {
                                ["type"] = "string",
                                ["description"] = "Öğrenciye verilecek kısa ipuçları.",
                                ["nullable"] = true
                            },
                            ["vocabulary_suggestions"] = new Dictionary<string, object>
                            {
                                ["type"] = "array",
                                ["items"] = new Dictionary<string, object>
                                {
                                    ["type"] = "string"
                                }
                            },
                            ["next_expected_action"] = new Dictionary<string, object>
                            {
                                ["type"] = "string",
                                ["description"] = "Öğrenciden beklenen bir sonraki adım.",
                                ["nullable"] = true
                            }
                        },
                        required = new[] { "customer_dialogue", "tutor_guidance" }
                    }
                }
            };
        }

        private async Task<TutorTurnInternal> GenerateTutorTurnAsync(ScenarioSession session, TrainingScenario scenario, CancellationToken cancellationToken)
        {
            var messages = new List<OpenAIChatMessage>
            {
                new("system", session.SystemPrompt ?? BuildSystemPrompt(scenario, session.Language, null))
            };

            var orderedMessages = session.Messages.OrderBy(m => m.Sequence).ToList();
            foreach (var message in orderedMessages)
            {
                messages.Add(ConvertToChatMessage(message));
            }

            if (!orderedMessages.Any())
            {
                messages.Add(new OpenAIChatMessage("user", "Senaryoyu başlat ve ilk müşteri diyalogunu üret."));
            }

            var request = new OpenAIChatCompletionRequest
            {
                Messages = messages,
                ResponseFormat = BuildTutorResponseFormat(),
                Temperature = openAIOptions.Value.Temperature,
                Model = openAIOptions.Value.DefaultModel
            };

            var result = await openAIChatClient.GetChatCompletionAsync(request, cancellationToken);

            return ParseTutorTurn(result.Content);
        }

        private ScenarioMessage CreateTutorMessageEntity(ScenarioSession session, TutorTurnInternal turn, int sequence)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.AppendLine($"Müşteri: {turn.CustomerDialogue}");
            contentBuilder.AppendLine($"Eğitmen: {turn.TutorGuidance}");

            if (!string.IsNullOrWhiteSpace(turn.CoachingTips))
            {
                contentBuilder.AppendLine($"İpucu: {turn.CoachingTips}");
            }

            if (turn.VocabularySuggestions.Length > 0)
            {
                contentBuilder.AppendLine("Kelime Önerileri: " + string.Join(", ", turn.VocabularySuggestions));
            }

            if (!string.IsNullOrWhiteSpace(turn.NextExpectedAction))
            {
                contentBuilder.AppendLine($"Beklenen aksiyon: {turn.NextExpectedAction}");
            }

            var message = new ScenarioMessage
            {
                SessionId = session.Id,
                Sender = ScenarioMessageSender.Tutor,
                Content = contentBuilder.ToString().Trim(),
                Sequence = sequence,
                SentAt = DateTime.UtcNow
            };

            return message;
        }

        private OpenAIChatMessage ConvertToChatMessage(ScenarioMessage message)
        {
            var role = message.Sender switch
            {
                ScenarioMessageSender.Student => "user",
                ScenarioMessageSender.Tutor => "assistant",
                ScenarioMessageSender.Customer => "assistant",
                _ => "assistant"
            };

            return new OpenAIChatMessage(role, message.Content);
        }

        private TutorTurnInternal ParseTutorTurn(string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    throw new JsonException("Boş içerik döndü.");
                }

                var node = JsonNode.Parse(content);
                if (node == null)
                {
                    throw new JsonException("Geçersiz JSON içeriği.");
                }

                return new TutorTurnInternal
                {
                    CustomerDialogue = node["customer_dialogue"]?.GetValue<string>() ?? content,
                    TutorGuidance = node["tutor_guidance"]?.GetValue<string>() ?? string.Empty,
                    CoachingTips = node["coaching_tips"]?.GetValue<string>(),
                    VocabularySuggestions = node["vocabulary_suggestions"]?.AsArray().Select(x => x?.GetValue<string>() ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray() ?? Array.Empty<string>(),
                    NextExpectedAction = node["next_expected_action"]?.GetValue<string>()
                };
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Tutor yanıtı JSON olarak ayrıştırılamadı. Metin içerik kullanılacak.");
                return new TutorTurnInternal
                {
                    CustomerDialogue = content,
                    TutorGuidance = content,
                    CoachingTips = null,
                    VocabularySuggestions = Array.Empty<string>(),
                    NextExpectedAction = null
                };
            }
        }

        private async Task<ScenarioEvaluation> EvaluateScenarioAsync(ScenarioSession session, TrainingScenario scenario, CancellationToken cancellationToken)
        {
            var transcript = BuildTranscript(session);

            var messages = new List<OpenAIChatMessage>
            {
                new("system", "Sen bir turizm eğitim değerlendirme motorusun. Öğrencinin performansını verilen kriterlere göre değerlendireceksin. Yalnızca geçerli JSON çıktısı üret."),
                new("user", $"Senaryo başlığı: {scenario.Title}\nÖğrencinin dili: {session.Language}\nTranskript:\n{transcript}\nKriterler: İletişim, Problem Çözme, Dil Kullanımı, Profesyonellik. Her biri için 0-100 arası puan ve geri bildirim ver.")
            };

            var request = new OpenAIChatCompletionRequest
            {
                Messages = messages,
                Model = openAIOptions.Value.EvaluationModel ?? openAIOptions.Value.DefaultModel,
                Temperature = openAIOptions.Value.EvaluationTemperature,
                ResponseFormat = BuildEvaluationResponseFormat()
            };

            OpenAIChatCompletionResult result;
            try
            {
                result = await openAIChatClient.GetChatCompletionAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Senaryo değerlendirmesi alınırken hata oluştu. Yerel değerlendirme kullanılacak.");
                return BuildFallbackEvaluation(session);
            }

            return ParseEvaluation(session, result.Content);
        }

        private object BuildEvaluationResponseFormat()
        {
            return new
            {
                type = "json_schema",
                json_schema = new
                {
                    name = "scenario_evaluation",
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,
                        properties = new Dictionary<string, object>
                        {
                            ["communication"] = BuildEvaluationCategorySchema("communication"),
                            ["problem_solving"] = BuildEvaluationCategorySchema("problem_solving"),
                            ["language"] = BuildEvaluationCategorySchema("language"),
                            ["professionalism"] = BuildEvaluationCategorySchema("professionalism"),
                            ["overall_feedback"] = new Dictionary<string, object> { ["type"] = "string" },
                            ["strengths"] = new Dictionary<string, object>
                            {
                                ["type"] = "array",
                                ["items"] = new Dictionary<string, object> { ["type"] = "string" }
                            },
                            ["areas_for_improvement"] = new Dictionary<string, object>
                            {
                                ["type"] = "array",
                                ["items"] = new Dictionary<string, object> { ["type"] = "string" }
                            },
                            ["suggested_next_steps"] = new Dictionary<string, object>
                            {
                                ["type"] = "array",
                                ["items"] = new Dictionary<string, object> { ["type"] = "string" }
                            }
                        },
                        required = new[] { "communication", "problem_solving", "language", "professionalism" }
                    }
                }
            };
        }

        private Dictionary<string, object> BuildEvaluationCategorySchema(string name)
        {
            return new Dictionary<string, object>
            {
                ["type"] = "object",
                ["additionalProperties"] = false,
                ["properties"] = new Dictionary<string, object>
                {
                    ["score"] = new Dictionary<string, object>
                    {
                        ["type"] = "integer",
                        ["minimum"] = 0,
                        ["maximum"] = 100
                    },
                    ["feedback"] = new Dictionary<string, object>
                    {
                        ["type"] = "string"
                    }
                },
                ["required"] = new[] { "score", "feedback" }
            };
        }

        private ScenarioEvaluation ParseEvaluation(ScenarioSession session, string content)
        {
            try
            {
                var node = JsonNode.Parse(content);
                if (node == null)
                {
                    throw new JsonException("Değerlendirme JSON verisi boş.");
                }

                var communicationScore = GetScore(node, "communication");
                var problemSolvingScore = GetScore(node, "problem_solving");
                var languageScore = GetScore(node, "language");
                var professionalismScore = GetScore(node, "professionalism");
                var totalScore = (communicationScore + problemSolvingScore + languageScore + professionalismScore) / 4;

                return new ScenarioEvaluation
                {
                    SessionId = session.Id,
                    CommunicationScore = communicationScore,
                    CommunicationFeedback = GetFeedback(node, "communication"),
                    ProblemSolvingScore = problemSolvingScore,
                    ProblemSolvingFeedback = GetFeedback(node, "problem_solving"),
                    LanguageScore = languageScore,
                    LanguageFeedback = GetFeedback(node, "language"),
                    ProfessionalismScore = professionalismScore,
                    ProfessionalismFeedback = GetFeedback(node, "professionalism"),
                    TotalScore = totalScore,
                    OverallFeedback = node["overall_feedback"]?.GetValue<string>(),
                    Strengths = string.Join("\n", node["strengths"]?.AsArray().Select(x => x?.GetValue<string>() ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)) ?? Array.Empty<string>()),
                    AreasForImprovement = string.Join("\n", node["areas_for_improvement"]?.AsArray().Select(x => x?.GetValue<string>() ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)) ?? Array.Empty<string>()),
                    SuggestedNextSteps = string.Join("\n", node["suggested_next_steps"]?.AsArray().Select(x => x?.GetValue<string>() ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)) ?? Array.Empty<string>()),
                    RawEvaluationJson = content,
                    PointsAwarded = totalScore
                };
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Değerlendirme JSON içeriği ayrıştırılamadı. Yedek değerlendirme oluşturuluyor.");
                return BuildFallbackEvaluation(session);
            }
        }

        private ScenarioEvaluation BuildFallbackEvaluation(ScenarioSession session)
        {
            var messageCount = session.Messages.Count(m => m.Sender == ScenarioMessageSender.Student);
            var score = Math.Min(100, 50 + (messageCount * 10));

            return new ScenarioEvaluation
            {
                SessionId = session.Id,
                CommunicationScore = score,
                ProblemSolvingScore = score,
                LanguageScore = score,
                ProfessionalismScore = score,
                TotalScore = score,
                OverallFeedback = "Otomatik değerlendirme: Sistem gerçek zamanlı değerlendirme sağlayamadı. Varsayılan puanlama uygulandı.",
                Strengths = "Simülasyonu tamamlamak için çaba gösterdi.",
                AreasForImprovement = "Geribildirim almak için eğitmenle tekrar çalış.",
                SuggestedNextSteps = "Gerçek senaryolarla pratik yapmaya devam et.",
                RawEvaluationJson = string.Empty,
                PointsAwarded = score
            };
        }

        private int GetScore(JsonNode node, string category)
        {
            return node[category]?["score"]?.GetValue<int>() ?? 0;
        }

        private string? GetFeedback(JsonNode node, string category)
        {
            return node[category]?["feedback"]?.GetValue<string>();
        }

        private string BuildTranscript(ScenarioSession session)
        {
            var builder = new StringBuilder();
            foreach (var message in session.Messages.OrderBy(m => m.Sequence))
            {
                var speaker = message.Sender switch
                {
                    ScenarioMessageSender.Student => "Öğrenci",
                    ScenarioMessageSender.Tutor => "AI Eğitmen",
                    ScenarioMessageSender.Customer => "AI Müşteri",
                    _ => "Bilinmeyen"
                };

                builder.AppendLine($"[{speaker}] {message.Content}");
            }

            return builder.ToString();
        }

        private static TutorTurnDto ToTutorTurnDto(TutorTurnInternal turn)
        {
            return new TutorTurnDto
            {
                CustomerDialogue = turn.CustomerDialogue,
                TutorGuidance = turn.TutorGuidance,
                CoachingTips = turn.CoachingTips,
                VocabularySuggestions = turn.VocabularySuggestions,
                NextExpectedAction = turn.NextExpectedAction
            };
        }

        private class TutorTurnInternal
        {
            public string CustomerDialogue { get; set; } = string.Empty;
            public string TutorGuidance { get; set; } = string.Empty;
            public string? CoachingTips { get; set; }
            public string[] VocabularySuggestions { get; set; } = Array.Empty<string>();
            public string? NextExpectedAction { get; set; }
        }
    }
}
