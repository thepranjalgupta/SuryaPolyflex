namespace SuryaPolyFlex.Application.Common.Interfaces;

public interface INumberSequenceService
{
    Task<string> GenerateAsync(string moduleCode);
}