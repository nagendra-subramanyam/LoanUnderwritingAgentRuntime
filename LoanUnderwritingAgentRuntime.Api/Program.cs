using System.Text.Json;
using LoanUnderwritingAgentRuntime.Application.DTOs;
using LoanUnderwritingAgentRuntime.Application.Interfaces;
using LoanUnderwritingAgentRuntime.Application.Orchestration;
using LoanUnderwritingAgentRuntime.Domain.ValueObjects;
using LoanUnderwritingAgentRuntime.Infrastructure.Brains;
using LoanUnderwritingAgentRuntime.Infrastructure.Tools;

var builder = WebApplication.CreateBuilder(args);

// Register Core Logging
builder.Services.AddLogging(c => c.AddConsole());

// Register Infrastructure Tools
builder.Services.AddSingleton<IAgentTool, FetchBorrowerFinancialsTool>();
builder.Services.AddSingleton<IAgentTool, CalculateDtiTool>();
builder.Services.AddSingleton<IAgentTool, QueryMortgageGuidelinesTool>();

// Register Infrastructure LLM Brain
builder.Services.AddSingleton<ILlmBrain, MockUnderwritingLlmBrain>();

// Register Application Orchestrator
builder.Services.AddScoped<IAgentRuntimeEngine, AgentRuntimeEngine>();

var app = builder.Build();

// REST Endpoint to trigger the autonomous agent
app.MapPost("/api/underwriting/evaluate", async (
    UnderwritingRequest request, 
    IAgentRuntimeEngine agentEngine, 
    CancellationToken ct) =>
{
    string goal = $"Underwrite loan application for borrower '{request.BorrowerId}'. Evaluate DTI, credit score, and guideline policy.";

    try
    {
        var resultContext = await agentEngine.ExecuteAsync(goal, ct);

        // Deserialize agent structured output
        var decision = JsonSerializer.Deserialize<UnderwritingDecision>(
            resultContext.FinalAnswer!, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return Results.Ok(new
        {
            BorrowerId = request.BorrowerId,
            TurnsTaken = resultContext.IterationCount,
            Decision = decision,
            ExecutionScratchpad = resultContext.History
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
});

app.Run();