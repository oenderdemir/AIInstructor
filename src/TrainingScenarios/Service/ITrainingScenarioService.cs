using AIInstructor.src.TrainingScenarios.DTO;

namespace AIInstructor.src.TrainingScenarios.Service
{
    public interface ITrainingScenarioService
    {
        Task<IEnumerable<TrainingScenarioSummaryDto>> GetAllAsync();
        Task<TrainingScenarioDetailDto> GetByIdAsync(Guid id);
        Task<TrainingScenarioDetailDto> CreateAsync(CreateTrainingScenarioRequest request);
        Task<TrainingScenarioDetailDto> UpdateAsync(Guid id, UpdateTrainingScenarioRequest request);
        Task DeleteAsync(Guid id);
    }
}
