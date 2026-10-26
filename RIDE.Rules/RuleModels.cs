using RIDE.Domain;

namespace RIDE.Rules;

public interface IRideRulesEngine
{
    RuleEvaluation Evaluate(RideCase rideCase, EvidenceAnalysis evidenceAnalysis);
}

public sealed class RuleEvaluation
{
    public int Severity { get; init; }
    public int EvidenceStrength { get; init; }
    public int Priority { get; init; }
    public List<string> Indicators { get; init; } = [];
    public List<string> MissingInformation { get; init; } = [];
    public List<string> EvidenceReferences { get; init; } = [];
    public string Recommendation { get; init; } = string.Empty;
}

public sealed class RideRulesEngine : IRideRulesEngine
{
    public RuleEvaluation Evaluate(RideCase rideCase, EvidenceAnalysis evidenceAnalysis)
    {
        var indicators = evidenceAnalysis.Indicators.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var missing = evidenceAnalysis.MissingInformation.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        var severity = 20;
        if (HasIndicator(indicators, "Connected-party transaction identified")) severity += 25;
        if (HasIndicator(indicators, "Disposal appears to have occurred shortly before insolvency")) severity += 25;
        if (HasIndicator(indicators, "Serious misconduct indicator")) severity += 20;

        var evidenceStrength = 25;
        if (rideCase.Evidence.Count > 0) evidenceStrength += 20;
        if (HasIndicator(indicators, "Documentary evidence present")) evidenceStrength += 20;
        if (HasIndicator(indicators, "Companies House corroboration")) evidenceStrength += 20;
        evidenceStrength -= Math.Min(20, missing.Count * 5);

        var priority = (int)Math.Round((severity * 0.55) + (evidenceStrength * 0.45));
        var recommendation = priority >= 70 ? "Vetter review recommended" : "Routine review recommended";

        return new RuleEvaluation
        {
            Severity = Clamp(severity),
            EvidenceStrength = Clamp(evidenceStrength),
            Priority = Clamp(priority),
            Indicators = indicators,
            MissingInformation = missing,
            EvidenceReferences = evidenceAnalysis.EvidenceReferences,
            Recommendation = recommendation
        };
    }

    private static bool HasIndicator(IEnumerable<string> indicators, string value) => indicators.Any(indicator => indicator.Equals(value, StringComparison.OrdinalIgnoreCase));
    private static int Clamp(int value) => Math.Clamp(value, 0, 100);
}
