using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public interface IStudentGamificationRepository : IBaseRepository<StudentGamificationProfile>
    {
        Task<StudentGamificationProfile?> GetByStudentIdAsync(Guid studentId);
    }
}
