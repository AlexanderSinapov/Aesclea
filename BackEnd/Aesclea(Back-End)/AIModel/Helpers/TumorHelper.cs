using System.Collections.Generic;

namespace Aesclea_Back_End_.AIModel.Helpers
{
    /// <summary>
    /// Helper class providing metadata for tumor classification
    /// </summary>
    public class TumorHelper
    {
        /// <summary>
        /// Available tumor types for classification
        /// </summary>
        public string[] TumorType = {
            // Benign tumors
            "Adenoma", "Papilloma", "Lipoma", "Fibroma", "Chondroma", "Schwannoma", "Neurofibroma", "Mature Teratoma", "Nevus", "Pituitary adenoma", 
            // Malignant tumors
            "Carcinoma", "Sarcoma", "Leukemia", "Lymphoma", "Myeloma", "Glioblastoma", "Medulloblastoma", "Neuroblastoma",
            "Seminoma", "Embryonal carcinoma", "Yolk sac tumor", "Choriocarcinoma", "Melanoma", 
            // Brain and CNS tumors
            "Glioma", "Astrocytoma", "Oligodendroglioma", "Ependymoma", "Meningioma", "Acoustic neuroma", "Craniopharyngioma", "Pineoblastoma",
            // Pediatric and specific organ tumors
            "Retinoblastoma", "Rhabdomyosarcoma", "Wilms tumor", "Hepatoblastoma", "Teratoma", "Dysgerminoma"
        };

        /// <summary>
        /// TNM staging system components
        /// T: Primary tumor size/extent
        /// N: Regional lymph node involvement
        /// M: Distant metastasis
        /// </summary>
        public string[][] TNMStaging =
        {
            // T - Primary tumor
            [ "TX", "T0", "Tis", "T1", "T1a", "T1b", "T1c", "T2", "T2a", "T2b", "T3", "T3a", "T3b", "T4", "T4a", "T4b", "T4c", "T4d" ],
            // N - Regional lymph nodes
            [ "NX", "N0", "N1", "N1a", "N1b", "N1c", "N2", "N2a", "N2b", "N3", "N3a", "N3b", "N3c" ],
            // M - Distant metastasis
            [ "M0", "M1", "M1a", "M1b", "M1c" ]
        };

        /// <summary>
        /// Simplified stage descriptions for generalized tumor staging
        /// </summary>
        public Dictionary<int, string> GeneralizedStage = new Dictionary<int, string>
        {
            { 0, "Stage 0 - Carcinoma in situ (pre-invasive)" },
            { 1, "Stage I - Localized tumor, minimal invasion" },
            { 2, "Stage II - Locally advanced, possible lymph node involvement" },
            { 3, "Stage III - Advanced local or regional spread" },
            { 4, "Stage IV - Distant metastasis" }
        };

        /// <summary>
        /// Tumor grade descriptions (cellular differentiation)
        /// </summary>
        public Dictionary<int, string> TumorGrade = new Dictionary<int, string>
        {
            { 1, "Grade 1 - Well differentiated (low-grade)" },
            { 2, "Grade 2 - Moderately differentiated (intermediate-grade)" },
            { 3, "Grade 3 - Poorly differentiated (high-grade)" },
            { 4, "Grade 4 - Undifferentiated/anaplastic (highest-grade)" }
        };

        /// <summary>
        /// Tumor locations with common associated tumor types
        /// </summary>
        public Dictionary<string, string[]> TumorLocation = new Dictionary<string, string[]>
        {
            { "Brain & CNS", new string[] {
                "Glioblastoma", "Meningioma", "Medulloblastoma", "Astrocytoma", "Oligodendroglioma",
                "Ependymoma", "Pituitary adenoma", "Acoustic neuroma", "Craniopharyngioma", "Pineoblastoma"
            }},
            { "Head & Neck", new string[] {
                "Nasopharyngeal carcinoma", "Laryngeal cancer", "Oral squamous cell carcinoma",
                "Salivary gland tumor", "Thyroid cancer", "Parathyroid adenoma"
            }},
            { "Thorax", new string[] {
                "Non-small cell lung cancer (NSCLC)", "Small cell lung cancer (SCLC)",
                "Invasive ductal carcinoma", "Lobular carcinoma", "Thymoma", "Mesothelioma"
            }},
            { "Abdomen", new string[] {
                "Hepatocellular carcinoma", "Pancreatic adenocarcinoma", "Gastric carcinoma",
                "Colorectal cancer", "Gallbladder carcinoma", "Renal cell carcinoma"
            }},
            { "Pelvis", new string[] {
                "Prostatic adenocarcinoma", "Transitional cell carcinoma", "Cervical squamous cell carcinoma",
                "Serous cystadenocarcinoma", "Endometrial carcinoma", "Ovarian cancer"
            }},
            { "Bone and Soft Tissue", new string[] {
                "Osteosarcoma", "Ewing's sarcoma", "Rhabdomyosarcoma", "Liposarcoma",
                "Chondrosarcoma", "Fibrosarcoma", "Leiomyosarcoma"
            }},
            { "Blood and Lymphatic System", new string[] {
                "Acute lymphoblastic leukemia", "Acute myeloid leukemia", "Chronic lymphocytic leukemia",
                "Chronic myeloid leukemia", "Hodgkin lymphoma", "Non-Hodgkin lymphoma", "Multiple myeloma"
            }}
        };

