using LoanUnderwritingAgentRuntime.Application.Common.Models;

namespace LoanUnderwritingAgentRuntime.Application.Common.Models;

public class AgentContext
{
    public required string Goal { get; init; }
    public List<AgentTurn> History { get; } = new();
    public int IterationCount { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;
    public string? FinalAnswer { get; set; }
}