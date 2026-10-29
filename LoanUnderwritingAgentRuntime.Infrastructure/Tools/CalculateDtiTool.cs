using System.Text.Json;
using System.Text.Json.Nodes;
using LoanUnderwritingAgentRuntime.Application.Interfaces;

namespace LoanUnderwritingAgentRuntime.Infrastructure.Tools;

public class CalculateDtiTool : IAgentTool
{
    public string Name => "CalculateDti";
    public string Description => "Accurately calculates the Debt-to-Income (DTI) ratio percentage given monthly debt and monthly income.";

    public JsonObject InputSchema => new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["monthlyDebt"] = new JsonObject { ["type"] = "number", ["description"] = "Total recurring monthly debt payments." },
            ["monthlyIncome"] = new JsonObject { ["type"] = "number", ["description"] = "Total verifiable gross monthly income." }
        },
        ["required"] = new JsonArray { "monthlyDebt", "monthlyIncome" }
    };

    public Task<string> ExecuteAsync(string argumentsJson, CancellationToken ct)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        double debt = doc.RootElement.GetProperty("monthlyDebt").GetDouble();
        double income = doc.RootElement.GetProperty("monthlyIncome").GetDouble();

        if (income <= 0)
        {
            return Task.FromResult(JsonSerializer.Serialize(new { error = "Monthly income must be greater than zero." }));
        }

        double dtiPercentage = Math.Round((debt / income) * 100.0, 2);
        return Task.FromResult(JsonSerializer.Serialize(new { DtiPercentage = dtiPercentage }));
    }
}