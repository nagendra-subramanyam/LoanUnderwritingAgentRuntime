using LoanUnderwritingAgentRuntime.Application.Common.Models;
using LoanUnderwritingAgentRuntime.Application.Interfaces;

namespace LoanUnderwritingAgentRuntime.Infrastructure.Brains;

public class MockUnderwritingLlmBrain : ILlmBrain
{
    public Task<LlmDecision> PlanNextStepAsync(
        string goal, 
        IReadOnlyList<AgentTurn> history, 
        IEnumerable<IAgentTool> availableTools, 
        CancellationToken ct)
    {
        // Turn 1: Fetch Borrower data
        if (history.Count == 0)
        {
            return Task.FromResult(new LlmDecision(
                Thought: "To evaluate this loan application, I first need the borrower's financials.",
                HasAction: true,
                ToolName: "FetchBorrowerFinancials",
                ToolArguments: """{"borrowerId":"BORR-1001"}""",
                FinalOutput: null
            ));
        }

        // Turn 2: Calculate DTI via Tool
        if (history.Count == 1)
        {
            return Task.FromResult(new LlmDecision(
                Thought: "I have the income and debt figures. Now I must calculate the exact DTI ratio using the calculator.",
                HasAction: true,
                ToolName: "CalculateDti",
                ToolArguments: """{"monthlyIncome":12500.0,"monthlyDebt":3100.0}""",
                FinalOutput: null
            ));
        }

        // Turn 3: Query Underwriting Policy
        if (history.Count == 2)
        {
            return Task.FromResult(new LlmDecision(
                Thought: "DTI is 24.8%. I must verify against underwriting limit guidelines.",
                HasAction: true,
                ToolName: "QueryMortgageGuidelines",
                ToolArguments: """{"category":"DTI_Limits"}""",
                FinalOutput: null
            ));
        }

        // Turn 4: Final Synthesis & Decision
        return Task.FromResult(new LlmDecision(
            Thought: "All data verified. DTI of 24.8% and credit score of 740 conform to guidelines.",
            HasAction: false,
            ToolName: null,
            ToolArguments: null,
            FinalOutput: """
            {
              "status": "Approved",
              "calculatedDti": 24.8,
              "rationale": "Borrower exceeds minimum 620 credit score and maintains a 24.8% DTI, well below the 43.0% limit.",
              "conditions": [
                "Verify standard W-2 documentation prior to closing.",
                "Satisfy property appraisal condition."
              ]
            }
            """
        ));
    }
}