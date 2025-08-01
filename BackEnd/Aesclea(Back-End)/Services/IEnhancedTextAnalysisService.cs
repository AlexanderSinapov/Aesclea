using Aesclea_Back_End_.Models;

namespace Aesclea_Back_End_.Services
{
    public interface IEnhancedTextAnalysisService
    {
        Task<EnhancedTextAnalysisResult> AnalyzeMedicalTextAsync(string text, TextAnalysisOptions? options = null);
    }
}
