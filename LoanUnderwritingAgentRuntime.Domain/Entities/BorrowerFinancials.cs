namespace LoanUnderwritingAgentRuntime.Domain.Entities;

public class BorrowerFinancials
{
    public required string BorrowerId { get; init; }
    public double MonthlyIncome { get; init; }
    public double MonthlyDebt { get; init; }
    public int CreditScore { get; init; }
}