using System.Collections.Concurrent;
using System.Text.Json;
using AIInstructor.src.TrainingScenarios.Entity;
using Microsoft.Extensions.Options;

namespace AIInstructor.src.TrainingScenarios.Repository;

public sealed class FileSystemScenarioRepository : IScenarioRepository
{
    private readonly ScenarioDataOptions _options;
    private readonly ConcurrentDictionary<string, ScenarioDefinition> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Lazy<Task<IReadOnlyCollection<ScenarioDefinition>>> _lazyInitializer;

    public FileSystemScenarioRepository(IOptions<ScenarioDataOptions> options, IWebHostEnvironment environment)
    {
        _options = options.Value;
        var root = environment.ContentRootPath;
        _lazyInitializer = new Lazy<Task<IReadOnlyCollection<ScenarioDefinition>>>(() => LoadAsync(root), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public async Task<IReadOnlyCollection<ScenarioDefinition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var scenarios = await _lazyInitializer.Value.ConfigureAwait(false);
        return scenarios;
    }

    public async Task<ScenarioDefinition?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        if (_cache.TryGetValue(id, out var cached))
        {
            return cached;
        }

        var scenarios = await _lazyInitializer.Value.ConfigureAwait(false);
        var scenario = scenarios.FirstOrDefault(s => string.Equals(s.Id, id, StringComparison.OrdinalIgnoreCase));
        if (scenario is not null)
        {
            _cache[id] = scenario;
        }

        return scenario;
    }

    private async Task<IReadOnlyCollection<ScenarioDefinition>> LoadAsync(string contentRoot)
    {
        if (string.IsNullOrWhiteSpace(_options.Directory))
        {
            return Array.Empty<ScenarioDefinition>();
        }

        var directory = Path.IsPathRooted(_options.Directory)
            ? _options.Directory
            : Path.Combine(contentRoot, _options.Directory);

        if (!Directory.Exists(directory))
        {
            return Array.Empty<ScenarioDefinition>();
        }

        var scenarios = new List<ScenarioDefinition>();
        foreach (var file in Directory.EnumerateFiles(directory, "*.json", SearchOption.AllDirectories))
        {
            await using var stream = File.OpenRead(file);
            var definition = await JsonSerializer.DeserializeAsync<ScenarioDefinition>(stream, SerializerOptions.Instance);
            if (definition is not null)
            {
                scenarios.Add(definition);
                _cache[definition.Id] = definition;
            }
        }

        return scenarios;
    }

    private sealed class SerializerOptions
    {
        public static readonly JsonSerializerOptions Instance = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
    }
}

public sealed class ScenarioDataOptions
{
    public string Directory { get; set; } = "jsonFiles/scenarios";
}
