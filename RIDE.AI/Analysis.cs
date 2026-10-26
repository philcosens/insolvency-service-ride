using RIDE.Domain;

namespace RIDE.AI;

public interface IRideAiAnalyzer
{
    Task<EvidenceAnalysis> AnalyseAsync(RideCase rideCase, CancellationToken cancellationToken = default);
}

// This deterministic implementation is the local seam for Azure OpenAI or Foundry.
// It deliberately returns structured indicators; it never calculates priority.
public sealed class LocalRideAiAnalyzer : IRideAiAnalyzer
{
    public Task<EvidenceAnalysis> AnalyseAsync(RideCase rideCase, CancellationToken cancellationToken = default)
    {
        var indicators = new List<string>();
        var missing = new List<string>();
        var references = new List<string>();

        var allText = string.Join("\n", rideCase.Dcrs.FreeTextAnswers.Select(answer => answer.Answer).Concat(rideCase.Evidence.Select(document => document.ExtractedText)));
        if (allText.Contains("connected", StringComparison.OrdinalIgnoreCase) || allText.Contains("related party", StringComparison.OrdinalIgnoreCase))
        {
            indicators.Add("Connected-party transaction identified");
            references.AddRange(rideCase.Dcrs.FreeTextAnswers.Where(answer => answer.Answer.Contains("connected", StringComparison.OrdinalIgnoreCase)).Select(answer => answer.QuestionId));
        }

        if (allText.Contains("shortly before insolvency", StringComparison.OrdinalIgnoreCase) || allText.Contains("before insolvency", StringComparison.OrdinalIgnoreCase))
        {
            indicators.Add("Disposal appears to have occurred shortly before insolvency");
        }

        if (rideCase.Evidence.Count > 0)
        {
            indicators.Add("Documentary evidence present");
            references.AddRange(rideCase.Evidence.Select(document => document.Name));
        }

        if (rideCase.CompaniesHouse.AssociatedCompanies.Count > 0)
        {
            indicators.Add("Companies House corroboration");
            indicators.Add("Director also associated with successor company");
        }

        if (allText.Contains("misconduct", StringComparison.OrdinalIgnoreCase)) indicators.Add("Serious misconduct indicator");
        if (allText.Contains("asset", StringComparison.OrdinalIgnoreCase) && !allText.Contains("valuation", StringComparison.OrdinalIgnoreCase)) missing.Add("supporting valuation");
        if (allText.Contains("consideration", StringComparison.OrdinalIgnoreCase) == false) missing.Add("consideration paid for assets");
        if (rideCase.Evidence.Count == 0) missing.Add("supporting documentation");

        var caseType = indicators.Any(indicator => indicator.Contains("successor", StringComparison.OrdinalIgnoreCase)) ? "Phoenix" : "Undetermined";
        return Task.FromResult(new EvidenceAnalysis
        {
            SuggestedCaseType = caseType,
            Indicators = indicators,
            MissingInformation = missing,
            EvidenceReferences = references.Distinct(StringComparer.OrdinalIgnoreCase).ToList()
        });
    }
}
