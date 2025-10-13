using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.Context;
using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public class StudentGamificationRepository : BaseRepository<StudentGamificationProfile>, IStudentGamificationRepository
    {
        public StudentGamificationRepository(VTSDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<StudentGamificationProfile?> GetByStudentIdAsync(Guid studentId)
        {
            return await _context.StudentGamificationProfiles
                .Include(profile => profile.Badges)
                .FirstOrDefaultAsync(profile => profile.StudentId == studentId);
        }
    }
}
