using LoanUnderwritingAgentRuntime.Application.Common.Models;

namespace LoanUnderwritingAgentRuntime.Application.Interfaces;

public interface IAgentRuntimeEngine
{
    Task<AgentContext> ExecuteAsync(string userGoal, CancellationToken ct = default);
}