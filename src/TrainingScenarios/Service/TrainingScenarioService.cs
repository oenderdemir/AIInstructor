using AutoMapper;
using Microsoft.Extensions.Logging;
using AIInstructor.src.TrainingScenarios.DTO;
using AIInstructor.src.TrainingScenarios.Entity;
using AIInstructor.src.TrainingScenarios.Repository;

namespace AIInstructor.src.TrainingScenarios.Service
{
    public class TrainingScenarioService : ITrainingScenarioService
    {
        private readonly ITrainingScenarioRepository trainingScenarioRepository;
        private readonly IMapper mapper;
        private readonly ILogger<TrainingScenarioService> logger;

        public TrainingScenarioService(
            ITrainingScenarioRepository trainingScenarioRepository,
            IMapper mapper,
            ILogger<TrainingScenarioService> logger)
        {
            this.trainingScenarioRepository = trainingScenarioRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<TrainingScenarioDetailDto> CreateAsync(CreateTrainingScenarioRequest request)
        {
            var entity = mapper.Map<TrainingScenario>(request);
            await trainingScenarioRepository.AddAsync(entity);
            await trainingScenarioRepository.SaveChangesAsync();

            logger.LogInformation("Yeni eğitim senaryosu oluşturuldu: {ScenarioTitle}", entity.Title);

            return mapper.Map<TrainingScenarioDetailDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await trainingScenarioRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Senaryo bulunamadı: {id}");
            }

            trainingScenarioRepository.Delete(entity);
            await trainingScenarioRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TrainingScenarioSummaryDto>> GetAllAsync()
        {
            var scenarios = await trainingScenarioRepository.GetAllAsync();
            return mapper.Map<IEnumerable<TrainingScenarioSummaryDto>>(scenarios);
        }

        public async Task<TrainingScenarioDetailDto> GetByIdAsync(Guid id)
        {
            var entity = await trainingScenarioRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Senaryo bulunamadı: {id}");
            }

            return mapper.Map<TrainingScenarioDetailDto>(entity);
        }

        public async Task<TrainingScenarioDetailDto> UpdateAsync(Guid id, UpdateTrainingScenarioRequest request)
        {
            var entity = await trainingScenarioRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Senaryo bulunamadı: {id}");
            }

            mapper.Map(request, entity);
            trainingScenarioRepository.Update(entity);
            await trainingScenarioRepository.SaveChangesAsync();

            logger.LogInformation("Eğitim senaryosu güncellendi: {ScenarioId}", id);

            return mapper.Map<TrainingScenarioDetailDto>(entity);
        }
    }
}
