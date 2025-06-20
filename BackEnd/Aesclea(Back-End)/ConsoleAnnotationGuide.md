# Console Tumor Annotation Features

## Overview
The console version of Aesclea Medical AI System now includes enhanced tumor annotation capabilities. When a tumor is detected in an image, you can save an annotated version with the tumor outlined in customizable colors.

## How to Use

### 1. Start the Console Application
Run the console application and select option **2** from the main menu:
```
2. Analyze Image with Annotation (Enhanced)
```

### 2. Provide Image Path
Enter the full path to your medical image file:
```
Enter the path to the image file to analyze:
C:\path\to\your\medical_image.jpg
```

### 3. Review Analysis Results
The system will display detailed tumor analysis results including:
- Tumor detection confidence
- Tumor type classification
- Grade and description
- Anatomical location
- Estimated stage

### 4. Choose to Save Annotated Image
If a tumor is detected, you'll be prompted:
```
Tumor detected! Would you like to save an annotated image? (y/n)
```

### 5. Select Outline Color
Choose from several color options:
```
Choose outline color:
1. Auto (based on tumor grade)
2. Red
3. Orange
4. Yellow
5. Green
6. Blue
7. Purple
Enter choice (1-7):
```

#### Auto Color Selection
When you choose "Auto" (option 1), colors are automatically selected based on tumor grade:
- **Yellow**: Grade 1 (Low-grade, well-differentiated)
- **Orange**: Grade 2 (Intermediate-grade, moderately differentiated)
- **Red**: Grade 3 (High-grade, poorly differentiated)
- **Dark Red**: Grade 4 (Highest-grade, undifferentiated/anaplastic)

### 6. Output Files
The system creates two files:

#### Annotated Image
- **Location**: `AnnotatedImages/` folder in your application directory
- **Format**: PNG file for best quality
- **Naming**: `annotated_[originalname]_[timestamp].png`
- **Features**:
  - Bold 6-pixel outline around detected tumor
  - Semi-transparent fill to highlight the region
  - Information box with tumor details
  - High-quality anti-aliased rendering

#### Analysis Summary
- **Location**: Same folder as annotated image
- **Format**: Text file (.txt)
- **Content**: Detailed analysis results including all tumor characteristics

## Example Output

```
========================================
         AESCLEA MEDICAL AI SYSTEM
========================================

Enter the path to the image file to analyze:
C:\medical_images\brain_scan.jpg

TUMOR ANALYSIS RESULTS:
Detection Confidence: 87.32%
Type: Glioma (confidence: 78.45%)
Grade: 3 - Grade 3 - Poorly differentiated (high-grade) (confidence: 82.11%)
Location: Brain & CNS (confidence: 91.23%)
Estimated Stage: 3 - Stage III - Advanced local or regional spread

Tumor detected! Would you like to save an annotated image? (y/n)
y

Choose outline color:
1. Auto (based on tumor grade)
2. Red
3. Orange
4. Yellow
5. Green
6. Blue
7. Purple
Enter choice (1-7): 1

Annotated image saved to: C:\path\to\app\AnnotatedImages\annotated_brain_scan_20250620143022.png
Analysis summary saved to: C:\path\to\app\AnnotatedImages\annotated_brain_scan_20250620143022.txt
```

## Visual Features

### Annotation Elements
1. **Tumor Outline**: Bold colored rectangle around estimated tumor region
2. **Highlight Fill**: Semi-transparent colored fill inside the outline
3. **Information Box**: Professional overlay with tumor details including:
   - Detection confidence percentage
   - Tumor type
   - Grade with description
   - Anatomical location
   - Estimated stage

### Information Box Details
The information box displays:
```
TUMOR DETECTED: 87.3%
Type: Glioma
Grade: 3 (Grade 3 - Poorly differentiated (high-grade))
Location: Brain & CNS
Stage: 3
```

## Technical Notes

### Platform Compatibility
- **Windows**: Full annotation features available
- **Other Platforms**: Basic analysis only (annotation requires Windows-specific graphics libraries)

### File Organization
- Original images: Remain in their original location
- Annotated images: Saved to `AnnotatedImages/` subfolder
- Analysis summaries: Saved alongside annotated images
- Automatic folder creation if directories don't exist

### Quality Settings
- **Image Format**: PNG for lossless quality
- **Graphics**: Anti-aliased rendering for smooth lines
- **Colors**: High contrast for medical imaging clarity
- **Text**: Bold, readable font optimized for medical documentation

## Integration with Existing Features

The new annotation feature complements the existing console options:
- **Option 1**: Basic tumor analysis (original functionality)
- **Option 2**: Enhanced analysis with annotation (new feature)
- Both options use the same AI models and provide identical analysis results
- The difference is in the visualization and saving capabilities

## Error Handling

The system gracefully handles:
- Invalid file paths
- Unsupported image formats
- Platform compatibility issues
- File permission problems
- Missing directories (auto-created)

## Tips for Best Results

1. **Image Quality**: Use high-resolution medical images for better analysis
2. **File Formats**: Supported formats include JPG, PNG, BMP, TIFF
3. **Color Selection**: Use "Auto" for consistent medical documentation
4. **File Management**: Annotated images are timestamped to prevent overwrites
5. **Storage**: Ensure adequate disk space for annotated image files
