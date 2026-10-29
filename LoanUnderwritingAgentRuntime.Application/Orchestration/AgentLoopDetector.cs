using System.Security.Cryptography;
using System.Text;
using LoanUnderwritingAgentRuntime.Domain.Exceptions;

namespace LoanUnderwritingAgentRuntime.Application.Orchestration;

public class AgentLoopDetector
{
    private readonly List<string> _actionSignatures = new();
    private readonly int _maxRepetitionsAllowed;

    public AgentLoopDetector(int maxRepetitionsAllowed = 2)
    {
        _maxRepetitionsAllowed = maxRepetitionsAllowed;
    }

    public void TrackAndValidate(string? toolName, string? argumentsJson)
    {
        if (string.IsNullOrEmpty(toolName)) return;

        string normalized = $"{toolName.Trim()}:{argumentsJson?.Trim()}";
        string signature = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)));

        int occurrenceCount = _actionSignatures.Count(sig => sig == signature);
        if (occurrenceCount >= _maxRepetitionsAllowed)
        {
            throw new AgentLoopException(toolName, occurrenceCount + 1);
        }

        _actionSignatures.Add(signature);
    }
}