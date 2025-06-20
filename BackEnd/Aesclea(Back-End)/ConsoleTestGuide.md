# Testing the Console Tumor Annotation Feature

## Quick Start Guide

1. **Build and Run the Application**
   ```bash
   cd "c:\Users\Alexander\source\repos\Aesclea(Back-End)\BackEnd\Aesclea(Back-End)"
   dotnet build
   dotnet run
   ```

2. **Select Server Mode**
   When prompted, choose:
   ```
   Select mode:
   1. Console Mode (AI Training/Testing)  <-- Choose this
   2. Web Server Mode (API Server)
   3. Both Modes (Console + Web Server)
   Enter your choice (1-3): 1
   ```

3. **Use the Enhanced Annotation Feature**
   From the main menu, select:
   ```
   2. Analyze Image with Annotation (Enhanced)
   ```

4. **Provide Image Path**
   Enter the full path to a medical image, for example:
   ```
   Enter the path to the image file to analyze:
   C:\path\to\your\brain_scan.jpg
   ```

5. **Review Results**
   The system will display detailed tumor analysis results.

6. **Save Annotated Image**
   If a tumor is detected, you'll see:
   ```
   Tumor detected! Would you like to save an annotated image? (y/n)
   y
   ```

7. **Choose Color**
   Select your preferred outline color:
   ```
   Choose outline color:
   1. Auto (based on tumor grade)  <-- Recommended
   2. Red
   3. Orange
   4. Yellow
   5. Green
   6. Blue
   7. Purple
   Enter choice (1-7): 1
   ```

8. **Find Your Results**
   The annotated image and analysis summary will be saved to:
   - `AnnotatedImages/annotated_[filename]_[timestamp].png`
   - `AnnotatedImages/annotated_[filename]_[timestamp].txt`

## Expected Output

```
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

Annotated image saved to: C:\...\AnnotatedImages\annotated_brain_scan_20250620143022.png
Analysis summary saved to: C:\...\AnnotatedImages\annotated_brain_scan_20250620143022.txt
```

## Features You'll See

### In the Annotated Image:
- **Bold colored outline** around the detected tumor region
- **Semi-transparent fill** highlighting the tumor area
- **Information box** with tumor details including:
  - Detection confidence percentage
  - Tumor type and classification
  - Grade with description
  - Anatomical location
  - Estimated stage

### Color Coding (Auto Mode):
- **Yellow**: Grade 1 tumors (low-grade, well differentiated)
- **Orange**: Grade 2 tumors (intermediate-grade)
- **Red**: Grade 3 tumors (high-grade, poorly differentiated)
- **Dark Red**: Grade 4 tumors (highest-grade, undifferentiated)

## Troubleshooting

### If you get "not on Windows" error:
- Make sure you're running on Windows (this feature requires Windows for graphics operations)
- Ensure you have Windows 6.1 or later
- The system should now automatically detect Windows correctly

### If no tumor is detected:
- The system will offer to save the analysis summary anyway
- Try with different medical images
- Ensure the image is a supported format (JPG, PNG, BMP, TIFF)

### File permission issues:
- Make sure the application has write permissions in its directory
- The `AnnotatedImages` folder will be created automatically

## Differences from Web Version

| Feature | Console Version | Web Version |
|---------|----------------|-------------|
| Platform Support | Windows Only | Cross-platform |
| User Interface | Text-based menu | Web browser |
| Image Upload | File path input | Drag & drop |
| Color Selection | Numbered menu | Dropdown |
| Download | Automatic save | Manual download |
| Batch Processing | Not available | Coming soon |

The console version is perfect for:
- Quick local analysis
- Batch processing scripts
- Windows-based medical workstations
- Integration with existing medical imaging workflows
