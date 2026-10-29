using System.Text.Json.Nodes;

namespace LoanUnderwritingAgentRuntime.Application.Interfaces;

public interface IAgentTool
{
    string Name { get; }
    string Description { get; }
    JsonObject InputSchema { get; }
    Task<string> ExecuteAsync(string argumentsJson, CancellationToken ct);
}