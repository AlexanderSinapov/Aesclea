# Enhanced Tumor Analysis API Documentation

## Overview
The Enhanced Tumor Analysis API provides comprehensive tumor detection and classification capabilities with advanced configuration options, batch processing, and detailed reporting features.

## Base URL
```
https://your-api-domain.com/api/enhancedtumoranalysis
```

## Authentication
All endpoints require appropriate authentication headers. Contact your system administrator for API keys.

## Endpoints

### 1. Enhanced Single Image Analysis
**POST** `/analyze/enhanced`

Enhanced analysis with configurable parameters for single image processing.

#### Parameters
- `imageFile` (form-data, required): Image file (JPG, PNG, BMP, TIFF)
- `detectionThreshold` (query, optional): Detection threshold (0.0-1.0, default: 0.5)
- `classificationThreshold` (query, optional): Classification threshold (0.0-1.0, default: 0.3)
- `enableDetailedAnalysis` (query, optional): Enable detailed analysis (boolean, default: true)
- `saveAnnotated` (query, optional): Save annotated image (boolean, default: true)

#### Response
```json
{
  "basicAnalysis": {
    "id": "analysis-id",
    "hasTumor": true,
    "tumorProbability": 0.85,
    "tumorType": "Meningioma",
    "tumorGrade": 2,
    "gradeDescription": "Low-grade tumor",
    "tumorLocation": "Frontal lobe",
    "estimatedStage": 2,
    "stageDescription": "Locally advanced",
    "summary": "Complete analysis summary...",
    "analysisTimestamp": "2025-06-21T10:30:00Z"
  },
  "analysisSettings": {
    "detectionThreshold": 0.5,
    "classificationThreshold": 0.3,
    "enableDetailedAnalysis": true
  },
  "clinicalRecommendations": [
    "FOLLOW-UP: Low-intermediate grade tumor. Consider follow-up imaging in 3-6 months.",
    "Monitor for progression with serial imaging.",
    "Type-specific considerations for Meningioma should be reviewed."
  ],
  "confidenceMetrics": {
    "overallConfidence": 0.82,
    "detectionConfidence": 0.85,
    "classificationReliability": 0.78
  }
}
```

### 2. Batch Image Analysis
**POST** `/analyze/batch`

Process multiple images in a single request.

#### Parameters
- `imageFiles` (form-data, required): Multiple image files (max 50)
- `detectionThreshold` (query, optional): Detection threshold (0.0-1.0, default: 0.5)
- `saveIndividualReports` (query, optional): Save individual reports (boolean, default: false)

#### Response
```json
{
  "totalSubmitted": 10,
  "successfullyProcessed": 9,
  "processingErrors": [
    "Error processing corrupted_image.jpg: Invalid format"
  ],
  "results": [
    {
      "id": "batch-result-1",
      "originalFileName": "scan1.jpg",
      "hasTumor": true,
      "tumorProbability": 0.92,
      "tumorType": "Glioblastoma",
      "tumorGrade": 4
    }
  ],
  "summary": {
    "totalAnalyzed": 9,
    "tumorsDetected": 3,
    "detectionRate": 0.33,
    "averageConfidence": 0.87,
    "highRiskCount": 2,
    "gradeDistribution": {
      "1": 0,
      "2": 1,
      "3": 0,
      "4": 2
    },
    "typeDistribution": {
      "Glioblastoma": 2,
      "Meningioma": 1
    }
  },
  "processedAt": "2025-06-21T10:30:00Z"
}
```

### 3. Image Comparison Analysis
**POST** `/analyze/compare`

Compare analysis results between multiple images.

#### Parameters
- `imageFiles` (form-data, required): 2-10 image files for comparison

#### Response
```json
{
  "comparedImages": [
    {
      "fileName": "patient_a_scan.jpg",
      "hasTumor": true,
      "tumorProbability": 0.85,
      "tumorType": "Meningioma",
      "tumorGrade": 2,
      "riskLevel": "Moderate Risk"
    },
    {
      "fileName": "patient_b_scan.jpg",
      "hasTumor": true,
      "tumorProbability": 0.92,
      "tumorType": "Glioblastoma",
      "tumorGrade": 4,
      "riskLevel": "High Risk"
    }
  ],
  "comparisonSummary": {
    "totalImages": 2,
    "tumorsDetected": 2,
    "highestConfidence": 0.92,
    "averageConfidence": 0.885,
    "mostCommonType": "Mixed types",
    "riskDistribution": {
      "Moderate Risk": 1,
      "High Risk": 1
    }
  },
  "comparedAt": "2025-06-21T10:30:00Z"
}
```

### 4. Detailed Analysis Report
**GET** `/report/detailed/{analysisId}`

Get a comprehensive detailed report for a specific analysis.

