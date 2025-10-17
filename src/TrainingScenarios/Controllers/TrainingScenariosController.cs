using AIInstructor.src.Shared.Controller;
using AIInstructor.src.TrainingScenarios.DTO;
using AIInstructor.src.TrainingScenarios.Entity;
using AIInstructor.src.TrainingScenarios.Repository;
using AIInstructor.src.TrainingScenarios.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIInstructor.src.TrainingScenarios.Controllers;


public sealed class TrainingScenariosController : UIController
{
    private readonly IScenarioRepository _scenarioRepository;
    private readonly IScenarioSessionService _sessionService;
    private readonly IGamificationService _gamificationService;

    public TrainingScenariosController(
        IScenarioRepository scenarioRepository,
        IScenarioSessionService sessionService,
        IGamificationService gamificationService)
    {
        _scenarioRepository = scenarioRepository;
        _sessionService = sessionService;
        _gamificationService = gamificationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ScenarioSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScenarios(CancellationToken cancellationToken)
    {
        var scenarios = await _scenarioRepository.GetAllAsync(cancellationToken);
        var summaries = scenarios.Select(s => new ScenarioSummaryDto(s.Id, s.Title, s.Description, s.Difficulty, s.Language, s.Tags.ToList())).ToList();
        return Ok(summaries);
    }

    [HttpPost("{scenarioId}/sessions")]
    [ProducesResponseType(typeof(StartScenarioResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartScenario(string scenarioId, [FromBody] StartScenarioRequest request, CancellationToken cancellationToken)
    {
        var response = await _sessionService.StartScenarioAsync(scenarioId, request.StudentId, cancellationToken);
        return Ok(response);
    }

    [HttpPost("sessions/{sessionId:guid}/student-message")]
    [ProducesResponseType(typeof(ScenarioTurnResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitStudentMessage(Guid sessionId, [FromBody] StudentMessageRequest request, CancellationToken cancellationToken)
    {
        var response = await _sessionService.SubmitStudentMessageAsync(sessionId, request.StudentId, request.Message, cancellationToken);
        return Ok(response);
    }

    [HttpPost("sessions/{sessionId:guid}/complete")]
    [ProducesResponseType(typeof(ScenarioTurnResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CompleteScenario(Guid sessionId, [FromBody] CompleteScenarioRequest request, CancellationToken cancellationToken)
    {
        var response = await _sessionService.CompleteScenarioAsync(sessionId, request.StudentId, cancellationToken);
        return Ok(response);
    }

    [HttpGet("sessions/{sessionId:guid}/transcript")]
    [ProducesResponseType(typeof(ScenarioTranscriptResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTranscript(Guid sessionId, CancellationToken cancellationToken)
    {
        var transcript = await _sessionService.GetTranscriptAsync(sessionId, cancellationToken);
        if (transcript is null)
        {
            return NotFound();
        }

        return Ok(transcript);
    }

    [HttpGet("students/{studentId}/profile")]
    [ProducesResponseType(typeof(GamificationProfile), StatusCodes.Status200OK)]
    public IActionResult GetProfile(string studentId)
    {
        var profile = _gamificationService.GetProfile(studentId);
        return Ok(profile);
    }
}
