namespace RIDE.Domain;

public sealed class RideCase
{
    public required string CaseId { get; init; }
    public Company Company { get; init; } = new();
    public List<Director> Directors { get; init; } = [];
    public DcrsAnswers Dcrs { get; init; } = new();
    public List<EvidenceDocument> Evidence { get; init; } = [];
    public CompaniesHouseSnapshot CompaniesHouse { get; init; } = new();
    public List<string> ExistingIndicators { get; init; } = [];
}

public sealed class Company
{
    public string CompanyNumber { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DateOnly? IncorporatedDate { get; init; }
    public string Status { get; init; } = string.Empty;
}

public sealed class Director
{
    public string Name { get; init; } = string.Empty;
    public List<string> OtherCompanyNumbers { get; init; } = [];
}

public sealed class DcrsAnswers
{
    public Dictionary<string, string> StructuredAnswers { get; init; } = [];
    public List<FreeTextAnswer> FreeTextAnswers { get; init; } = [];
}

public sealed class FreeTextAnswer
{
    public string QuestionId { get; init; } = string.Empty;
    public string QuestionText { get; init; } = string.Empty;
    public string Answer { get; init; } = string.Empty;
}

public sealed class EvidenceDocument
{
    public string Name { get; init; } = string.Empty;
    public string ExtractedText { get; init; } = string.Empty;
    public List<int> Pages { get; init; } = [];
}

public sealed class CompaniesHouseSnapshot
{
    public string CompanyNumber { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public List<CompaniesHouseDirector> Directors { get; init; } = [];
    public List<string> AssociatedCompanies { get; init; } = [];
}

public sealed class CompaniesHouseDirector
{
    public string Name { get; init; } = string.Empty;
    public List<string> OtherCompanyNumbers { get; init; } = [];
}

public sealed class RideCaseAssessment
{
    public required string CaseId { get; init; }
    public string SuggestedCaseType { get; init; } = "Undetermined";
    public int Severity { get; init; }
    public int EvidenceStrength { get; init; }
    public int Priority { get; init; }
    public List<string> KeyIndicators { get; init; } = [];
    public List<string> MissingInformation { get; init; } = [];
    public string Recommendation { get; init; } = string.Empty;
    public List<string> EvidenceReferences { get; init; } = [];
    public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class EvidenceAnalysis
{
    public string SuggestedCaseType { get; init; } = "Undetermined";
    public List<string> Indicators { get; init; } = [];
    public List<string> MissingInformation { get; init; } = [];
    public List<string> EvidenceReferences { get; init; } = [];
}

public sealed class RideCaseInput
{
    public required string CaseId { get; init; }
    public Company Company { get; init; } = new();
    public List<Director> Directors { get; init; } = [];
    public DcrsAnswers Dcrs { get; init; } = new();
    public List<EvidenceDocument> Evidence { get; init; } = [];
    public CompaniesHouseSnapshot CompaniesHouse { get; init; } = new();
    public List<string> ExistingIndicators { get; init; } = [];
}
