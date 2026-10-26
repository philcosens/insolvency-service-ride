using RIDE.Domain;

namespace RIDE.Data;

public interface IRideAssessmentStore
{
    Task SaveAsync(RideCaseAssessment assessment, CancellationToken cancellationToken = default);
    Task<RideCaseAssessment?> GetAsync(string caseId, CancellationToken cancellationToken = default);
}

public sealed class InMemoryRideAssessmentStore : IRideAssessmentStore
{
    private readonly Dictionary<string, RideCaseAssessment> assessments = new(StringComparer.OrdinalIgnoreCase);

    public Task SaveAsync(RideCaseAssessment assessment, CancellationToken cancellationToken = default)
    {
        assessments[assessment.CaseId] = assessment;
        return Task.CompletedTask;
    }

    public Task<RideCaseAssessment?> GetAsync(string caseId, CancellationToken cancellationToken = default) =>
        Task.FromResult(assessments.GetValueOrDefault(caseId));
}
