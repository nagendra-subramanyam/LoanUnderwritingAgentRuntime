using System.Text.Json;
using System.Text.Json.Nodes;
using LoanUnderwritingAgentRuntime.Application.Interfaces;

namespace LoanUnderwritingAgentRuntime.Infrastructure.Tools;

public class QueryMortgageGuidelinesTool : IAgentTool
{
    public string Name => "QueryMortgageGuidelines";
    public string Description => "Queries underwriting risk policies for conventional conforming loans.";

    public JsonObject InputSchema => new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["category"] = new JsonObject { ["type"] = "string", ["description"] = "Policy area such as 'DTI_Limits' or 'Credit_Thresholds'." }
        },
        ["required"] = new JsonArray { "category" }
    };

    public Task<string> ExecuteAsync(string argumentsJson, CancellationToken ct)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var category = doc.RootElement.GetProperty("category").GetString();

        var policy = category switch
        {
            "DTI_Limits" => "Standard maximum DTI is 43.00%. Ratios between 43.01% and 50.00% require an underwriting referral with compensating factors.",
            "Credit_Thresholds" => "Minimum allowable credit score is 620. Scores under 620 result in automatic denial.",
            _ => "No explicit policy matched. Escalate to senior human underwriter."
        };

        return Task.FromResult(JsonSerializer.Serialize(new { Policy = policy }));
    }
}