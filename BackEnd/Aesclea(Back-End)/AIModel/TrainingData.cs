// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

namespace Aesclea_Back_End_.AIModel
{
    /// <summary>
    /// Training data structure for medical AI model
    /// </summary>
    public class TrainingData
    {
        public string InputText { get; set; } = "";
        public int DiagnosisCategory { get; set; }
        public int SeverityLevel { get; set; }
        public int UrgencyLevel { get; set; }
        public string Specialty { get; set; } = "";
        public string Keywords { get; set; } = "";
    }
}
