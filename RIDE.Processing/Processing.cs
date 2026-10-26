using RIDE.AI;
using RIDE.Data;
using RIDE.Domain;
using RIDE.Integrations;
using RIDE.Rules;

namespace RIDE.Processing;

public interface IRideCaseProcessor
{
    Task<RideCaseAssessment> ProcessAsync(string caseId, CancellationToken cancellationToken = default);
}

public sealed class RideCaseProcessor(
    IDcrsCaseSource dcrs,
    ICompaniesHouseSource companiesHouse,
    IRideAiAnalyzer ai,
    IRideRulesEngine rules,
    IRideAssessmentStore store) : IRideCaseProcessor
{
    public async Task<RideCaseAssessment> ProcessAsync(string caseId, CancellationToken cancellationToken = default)
    {
        var input = await dcrs.GetUnprocessedCaseAsync(caseId, cancellationToken)
            ?? throw new InvalidOperationException($"No unprocessed DCRS submission was found for '{caseId}'.");

        var house = await companiesHouse.GetCompanyAsync(input.Company.CompanyNumber, cancellationToken);
        var rideCase = new RideCase
        {
            CaseId = input.CaseId,
            Company = input.Company,
            Directors = input.Directors,
            Dcrs = input.Dcrs,
            Evidence = input.Evidence,
            CompaniesHouse = house,
            ExistingIndicators = input.ExistingIndicators
        };

        var analysis = await ai.AnalyseAsync(rideCase, cancellationToken);
        var evaluation = rules.Evaluate(rideCase, analysis);
        var assessment = new RideCaseAssessment
        {
            CaseId = rideCase.CaseId,
            SuggestedCaseType = analysis.SuggestedCaseType,
            Severity = evaluation.Severity,
            EvidenceStrength = evaluation.EvidenceStrength,
            Priority = evaluation.Priority,
            KeyIndicators = evaluation.Indicators,
            MissingInformation = evaluation.MissingInformation,
            Recommendation = evaluation.Recommendation,
            EvidenceReferences = evaluation.EvidenceReferences
        };

        await store.SaveAsync(assessment, cancellationToken);
        return assessment;
    }
}

public static class RideAssessmentText
{
    public static string Format(RideCaseAssessment assessment) => $"""
        RIDE CASE ASSESSMENT

        Case: {assessment.CaseId}

        Suggested case type: {assessment.SuggestedCaseType}

        Severity: {assessment.Severity} / 100

        Evidence strength: {assessment.EvidenceStrength} / 100

        Priority: {assessment.Priority} / 100

        Key indicators:
        {BulletList(assessment.KeyIndicators)}

        Missing information:
        {BulletList(assessment.MissingInformation)}

        Recommendation:
        {assessment.Recommendation}

        Evidence:
        {BulletList(assessment.EvidenceReferences)}
        """;

    private static string BulletList(IEnumerable<string> values) => string.Join(Environment.NewLine, values.Select(value => $"- {value}"));
}
