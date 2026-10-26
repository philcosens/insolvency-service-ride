using Microsoft.AspNetCore.Mvc;
using RIDE.Data;
using RIDE.Processing;

namespace RIDE.Api.Controllers;

[ApiController]
[Route("api/ride/cases")]
public sealed class RideCasesController(IRideCaseProcessor processor, IRideAssessmentStore store) : ControllerBase
{
    [HttpPost("{caseId}/process")]
    public async Task<ActionResult> Process(string caseId, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await processor.ProcessAsync(caseId, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    [HttpGet("{caseId}/assessment")]
    public async Task<ActionResult> GetAssessment(string caseId, CancellationToken cancellationToken)
    {
        var assessment = await store.GetAsync(caseId, cancellationToken);
        return assessment is null ? NotFound() : Ok(assessment);
    }
}
