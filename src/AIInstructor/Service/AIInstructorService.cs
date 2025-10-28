using System;
using System.Collections.Generic;
using System.Linq;
using AIInstructor.src.AIInstructor.Entity;
using AIInstructor.src.Gamification.Service;
using AIInstructor.src.OgrenciSenaryo.Repository;
using AIInstructor.src.SenaryoAdim.Repository;
using AIInstructor.src.Senaryolar.Repository;

namespace AIInstructor.src.AIInstructor.Service
{
    public interface IAIInstructorService
    {
        Task<AIInstructorSession> EvaluateAsync(Guid ogrenciSenaryoId, IEnumerable<string> messages);
    }

    public class AIInstructorService : IAIInstructorService
    {
        private readonly IOgrenciSenaryoRepository ogrenciSenaryoRepository;
        private readonly ISenaryoRepository senaryoRepository;
        private readonly ISenaryoAdimRepository senaryoAdimRepository;
        private readonly IGamificationResultService gamificationService;

        public AIInstructorService(
            IOgrenciSenaryoRepository ogrenciSenaryoRepository,
            ISenaryoRepository senaryoRepository,
            ISenaryoAdimRepository senaryoAdimRepository,
            IGamificationResultService gamificationService)
        {
            this.ogrenciSenaryoRepository = ogrenciSenaryoRepository;
            this.senaryoRepository = senaryoRepository;
            this.senaryoAdimRepository = senaryoAdimRepository;
            this.gamificationService = gamificationService;
        }

        public async Task<AIInstructorSession> EvaluateAsync(Guid ogrenciSenaryoId, IEnumerable<string> messages)
        {
            var ogrenciSenaryo = await ogrenciSenaryoRepository.GetByIdAsync(ogrenciSenaryoId);
            if (ogrenciSenaryo == null)
            {
                throw new ArgumentException("Öğrenci senaryo kaydı bulunamadı", nameof(ogrenciSenaryoId));
            }

            var adimlar = await senaryoAdimRepository.GetBySenaryoIdAsync(ogrenciSenaryo.SenaryoId);

            var session = new AIInstructorSession
            {
                Id = Guid.NewGuid(),
                OgrenciSenaryoId = ogrenciSenaryoId,
                SenaryoId = ogrenciSenaryo.SenaryoId,
                OgrenciId = ogrenciSenaryo.OgrenciId,
                Durum = "Evaluated"
            };

            var basariliKriterler = new List<string>();
            int index = 0;
            foreach (var message in messages)
            {
                var adim = adimlar.ElementAtOrDefault(index);
                var success = adim != null && message.Contains(adim.SuccessCriteria, StringComparison.OrdinalIgnoreCase);
                if (success && adim != null)
                {
                    basariliKriterler.Add(adim.SuccessCriteria);
                }

                session.GeriBildirimler.Add(new AIInstructorStepFeedback
                {
                    Id = Guid.NewGuid(),
                    AIInstructorSessionId = session.Id,
                    Mesaj = success ? "Başarılı adım" : "Geliştirilebilir adım",
                    Success = success
                });

                index++;
            }

            var gamificationResult = await gamificationService.CalculateAsync(ogrenciSenaryoId, basariliKriterler);
            gamificationResult.Detay = "AI Instructor değerlendirmesi";
            await gamificationService.SaveResultAsync(gamificationResult);

            return session;
        }
    }
}
