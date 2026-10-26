using RIDE.Domain;

namespace RIDE.Integrations;

public interface IDcrsCaseSource
{
    Task<RideCaseInput?> GetUnprocessedCaseAsync(string caseId, CancellationToken cancellationToken = default);
}

public interface ICompaniesHouseSource
{
    Task<CompaniesHouseSnapshot> GetCompanyAsync(string companyNumber, CancellationToken cancellationToken = default);
}

public sealed class LocalDcrsCaseSource : IDcrsCaseSource
{
    public Task<RideCaseInput?> GetUnprocessedCaseAsync(string caseId, CancellationToken cancellationToken = default)
    {
        if (!caseId.Equals("CASE-12345", StringComparison.OrdinalIgnoreCase)) return Task.FromResult<RideCaseInput?>(null);

        return Task.FromResult<RideCaseInput?>(new RideCaseInput
        {
            CaseId = caseId,
            Company = new Company { CompanyNumber = "12345678", Name = "Example Ltd", IncorporatedDate = new DateOnly(2018, 4, 12), Status = "Liquidation" },
            Directors = [new Director { Name = "Example Director", OtherCompanyNumbers = ["09876543"] }],
            Dcrs = new DcrsAnswers
            {
                StructuredAnswers = new Dictionary<string, string> { ["section-8-1"] = "Yes", ["section-8-2"] = "Connected party transaction" },
                FreeTextAnswers = [new FreeTextAnswer { QuestionId = "Q17", QuestionText = "Describe the transaction", Answer = "A connected party asset disposal took place shortly before insolvency." }]
            },
            Evidence = [new EvidenceDocument { Name = "BankStatement.pdf", ExtractedText = "Payment evidence is present on page 8.", Pages = [8] }]
        });
    }
}

public sealed class LocalCompaniesHouseSource : ICompaniesHouseSource
{
    public Task<CompaniesHouseSnapshot> GetCompanyAsync(string companyNumber, CancellationToken cancellationToken = default) => Task.FromResult(new CompaniesHouseSnapshot
    {
        CompanyNumber = companyNumber,
        Name = "Example Ltd",
        Status = "Liquidation",
        Directors = [new CompaniesHouseDirector { Name = "Example Director", OtherCompanyNumbers = ["09876543"] }],
        AssociatedCompanies = ["09876543"]
    });
}
