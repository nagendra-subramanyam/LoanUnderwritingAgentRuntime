namespace LoanUnderwritingAgentRuntime.Application.Common.Models;

public record LlmDecision(
    string Thought, 
    bool HasAction, 
    string? ToolName, 
    string? ToolArguments, 
    string? FinalOutput
);