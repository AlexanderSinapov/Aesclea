# Tumor Annotation API Documentation

## Overview
The Tumor Analysis API now provides enhanced features for detecting tumors in medical images and creating annotated images with outlined tumor regions. When a tumor is detected, the system can automatically save an annotated version of the image with the tumor area highlighted in customizable colors.

## Key Features

### 1. Automatic Tumor Detection & Annotation
- Detects tumors in uploaded medical images
- Automatically creates annotated images when tumors are detected
- Provides detailed analysis including tumor type, grade, location, and stage

### 2. Customizable Tumor Outlines
- **Color-coded by Grade**: Automatic color selection based on tumor severity
  - Grade 1 (Low): Yellow outline
  - Grade 2 (Moderate): Orange outline  
  - Grade 3 (High): Red outline
  - Grade 4 (Highest): Dark Red outline
- **Custom Colors**: Choose from predefined colors (Red, Blue, Green, Purple, etc.)
- **Enhanced Visualization**: Semi-transparent fill with bold outline for better visibility

### 3. On-Demand Annotation
- Create annotated images for previously analyzed tumors
- Regenerate annotations with different colors
- Save multiple versions with different color schemes

## API Endpoints

### POST `/api/tumoranalysis/analyze`
Analyzes an uploaded image for tumors and optionally creates an annotated version.

**Parameters:**
- `imageFile` (form-data): The medical image file to analyze
- `saveAnnotated` (query, optional): Whether to automatically save an annotated image (default: true)

**Response:**
```json
{
  "id": "unique-analysis-id",
  "analysisTimestamp": "2025-06-20T10:30:00Z",
  "originalFileName": "scan.jpg",
  "hasTumor": true,
  "canCreateAnnotatedImage": true,
  "annotatedImagePath": "/path/to/annotated_scan.png",
  "annotatedFileName": "annotated_scan.png",
  "tumorProbability": 0.89,
  "tumorType": "Glioma",
  "tumorGrade": 3,
  "gradeDescription": "Grade 3 - Poorly differentiated (high-grade)",
  "tumorLocation": "Brain & CNS",
  "estimatedStage": 3,
  "summary": "Detailed analysis summary..."
}
```

### POST `/api/tumoranalysis/annotate/{analysisId}`
Creates an annotated image for a previously analyzed tumor with custom outline color.

**Parameters:**
- `analysisId` (path): The ID of the tumor analysis
- `outlineColor` (query, optional): Color for the tumor outline (default: "Auto")

**Available Colors:**
- `Auto`: Automatic color based on tumor grade
- `Red`, `Orange`, `Yellow`, `Green`, `Blue`, `Purple`, `DarkRed`, `Pink`

**Response:**
```json
{
  "message": "Annotated image created successfully",
  "annotatedImagePath": "/path/to/annotated_image.png",
  "fileName": "annotated_image.png",
  "downloadUrl": "/api/tumoranalysis/download/annotated_image.png",
  "outlineColor": "Red"
}
```

### GET `/api/tumoranalysis/outline-colors`
Returns available outline colors for tumor annotation.

**Response:**
```json
[
  {
    "value": "Auto",
    "description": "Automatic color based on tumor grade (Yellow=Grade 1, Orange=Grade 2, Red=Grade 3, DarkRed=Grade 4)"
  },
  {
    "value": "Red",
    "description": "Classic red outline"
  }
  // ... more colors
]
```

### GET `/api/tumoranalysis/download/{fileName}`
Downloads an annotated image file.

**Parameters:**
- `fileName` (path): The filename of the annotated image

**Response:** Binary image file

### GET `/api/tumoranalysis/history`
Retrieves the history of all tumor analyses.

**Response:** Array of `TumorAnalysisResponse` objects

## Usage Examples

### 1. Basic Tumor Analysis with Automatic Annotation
```bash
curl -X POST "http://localhost:5000/api/tumoranalysis/analyze" \
  -H "Content-Type: multipart/form-data" \
  -F "imageFile=@brain_scan.jpg"
```

### 2. Analyze Without Saving Annotation
```bash
curl -X POST "http://localhost:5000/api/tumoranalysis/analyze?saveAnnotated=false" \
  -H "Content-Type: multipart/form-data" \
  -F "imageFile=@brain_scan.jpg"
```

### 3. Create Custom Color Annotation
```bash
curl -X POST "http://localhost:5000/api/tumoranalysis/annotate/analysis-id-123?outlineColor=Blue"
```

### 4. Download Annotated Image
```bash
curl -X GET "http://localhost:5000/api/tumoranalysis/download/annotated_brain_scan.png" \
  --output annotated_image.png
```

## Annotation Features

### Visual Elements
- **Bold Outline**: 6-pixel wide border around detected tumor region
- **Semi-transparent Fill**: 60% transparent fill to highlight the area without obscuring details
- **Information Box**: Detailed tumor information displayed with high contrast
- **High Quality**: Anti-aliased rendering for professional medical imaging

### Information Displayed
- Tumor detection confidence percentage
- Tumor type classification
- Grade and description
- Anatomical location
- Estimated stage with description

## File Storage
- **Original Images**: Stored in `uploads/original/` directory
- **Annotated Images**: Stored in `uploads/annotated/` directory
- **Format**: Annotated images are saved as PNG files for best quality
- **Naming**: Annotated files are prefixed with "annotated_" followed by the original filename

## Integration Notes
- All endpoints support CORS for web application integration
- File uploads support common medical image formats: JPG, PNG, BMP, TIFF
- Analysis results are stored in memory for the session (consider implementing persistent storage for production)
- Annotated images can be regenerated with different colors without re-running the AI analysis

## Error Handling
- Invalid file formats return HTTP 400 with error message
- Missing analysis IDs return HTTP 404
- Server errors return HTTP 500 with detailed error information
- Non-tumor images cannot be annotated (logical validation)
