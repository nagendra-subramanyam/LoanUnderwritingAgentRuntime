namespace LoanUnderwritingAgentRuntime.Domain.Exceptions;

public class AgentLoopException : Exception
{
    public string ToolName { get; }
    public int ExecutionCount { get; }

    public AgentLoopException(string toolName, int executionCount)
        : base($"Agent loop detected: Tool '{toolName}' has executed {executionCount} times with identical inputs.")
    {
        ToolName = toolName;
        ExecutionCount = executionCount;
    }
}