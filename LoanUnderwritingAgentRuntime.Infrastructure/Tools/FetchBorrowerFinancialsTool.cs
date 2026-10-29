using System.Text.Json;
using System.Text.Json.Nodes;
using LoanUnderwritingAgentRuntime.Application.Interfaces;
using LoanUnderwritingAgentRuntime.Domain.Entities;

namespace LoanUnderwritingAgentRuntime.Infrastructure.Tools;

public class FetchBorrowerFinancialsTool : IAgentTool
{
    public string Name => "FetchBorrowerFinancials";
    public string Description => "Retrieves official monthly gross income, monthly debt obligations, and credit score for a borrower by their ID.";

    public JsonObject InputSchema => new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["borrowerId"] = new JsonObject 
            { 
                ["type"] = "string", 
                ["description"] = "The unique borrower identifier (e.g. BORR-1001)" 
            }
        },
        ["required"] = new JsonArray { "borrowerId" }
    };

    public Task<string> ExecuteAsync(string argumentsJson, CancellationToken ct)
    {
        using var doc = JsonDocument.Parse(argumentsJson);
        var borrowerId = doc.RootElement.GetProperty("borrowerId").GetString();

        var financials = borrowerId switch
        {
            "BORR-1001" => new BorrowerFinancials { BorrowerId = "BORR-1001", MonthlyIncome = 12500.00, MonthlyDebt = 3100.00, CreditScore = 740 },
            "BORR-1002" => new BorrowerFinancials { BorrowerId = "BORR-1002", MonthlyIncome = 4500.00, MonthlyDebt = 2800.00, CreditScore = 610 },
            _ => throw new KeyNotFoundException($"Borrower '{borrowerId}' not found in record system.")
        };

        return Task.FromResult(JsonSerializer.Serialize(financials));
    }
}