using LoanUnderwritingAgentRuntime.Application.Common.Models;

namespace LoanUnderwritingAgentRuntime.Application.Interfaces;

public interface ILlmBrain
{
    Task<LlmDecision> PlanNextStepAsync(
        string goal, 
        IReadOnlyList<AgentTurn> history, 
        IEnumerable<IAgentTool> availableTools, 
        CancellationToken ct
    );
}