#### Response
```json
{
  "analysisId": "analysis-123",
  "generatedAt": "2025-06-21T10:30:00Z",
  "reportSections": [
    {
      "title": "Executive Summary",
      "content": "Comprehensive tumor analysis completed with detailed classification results..."
    },
    {
      "title": "Technical Analysis",
      "content": "AI model performance metrics and confidence intervals..."
    },
    {
      "title": "Clinical Recommendations",
      "content": "Evidence-based recommendations for follow-up care..."
    }
  ]
}
```

### 5. Configuration Management
**GET** `/configuration`

Get current classifier configuration.

#### Response
```json
{
  "detectionThreshold": 0.5,
  "classificationThreshold": 0.3,
  "enableDetailedAnalysis": true
}
```

**POST** `/configuration/update`

Update classifier configuration.

#### Request Body
```json
{
  "detectionThreshold": 0.6,
  "classificationThreshold": 0.4,
  "enableDetailedAnalysis": true
}
```

#### Response
```json
{
  "success": true,
  "message": "Configuration updated successfully",
  "currentConfiguration": {
    "detectionThreshold": 0.6,
    "classificationThreshold": 0.4,
    "enableDetailedAnalysis": true
  },
  "updatedAt": "2025-06-21T10:30:00Z"
}
```

### 6. Export Analysis Results
**POST** `/export`

Export analysis results in various formats.

#### Request Body
```json
{
  "analysisIds": ["analysis-1", "analysis-2", "analysis-3"],
  "format": "CSV",
  "includeImages": false
}
```

#### Response
```json
{
  "exportId": "export-456",
  "format": "CSV",
  "status": "Completed",
  "downloadUrl": "/api/enhancedtumoranalysis/download-export/export-456",
  "exportedAt": "2025-06-21T10:30:00Z",
  "recordCount": 3
}
```

## Console Application Enhancements

The console application has been enhanced with the following new features:

### New Menu Options

1. **Enhanced Single Image Analysis** - Advanced analysis with configurable parameters
2. **Batch Image Analysis** - Process multiple images with progress tracking
3. **Advanced Tumor Analysis** - Detailed analysis with custom thresholds
4. **Compare Analysis Results** - Side-by-side comparison of multiple images
5. **Train with Custom Parameters** - Advanced training with validation
6. **Validate Model Performance** - Comprehensive model validation with metrics
7. **Configure Analysis Settings** - Real-time configuration adjustments
8. **Export Analysis Results** - Export results to various formats

### Key Features

#### Enhanced Analysis Capabilities
- Configurable detection and classification thresholds
- Real-time confidence metrics
- Risk assessment calculations
- Clinical recommendations generation
- Detailed reporting with timestamps

#### Batch Processing
- Process up to 50 images simultaneously
- Progress tracking with real-time updates
- Comprehensive batch statistics
- Error handling for individual images
- Exportable results in CSV and text formats

#### Advanced Configuration
- Dynamic threshold adjustment
- Enable/disable detailed analysis
- Real-time settings validation
- Settings persistence across sessions

#### Validation and Testing
- Confusion matrix calculations
- ROC curve analysis
- Precision, recall, and F1-score metrics
- Cross-validation support
- Performance benchmarking

#### Clinical Integration
- Evidence-based recommendations
- Risk stratification
- Treatment priority suggestions
- Follow-up scheduling guidance

## Usage Examples

### cURL Examples

#### Enhanced Single Analysis
```bash
curl -X POST "https://api.example.com/api/enhancedtumoranalysis/analyze/enhanced?detectionThreshold=0.6&enableDetailedAnalysis=true" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "imageFile=@brain_scan.jpg"
```

#### Batch Analysis
```bash
curl -X POST "https://api.example.com/api/enhancedtumoranalysis/analyze/batch" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "imageFiles=@scan1.jpg" \
  -F "imageFiles=@scan2.jpg" \
  -F "imageFiles=@scan3.jpg"
```

#### Update Configuration
```bash
curl -X POST "https://api.example.com/api/enhancedtumoranalysis/configuration/update" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"detectionThreshold": 0.7, "classificationThreshold": 0.4, "enableDetailedAnalysis": true}'
```

## Error Handling

All endpoints return appropriate HTTP status codes:

- `200 OK` - Successful operation
- `400 Bad Request` - Invalid input parameters
- `401 Unauthorized` - Authentication required
- `413 Payload Too Large` - File size exceeds limit
- `422 Unprocessable Entity` - Invalid file format
- `500 Internal Server Error` - Server processing error

Error responses include detailed error messages:

```json
{
  "error": "Invalid file format",
  "details": "Supported formats: JPG, PNG, BMP, TIFF",
  "timestamp": "2025-06-21T10:30:00Z"
}
```

## Rate Limiting

- Single analysis: 60 requests per minute
- Batch analysis: 10 requests per minute
- Configuration updates: 20 requests per minute

## File Size Limits

- Single image: 50MB maximum
- Batch processing: 500MB total maximum
- Export files: 100MB maximum

## Support

For technical support or feature requests, please contact the development team or refer to the project documentation.
