using RIDE.AI;
using RIDE.Data;
using RIDE.Integrations;
using RIDE.Processing;
using RIDE.Rules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IDcrsCaseSource, LocalDcrsCaseSource>();
builder.Services.AddSingleton<ICompaniesHouseSource, LocalCompaniesHouseSource>();
builder.Services.AddSingleton<IRideAiAnalyzer, LocalRideAiAnalyzer>();
builder.Services.AddSingleton<IRideRulesEngine, RideRulesEngine>();
builder.Services.AddSingleton<IRideAssessmentStore, InMemoryRideAssessmentStore>();
builder.Services.AddScoped<IRideCaseProcessor, RideCaseProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();
