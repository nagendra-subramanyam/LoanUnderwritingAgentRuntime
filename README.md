# Enterprise Underwriting Agent Runtime - Clean Architecture (.NET 10)

This solution implements the complete ReAct (Reason + Act) autonomous agent orchestration engine separated into 4 distinct physical layers.

## Solution Layout:
- **`LoanUnderwritingAgentRuntime.Domain`**: Core business entities, value objects, and domain exceptions (zero external dependencies).
- **`LoanUnderwritingAgentRuntime.Application`**: Agent abstractions (`IAgentTool`, `ILlmBrain`, `IAgentRuntimeEngine`), ReAct orchestration engine, and loop detection logic.
- **`LoanUnderwritingAgentRuntime.Infrastructure`**: Concrete tool implementations (financial lookup, DTI calculator, mortgage guidelines) and LLM brain.
- **`LoanUnderwritingAgentRuntime.Api`**: ASP.NET Core Web API Minimal API endpoints and Dependency Injection container wiring.

## How to Debug in Visual Studio:
1. Extract this zip file.
2. Open `LoanUnderwritingAgentRuntime.sln` in Visual Studio.
3. Ensure `LoanUnderwritingAgentRuntime.Api` is set as the **Startup Project** (right click `LoanUnderwritingAgentRuntime.Api` -> **Set as Startup Project**).
4. Set breakpoints inside:
   - `LoanUnderwritingAgentRuntime.Application/Orchestration/AgentRuntimeEngine.cs` (Inside `ExecuteAsync`)
   - `LoanUnderwritingAgentRuntime.Infrastructure/Brains/MockUnderwritingLlmBrain.cs` (Inside `PlanNextStepAsync`)
   - `LoanUnderwritingAgentRuntime.Infrastructure/Tools/CalculateDtiTool.cs` (Inside `ExecuteAsync`)
5. Press **F5** to start debugging.
6. Test with curl, Postman, or PowerShell:
```bash
curl -X POST http://localhost:5240/api/underwriting/evaluate \
  -H "Content-Type: application/json" \
  -d '{"borrowerId":"BORR-1001"}'
```