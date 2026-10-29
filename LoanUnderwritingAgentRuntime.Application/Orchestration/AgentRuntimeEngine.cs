using LoanUnderwritingAgentRuntime.Application.Common.Models;
using LoanUnderwritingAgentRuntime.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace LoanUnderwritingAgentRuntime.Application.Orchestration;

public class AgentRuntimeEngine : IAgentRuntimeEngine
{
    private readonly ILlmBrain _brain;
    private readonly Dictionary<string, IAgentTool> _tools;
    private readonly ILogger<AgentRuntimeEngine> _logger;
    private const int MaxAllowedTurns = 6;

    public AgentRuntimeEngine(
        ILlmBrain brain, 
        IEnumerable<IAgentTool> tools, 
        ILogger<AgentRuntimeEngine> logger)
    {
        _brain = brain;
        _tools = tools.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
        _logger = logger;
    }

    public async Task<AgentContext> ExecuteAsync(string userGoal, CancellationToken ct = default)
    {
        var context = new AgentContext { Goal = userGoal };
        var loopDetector = new AgentLoopDetector(maxRepetitionsAllowed: 2);

        _logger.LogInformation("Agent initialized with goal: {Goal}", userGoal);

        while (!context.IsCompleted && context.IterationCount < MaxAllowedTurns)
        {
            context.IterationCount++;
            _logger.LogInformation("--- Agent Step {Step} ---", context.IterationCount);

            // 1. REASON
            var decision = await _brain.PlanNextStepAsync(
                context.Goal, 
                context.History, 
                _tools.Values, 
                ct
            );

            _logger.LogInformation("Agent Thought: {Thought}", decision.Thought);

            // 2. CHECK COMPLETION
            if (!decision.HasAction)
            {
                context.FinalAnswer = decision.FinalOutput;
                context.IsCompleted = true;
                _logger.LogInformation("Agent goal satisfied.");
                break;
            }

            // 3. VALIDATE LOOP SAFETY
            loopDetector.TrackAndValidate(decision.ToolName, decision.ToolArguments);

            // 4. ACT
            if (!_tools.TryGetValue(decision.ToolName!, out var tool))
            {
                var errorObservation = $"Error: Tool '{decision.ToolName}' does not exist.";
                _logger.LogWarning("{Error}", errorObservation);
                context.History.Add(new AgentTurn(decision.Thought, decision.ToolName, decision.ToolArguments, errorObservation));
                continue;
            }

            // 5. OBSERVE
            string observation;
            try
            {
                _logger.LogInformation("Executing Tool: {Tool} with Args: {Args}", decision.ToolName, decision.ToolArguments);
                observation = await tool.ExecuteAsync(decision.ToolArguments ?? "{}", ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tool execution failed.");
                observation = $"Tool execution error: {ex.Message}";
            }

            // 6. RECORD
            context.History.Add(new AgentTurn(
                Thought: decision.Thought,
                ToolCalled: decision.ToolName,
                ToolArguments: decision.ToolArguments,
                ToolObservation: observation
            ));
        }

        if (!context.IsCompleted)
        {
            throw new TimeoutException($"Agent failed to conclude within the limit of {MaxAllowedTurns} turns.");
        }

        return context;
    }
}