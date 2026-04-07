namespace SuryaPolyFlex.Application.Common.Interfaces;

public interface IRuleEvaluationService
{
    Task<bool> EvaluateRulesAsync(string permissionName, string userId, object resourceContext);
}