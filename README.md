# RIDE backend

RIDE (Rules, Intelligence, Decision and Escalation) turns a DCRS submission into a canonical case, applies deterministic rules, and stores an assessment for the Vetter/Investigator app.

## Projects

- `RIDE.Api` exposes processing and assessment retrieval endpoints.
- `RIDE.Processing` owns the use case and orchestration pipeline.
- `RIDE.Rules` owns hard gates, scoring, severity, evidence strength and priority.
- `RIDE.AI` owns the evidence-analysis seam. The local implementation is deterministic and can be replaced by Azure OpenAI/Foundry.
- `RIDE.Integrations` owns DCRS, Companies House and document-extraction adapters.
- `RIDE.Domain` contains the canonical `RideCase` and assessment models.
- `RIDE.Data` owns assessment persistence. The first implementation is in-memory for local development.
- `RIDE.Worker` provides the local command-line runner.

## Local processing

The local fixture supports the example case:

```bash
dotnet run --project RIDE.Worker -- process CASE-12345
```

The command builds a canonical case before analysis. AI produces indicators, missing information and evidence references; the rules engine calculates severity, evidence strength and priority.

## API

```text
POST /api/ride/cases/CASE-12345/process
GET  /api/ride/cases/CASE-12345/assessment
```

The local adapters are deliberately small. Replace them with HTTP/database/document services without changing the processing pipeline or the canonical case contract.
