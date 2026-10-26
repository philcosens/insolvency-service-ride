using RIDE.AI;
using RIDE.Data;
using RIDE.Integrations;
using RIDE.Processing;
using RIDE.Rules;

if (args.Length != 2 || !args[0].Equals("process", StringComparison.OrdinalIgnoreCase))
{
    Console.Error.WriteLine("Usage: ride process CASE-12345");
    return 1;
}

var processor = new RideCaseProcessor(
    new LocalDcrsCaseSource(),
    new LocalCompaniesHouseSource(),
    new LocalRideAiAnalyzer(),
    new RideRulesEngine(),
    new InMemoryRideAssessmentStore());

try
{
    var assessment = await processor.ProcessAsync(args[1]);
    Console.WriteLine(RideAssessmentText.Format(assessment));
    return 0;
}
catch (InvalidOperationException exception)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}
