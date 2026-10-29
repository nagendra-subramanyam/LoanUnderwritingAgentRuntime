using System.Text.Json.Serialization;

namespace LoanUnderwritingAgentRuntime.Domain.ValueObjects;

public class UnderwritingDecision
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty; // "Approved", "Denied", "Referred"

    [JsonPropertyName("calculatedDti")]
    public double CalculatedDti { get; set; }

    [JsonPropertyName("rationale")]
    public string Rationale { get; set; } = string.Empty;

    [JsonPropertyName("conditions")]
    public List<string> Conditions { get; set; } = new();
}