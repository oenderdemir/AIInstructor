using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using AIInstructor.src.TrainingScenarios.DTO;
using AIInstructor.src.TrainingScenarios.Entity;
using AIInstructor.src.TrainingScenarios.Repository;

namespace AIInstructor.src.TrainingScenarios.Service
{
    public class GamificationService : IGamificationService
    {
        private readonly IStudentGamificationRepository gamificationRepository;
        private readonly IEarnedBadgeRepository earnedBadgeRepository;
        private readonly IMapper mapper;
        private readonly ILogger<GamificationService> logger;

        private static readonly (string Key, string Name, string Description, Func<ScenarioEvaluation, TrainingScenario, bool> Predicate)[] badgeRules =
        {
            ("perfect_check_in", "Mükemmel Check-in", "Tüm değerlendirme kriterlerinde 90+ puan alan öğrenci.", (evaluation, _) =>
                evaluation.CommunicationScore >= 90 &&
                evaluation.ProblemSolvingScore >= 90 &&
                evaluation.LanguageScore >= 90 &&
                evaluation.ProfessionalismScore >= 90),
            ("resilient_solver", "Zor Müşteri Ustası", "Zorlu senaryolarda müşteri sorunlarını başarıyla çözen öğrenci.", (evaluation, scenario) =>
                scenario.InteractionRounds >= 5 && evaluation.ProblemSolvingScore >= 85),
            ("language_virtuoso", "Dil Virtüözü", "Dil kullanımında 95+ puan alan öğrenci.", (evaluation, _) =>
                evaluation.LanguageScore >= 95)
        };

        public GamificationService(
            IStudentGamificationRepository gamificationRepository,
            IEarnedBadgeRepository earnedBadgeRepository,
            IMapper mapper,
            ILogger<GamificationService> logger)
        {
            this.gamificationRepository = gamificationRepository;
            this.earnedBadgeRepository = earnedBadgeRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<GamificationProfileDto> ApplyScenarioResultAsync(Guid studentId, ScenarioEvaluation evaluation, TrainingScenario scenario)
        {
            var profile = await gamificationRepository.GetByStudentIdAsync(studentId);
            if (profile == null)
            {
                profile = new StudentGamificationProfile
                {
                    StudentId = studentId,
                    TotalPoints = 0,
                    CurrentLevel = 1,
                    ExperiencePoints = 0,
                    TotalScenariosCompleted = 0
                };

                await gamificationRepository.AddAsync(profile);
            }

            profile.TotalScenariosCompleted += 1;
            profile.ExperiencePoints += evaluation.PointsAwarded;
            profile.TotalPoints += evaluation.PointsAwarded;
            profile.LastUpdatedAt = DateTime.UtcNow;
            profile.CurrentLevel = CalculateLevel(profile.TotalPoints);

            gamificationRepository.Update(profile);

            var earnedBadges = new List<EarnedBadge>();
            foreach (var (key, name, description, predicate) in badgeRules)
            {
                if (!predicate(evaluation, scenario))
                {
                    continue;
                }

                if (await earnedBadgeRepository.HasBadgeAsync(profile.Id, key))
                {
                    continue;
                }

                var badge = new EarnedBadge
                {
                    GamificationProfileId = profile.Id,
                    BadgeKey = key,
                    BadgeName = name,
                    Description = description,
                    EarnedAt = DateTime.UtcNow
                };

                earnedBadges.Add(badge);
                await earnedBadgeRepository.AddAsync(badge);
            }

            if (earnedBadges.Count > 0)
            {
                logger.LogInformation("Öğrenci {StudentId} için {BadgeCount} yeni rozet verildi.", studentId, earnedBadges.Count);
            }

            await gamificationRepository.SaveChangesAsync();

            profile = await gamificationRepository.GetByStudentIdAsync(studentId) ?? profile;

            return mapper.Map<GamificationProfileDto>(profile);
        }

        private static int CalculateLevel(int totalPoints)
        {
            if (totalPoints >= 3000)
            {
                return 5;
            }

            if (totalPoints >= 2000)
            {
                return 4;
            }

            if (totalPoints >= 1200)
            {
                return 3;
            }

            if (totalPoints >= 600)
            {
                return 2;
            }

            return 1;
        }
    }
}
