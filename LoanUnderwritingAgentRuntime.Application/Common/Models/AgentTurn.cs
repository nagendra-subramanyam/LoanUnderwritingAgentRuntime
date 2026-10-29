namespace LoanUnderwritingAgentRuntime.Application.Common.Models;

public record AgentTurn(
    string Thought, 
    string? ToolCalled, 
    string? ToolArguments, 
    string? ToolObservation
);