        /// <summary>
        /// Common growth patterns of tumors
        /// </summary>
        public Dictionary<string, string> TumorGrowthPattern = new Dictionary<string, string>
        {
            { "Expansile", "Tumor grows as a cohesive mass, pushing surrounding tissue aside" },
            { "Infiltrative", "Tumor cells invade between normal tissue elements without a clear border" },
            { "Multifocal", "Multiple separate tumor foci within the same organ" },
            { "Diffuse", "Widespread involvement without discrete mass formation" },
            { "Polypoid", "Tumor grows as a mass protruding into a lumen or cavity" },
            { "Ulcerative", "Tumor with central necrosis causing ulceration of overlying surface" }
        };

        /// <summary>
        /// Simplified biological behavior categories
        /// </summary>
        public Dictionary<string, string> BiologicalBehavior = new Dictionary<string, string>
        {
            { "Benign", "Non-invasive, well-circumscribed tumors that do not metastasize" },
            { "Borderline/Intermediate", "Tumors with unpredictable behavior, locally aggressive but rarely metastasizing" },
            { "Malignant", "Invasive tumors with potential for metastasis" },
            { "In situ", "Malignant cells that have not invaded through the basement membrane" }
        };

        /// <summary>
        /// Common treatment modalities based on tumor characteristics
        /// </summary>
        public Dictionary<string, string[]> CommonTreatmentApproaches = new Dictionary<string, string[]>
        {
            { "Surgery", new string[] { "Resection", "Excision", "Lobectomy", "Mastectomy", "Prostatectomy" } },
            { "Radiation", new string[] { "External beam", "Brachytherapy", "Stereotactic radiosurgery", "Proton therapy" } },
            { "Systemic Therapy", new string[] { "Chemotherapy", "Immunotherapy", "Hormone therapy", "Targeted therapy" } },
            { "Combined Modality", new string[] { "Neoadjuvant", "Adjuvant", "Concurrent", "Salvage" } }
        };

        /// <summary>
        /// Genomic alterations frequently associated with certain tumor types
        /// </summary>
        public Dictionary<string, string[]> CommonGenomicAlterations = new Dictionary<string, string[]>
        {
            { "Mutations", new string[] { "EGFR", "BRAF", "KRAS", "TP53", "PIK3CA", "IDH1/2", "BRCA1/2" } },
            { "Translocations", new string[] { "BCR-ABL", "EWS-FLI1", "ALK", "MYC" } },
            { "Amplifications", new string[] { "HER2", "MYCN", "EGFR", "MET" } },
            { "Deletions", new string[] { "CDKN2A", "PTEN", "RB1", "TP53" } }
        };

        /// <summary>
        /// Common biomarkers used for tumor detection and monitoring
        /// </summary>
        public Dictionary<string, string> TumorBiomarkers = new Dictionary<string, string>
        {
            { "AFP", "Alpha-fetoprotein - Liver cancer, germ cell tumors" },
            { "CEA", "Carcinoembryonic antigen - Colorectal, pancreatic, gastric, lung cancers" },
            { "CA-125", "Cancer antigen 125 - Ovarian cancer" },
            { "PSA", "Prostate-specific antigen - Prostate cancer" },
            { "CA 19-9", "Cancer antigen 19-9 - Pancreatic cancer" },
            { "hCG", "Human chorionic gonadotropin - Gestational trophoblastic disease, germ cell tumors" },
            { "LDH", "Lactate dehydrogenase - Lymphomas, germ cell tumors, melanoma" }
        };

        /// <summary>
        /// Get detailed description for a specific tumor type
        /// </summary>
        public string GetTumorTypeDescription(string tumorType)
        {
            var descriptions = new Dictionary<string, string>
            {
                { "Adenoma", "Benign epithelial tumor with glandular organization" },
                { "Carcinoma", "Malignant epithelial tumor with potential for invasion and metastasis" },
                { "Sarcoma", "Malignant tumor of mesenchymal origin (connective tissue, bone, muscle)" },
                { "Lymphoma", "Malignant tumor originating from lymphoid tissue" },
                { "Glioblastoma", "Highly aggressive malignant astrocytic brain tumor (WHO grade IV)" },
                { "Melanoma", "Malignant tumor arising from melanocytes, typically in skin" }
                // Add more as needed
            };

            if (descriptions.ContainsKey(tumorType))
                return descriptions[tumorType];

            return "Detailed description not available";
        }

        /// <summary>
        /// Determine if a tumor type is generally considered benign or malignant
        /// </summary>
        public string DetermineMalignancyStatus(string tumorType)
        {
            string[] benignTypes = { "Adenoma", "Papilloma", "Lipoma", "Fibroma", "Chondroma", "Schwannoma",
                                    "Neurofibroma", "Mature Teratoma", "Nevus", "Pituitary adenoma" };

            foreach (var benign in benignTypes)
            {
                if (tumorType.Contains(benign))
                    return "Benign";
            }

            return "Malignant or potentially malignant";
        }
    }
}
