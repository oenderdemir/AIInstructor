using System.Linq;
using AutoMapper;
using AIInstructor.src.TrainingScenarios.DTO;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Mapper
{
    public class TrainingScenarioProfile : Profile
    {
        public TrainingScenarioProfile()
        {
            CreateMap<TrainingScenario, TrainingScenarioSummaryDto>();
            CreateMap<TrainingScenario, TrainingScenarioDetailDto>();
            CreateMap<CreateTrainingScenarioRequest, TrainingScenario>();
            CreateMap<UpdateTrainingScenarioRequest, TrainingScenario>();

            CreateMap<ScenarioMessage, ScenarioMessageDto>();
            CreateMap<ScenarioEvaluation, ScenarioEvaluationDto>();
            CreateMap<ScenarioSession, ScenarioSessionDetailDto>()
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages.OrderBy(m => m.Sequence)))
                .ForMember(dest => dest.Evaluation, opt => opt.MapFrom(src => src.Evaluation));

            CreateMap<EarnedBadge, EarnedBadgeDto>();
            CreateMap<StudentGamificationProfile, GamificationProfileDto>()
                .ForMember(dest => dest.Badges, opt => opt.MapFrom(src => src.Badges.OrderByDescending(b => b.EarnedAt)));
        }
    }
}
