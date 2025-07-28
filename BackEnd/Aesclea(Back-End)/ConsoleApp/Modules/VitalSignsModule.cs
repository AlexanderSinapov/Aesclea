using Aesclea_Back_End_.ConsoleApp.Modules;
using Aesclea_Back_End_.Models.VitalSigns;
using Aesclea_Back_End_.Services.VitalSigns;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Aesclea_Back_End_.ConsoleApp.Modules
{
    public class VitalSignsModule : IConsoleModule
    {
        private IVitalSignsService? _vitalSignsService;
        private readonly List<VitalSignsReading> _sessionReadings = new();

        public async Task InitializeAsync()
        {
            // In a real application, you would configure DI container
            // For console app, we'll create the service manually
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<VitalSignsService>();
            _vitalSignsService = new VitalSignsService(logger);
            
            await Task.CompletedTask;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                DisplayVitalSignsMenu();
                var choice = global::System.Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await EnterVitalSignsAsync();
                        break;
                    case "2":
                        await AnalyzeVitalSignsAsync();
                        break;
                    case "3":
                        await ViewVitalSignsHistoryAsync();
                        break;
                    case "4":
                        await SimulatePatientMonitoringAsync();
                        break;
                    case "5":
                        await BatchAnalyzeVitalSignsAsync();
                        break;
                    case "6":
                        await ExportVitalSignsDataAsync();
                        break;
                    case "7":
                        DisplayVitalSignsReference();
                        break;
                    case "8":
                        await ClearSessionDataAsync();
                        break;
                    case "9":
                        return; // Return to main menu
                    default:
                        global::System.Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                if (choice != "9")
                {
                    global::System.Console.WriteLine("\nPress Enter to continue...");
                    global::System.Console.ReadLine();
                }
            }
        }

        public async Task<string> GetStatusAsync()
        {
            await Task.CompletedTask;
            return _vitalSignsService != null ? "✅ Ready" : "❌ Not Initialized";
        }

        private void DisplayVitalSignsMenu()
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("         VITAL SIGNS ANALYSIS MODULE");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();
            global::System.Console.WriteLine("Options:");
            global::System.Console.WriteLine();
            global::System.Console.WriteLine("1. 📊 Enter Vital Signs");
            global::System.Console.WriteLine("2. 🔍 Analyze Current Vital Signs");
            global::System.Console.WriteLine("3. 📋 View Vital Signs History");
            global::System.Console.WriteLine("4. 🔄 Simulate Patient Monitoring");
            global::System.Console.WriteLine("5. 📦 Batch Analyze Multiple Readings");
            global::System.Console.WriteLine("6. 💾 Export Vital Signs Data");
            global::System.Console.WriteLine("7. 📚 View Reference Ranges");
            global::System.Console.WriteLine("8. 🗑️  Clear Session Data");
            global::System.Console.WriteLine("9. ⬅️  Return to Main Menu");
            global::System.Console.WriteLine();
            global::System.Console.WriteLine($"Session Readings: {_sessionReadings.Count}");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.Write("Enter your choice (1-9): ");
        }

        private async Task EnterVitalSignsAsync()
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("           ENTER VITAL SIGNS");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            var reading = new VitalSignsReading
            {
                PatientId = GetPatientId(),
                RecordedAt = DateTime.UtcNow
            };

            global::System.Console.WriteLine("Enter vital signs (press Enter to skip):");
            global::System.Console.WriteLine();

            // Blood Pressure
            reading.SystolicBP = GetIntegerInput("Systolic Blood Pressure (mmHg)");
            if (reading.SystolicBP.HasValue)
            {
                reading.DiastolicBP = GetIntegerInput("Diastolic Blood Pressure (mmHg)");
            }

            // Heart Rate
            reading.HeartRate = GetIntegerInput("Heart Rate (bpm)");

            // Temperature
            reading.Temperature = GetDecimalInput("Temperature");
            if (reading.Temperature.HasValue)
            {
                global::System.Console.Write("Temperature unit (C)elsius or (F)ahrenheit [C]: ");
                var tempUnit = global::System.Console.ReadLine()?.ToLower();
                reading.TemperatureUnit = tempUnit == "f" ? TemperatureUnit.Fahrenheit : TemperatureUnit.Celsius;
            }

            // Respiratory Rate
            reading.RespiratoryRate = GetIntegerInput("Respiratory Rate (breaths/min)");

            // Oxygen Saturation
            reading.OxygenSaturation = GetDecimalInput("Oxygen Saturation (%)");

            // Blood Glucose
            reading.BloodGlucose = GetDecimalInput("Blood Glucose");
            if (reading.BloodGlucose.HasValue)
            {
                global::System.Console.Write("Blood glucose unit (M)g/dL or m(M)ol/L [M]: ");
                var glucoseUnit = global::System.Console.ReadLine()?.ToLower();
                reading.BloodGlucoseUnit = glucoseUnit == "mm" ? BloodGlucoseUnit.MmolL : BloodGlucoseUnit.MgDl;
            }

            // Weight and Height
            reading.Weight = GetDecimalInput("Weight");
            if (reading.Weight.HasValue)
            {
                global::System.Console.Write("Weight unit (K)g or (L)bs [K]: ");
                var weightUnit = global::System.Console.ReadLine()?.ToLower();
                reading.WeightUnit = weightUnit == "l" ? WeightUnit.Lbs : WeightUnit.Kg;
            }

            reading.Height = GetDecimalInput("Height");
            if (reading.Height.HasValue)
            {
                global::System.Console.Write("Height unit (C)m, (I)nches, or (F)eet [C]: ");
                var heightUnit = global::System.Console.ReadLine()?.ToLower();
                reading.HeightUnit = heightUnit switch
                {
                    "i" => HeightUnit.Inches,
                    "f" => HeightUnit.Feet,
                    _ => HeightUnit.Cm
                };
            }

            // Pain Level
            reading.PainLevel = GetIntegerInput("Pain Level (0-10 scale)", 0, 10);

            // Notes
            global::System.Console.Write("Additional notes (optional): ");
            reading.Notes = global::System.Console.ReadLine();

            _sessionReadings.Add(reading);

            global::System.Console.WriteLine();
            global::System.Console.WriteLine("✅ Vital signs recorded successfully!");
            global::System.Console.WriteLine($"📊 Session now contains {_sessionReadings.Count} reading(s)");

            // Offer immediate analysis
            global::System.Console.WriteLine();
            global::System.Console.Write("Would you like to analyze these vital signs now? (y/n): ");
            if (global::System.Console.ReadLine()?.ToLower() == "y")
            {
                await AnalyzeSpecificReadingAsync(reading);
            }
        }

        private async Task AnalyzeVitalSignsAsync()
        {
            if (!_sessionReadings.Any())
            {
                global::System.Console.WriteLine("❌ No vital signs data available. Please enter some readings first.");
                return;
            }

            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("         VITAL SIGNS ANALYSIS");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("Available readings:");
            for (int i = 0; i < _sessionReadings.Count; i++)
            {
                var reading = _sessionReadings[i];
                global::System.Console.WriteLine($"{i + 1}. Patient {reading.PatientId} - {reading.RecordedAt:yyyy-MM-dd HH:mm:ss}");
            }

            global::System.Console.WriteLine();
            global::System.Console.Write("Select reading to analyze (1-{0}) or 'a' for all: ", _sessionReadings.Count);
            var choice = global::System.Console.ReadLine();

            if (choice?.ToLower() == "a")
            {
                // Analyze all readings
                for (int i = 0; i < _sessionReadings.Count; i++)
                {
                    global::System.Console.WriteLine($"\n--- Analysis {i + 1} ---");
                    await AnalyzeSpecificReadingAsync(_sessionReadings[i]);
                }
            }
            else if (int.TryParse(choice, out int index) && index >= 1 && index <= _sessionReadings.Count)
            {
                await AnalyzeSpecificReadingAsync(_sessionReadings[index - 1]);
            }
            else
            {
                global::System.Console.WriteLine("Invalid selection.");
            }
        }

        private async Task AnalyzeSpecificReadingAsync(VitalSignsReading reading)
        {
            if (_vitalSignsService == null)
            {
                global::System.Console.WriteLine("❌ Vital signs service not initialized.");
                return;
            }

            try
            {
                var analysis = await _vitalSignsService.AnalyzeVitalSignsAsync(reading);

                global::System.Console.WriteLine();
                global::System.Console.WriteLine("📊 VITAL SIGNS ANALYSIS RESULTS");
                global::System.Console.WriteLine("==========================================");
                global::System.Console.WriteLine();

                // Display reading summary
                global::System.Console.WriteLine("📋 Reading Summary:");
                global::System.Console.WriteLine($"   Patient ID: {reading.PatientId}");
                global::System.Console.WriteLine($"   Recorded: {reading.RecordedAt:yyyy-MM-dd HH:mm:ss}");
                
                if (reading.SystolicBP.HasValue && reading.DiastolicBP.HasValue)
                    global::System.Console.WriteLine($"   Blood Pressure: {reading.BloodPressureFormatted}");
                
                if (reading.HeartRate.HasValue)
                    global::System.Console.WriteLine($"   Heart Rate: {reading.HeartRate} bpm");
                
                if (reading.Temperature.HasValue)
                    global::System.Console.WriteLine($"   Temperature: {reading.Temperature}°{(reading.TemperatureUnit == TemperatureUnit.Celsius ? "C" : "F")}");
                
                if (reading.RespiratoryRate.HasValue)
                    global::System.Console.WriteLine($"   Respiratory Rate: {reading.RespiratoryRate} breaths/min");
                
                if (reading.OxygenSaturation.HasValue)
                    global::System.Console.WriteLine($"   Oxygen Saturation: {reading.OxygenSaturation}%");
                
                if (reading.BMI.HasValue)
                    global::System.Console.WriteLine($"   BMI: {reading.BMI:F1}");

                global::System.Console.WriteLine();

                // Display assessments
                global::System.Console.WriteLine("🔍 Assessment Results:");
                global::System.Console.WriteLine($"   Blood Pressure: {analysis.Assessment.BloodPressure.Description}");
                global::System.Console.WriteLine($"   Heart Rate: {analysis.Assessment.HeartRate.Description}");
                global::System.Console.WriteLine($"   Temperature: {analysis.Assessment.Temperature.Description}");
                global::System.Console.WriteLine($"   Oxygen Saturation: {analysis.Assessment.OxygenSaturation.Description}");
                global::System.Console.WriteLine($"   Respiratory Rate: {analysis.Assessment.RespiratoryRate.Description}");
                global::System.Console.WriteLine($"   BMI: {analysis.Assessment.BMI.Description}");

                global::System.Console.WriteLine();

                // Display overall risk
                global::System.Console.WriteLine($"⚠️  Overall Risk Level: {analysis.Assessment.OverallRisk}");

                // Display alerts
                if (analysis.Alerts.Any())
                {
                    global::System.Console.WriteLine();
                    global::System.Console.WriteLine("🚨 ALERTS:");
                    foreach (var alert in analysis.Alerts)
                    {
                        var icon = alert.Severity switch
                        {
                            AlertSeverity.Critical => "🔴",
                            AlertSeverity.Warning => "🟡",
                            _ => "ℹ️"
                        };
                        
                        global::System.Console.WriteLine($"   {icon} {alert.Parameter}: {alert.Message}");
                        
                        if (alert.RequiresImmediateAttention)
                        {
                            global::System.Console.WriteLine("     ⚡ REQUIRES IMMEDIATE ATTENTION");
                        }
                    }
                }

                // Display recommendations
                if (analysis.Assessment.Recommendations.Any())
                {
                    global::System.Console.WriteLine();
                    global::System.Console.WriteLine("💡 Recommendations:");
                    foreach (var recommendation in analysis.Assessment.Recommendations)
                    {
                        global::System.Console.WriteLine($"   • {recommendation}");
                    }
                }

                global::System.Console.WriteLine();
                global::System.Console.WriteLine("==========================================");
            }
            catch (Exception ex)
            {
                global::System.Console.WriteLine($"❌ Error analyzing vital signs: {ex.Message}");
            }
        }

        private async Task ViewVitalSignsHistoryAsync()
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("         VITAL SIGNS HISTORY");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            if (!_sessionReadings.Any())
            {
                global::System.Console.WriteLine("📭 No vital signs readings in current session.");
                global::System.Console.WriteLine("Use option 1 to enter some readings first.");
                return;
            }

            global::System.Console.WriteLine($"📊 Session History ({_sessionReadings.Count} readings):");
            global::System.Console.WriteLine();

            var groupedByPatient = _sessionReadings.GroupBy(r => r.PatientId).ToList();

            foreach (var patientGroup in groupedByPatient)
            {
                global::System.Console.WriteLine($"👤 Patient {patientGroup.Key}:");
                
                foreach (var reading in patientGroup.OrderBy(r => r.RecordedAt))
                {
                    global::System.Console.WriteLine($"   📅 {reading.RecordedAt:yyyy-MM-dd HH:mm:ss}");
                    
                    if (reading.SystolicBP.HasValue && reading.DiastolicBP.HasValue)
                        global::System.Console.WriteLine($"      🩸 BP: {reading.BloodPressureFormatted}");
                    
                    if (reading.HeartRate.HasValue)
                        global::System.Console.WriteLine($"      ❤️  HR: {reading.HeartRate} bpm");
                    
                    if (reading.Temperature.HasValue)
                        global::System.Console.WriteLine($"      🌡️  Temp: {reading.Temperature}°{(reading.TemperatureUnit == TemperatureUnit.Celsius ? "C" : "F")}");
                    
                    if (reading.OxygenSaturation.HasValue)
                        global::System.Console.WriteLine($"      🫁 O2: {reading.OxygenSaturation}%");
                    
                    if (reading.BMI.HasValue)
                        global::System.Console.WriteLine($"      ⚖️  BMI: {reading.BMI:F1}");
                    
                    if (!string.IsNullOrEmpty(reading.Notes))
                        global::System.Console.WriteLine($"      📝 Notes: {reading.Notes}");
                    
                    global::System.Console.WriteLine();
                }
            }

            await Task.CompletedTask;
        }

        private async Task SimulatePatientMonitoringAsync()
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("         PATIENT MONITORING SIMULATION");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            global::System.Console.Write("Enter Patient ID for monitoring: ");
            if (!int.TryParse(global::System.Console.ReadLine(), out int patientId))
            {
                global::System.Console.WriteLine("Invalid Patient ID.");
                return;
            }

            global::System.Console.Write("Enter monitoring duration in minutes [5]: ");
            var durationInput = global::System.Console.ReadLine();
            if (!int.TryParse(durationInput, out int duration) || duration <= 0)
            {
                duration = 5;
            }

            global::System.Console.Write("Enter reading interval in seconds [30]: ");
            var intervalInput = global::System.Console.ReadLine();
            if (!int.TryParse(intervalInput, out int interval) || interval <= 0)
            {
                interval = 30;
            }

            global::System.Console.WriteLine();
            global::System.Console.WriteLine($"🔄 Starting {duration}-minute monitoring simulation for Patient {patientId}");
            global::System.Console.WriteLine($"📊 Taking readings every {interval} seconds");
            global::System.Console.WriteLine("Press 'q' to quit early...");
            global::System.Console.WriteLine();

            var endTime = DateTime.Now.AddMinutes(duration);
            var readingCount = 0;

            while (DateTime.Now < endTime)
            {
                if (global::System.Console.KeyAvailable)
                {
                    var key = global::System.Console.ReadKey(true);
                    if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                    {
                        global::System.Console.WriteLine("Monitoring stopped by user.");
                        break;
                    }
                }

                readingCount++;
                var simulatedReading = GenerateSimulatedReading(patientId);
                _sessionReadings.Add(simulatedReading);

                global::System.Console.WriteLine($"Reading #{readingCount} - {simulatedReading.RecordedAt:HH:mm:ss}");
                global::System.Console.WriteLine($"   BP: {simulatedReading.BloodPressureFormatted}, HR: {simulatedReading.HeartRate}, Temp: {simulatedReading.Temperature}°C");

                // Quick analysis for critical values
                if (_vitalSignsService != null)
                {
                    var analysis = await _vitalSignsService.AnalyzeVitalSignsAsync(simulatedReading);
                    var criticalAlerts = analysis.Alerts.Where(a => a.Severity == AlertSeverity.Critical).ToList();
                    
                    if (criticalAlerts.Any())
                    {
                        global::System.Console.WriteLine("   🚨 CRITICAL ALERT: " + string.Join(", ", criticalAlerts.Select(a => a.Message)));
                    }
                }

                global::System.Console.WriteLine();

                await Task.Delay(interval * 1000);
            }

            global::System.Console.WriteLine($"✅ Monitoring completed. {readingCount} readings recorded.");
        }

        private VitalSignsReading GenerateSimulatedReading(int patientId)
        {
            var random = new Random();
            
            return new VitalSignsReading
            {
                PatientId = patientId,
                RecordedAt = DateTime.UtcNow,
                SystolicBP = random.Next(110, 140),
                DiastolicBP = random.Next(70, 90),
                HeartRate = random.Next(60, 100),
                Temperature = Math.Round((decimal)(36.0 + random.NextDouble() * 2.0), 1),
                TemperatureUnit = TemperatureUnit.Celsius,
                RespiratoryRate = random.Next(12, 20),
                OxygenSaturation = Math.Round((decimal)(95 + random.NextDouble() * 5), 1),
                Weight = Math.Round((decimal)(60 + random.NextDouble() * 40), 1),
                WeightUnit = WeightUnit.Kg,
                Height = Math.Round((decimal)(160 + random.NextDouble() * 30), 0),
                HeightUnit = HeightUnit.Cm,
                PainLevel = random.Next(0, 4),
                Notes = $"Simulated reading #{random.Next(1000, 9999)}"
            };
        }

        private async Task BatchAnalyzeVitalSignsAsync()
        {
            if (!_sessionReadings.Any())
            {
                global::System.Console.WriteLine("❌ No readings available for batch analysis.");
                return;
            }

            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("         BATCH VITAL SIGNS ANALYSIS");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine($"📊 Analyzing {_sessionReadings.Count} readings...");
            global::System.Console.WriteLine();

            if (_vitalSignsService == null)
            {
                global::System.Console.WriteLine("❌ Vital signs service not initialized.");
                return;
            }

            var analyses = new List<VitalSignsAnalysis>();
            var criticalCount = 0;
            var warningCount = 0;

            foreach (var reading in _sessionReadings)
            {
                try
                {
                    var analysis = await _vitalSignsService.AnalyzeVitalSignsAsync(reading);
                    analyses.Add(analysis);

                    var criticalAlerts = analysis.Alerts.Count(a => a.Severity == AlertSeverity.Critical);
                    var warningAlerts = analysis.Alerts.Count(a => a.Severity == AlertSeverity.Warning);

                    criticalCount += criticalAlerts;
                    warningCount += warningAlerts;
                }
                catch (Exception ex)
                {
                    global::System.Console.WriteLine($"⚠️  Error analyzing reading for Patient {reading.PatientId}: {ex.Message}");
                }
            }

            // Display summary
            global::System.Console.WriteLine("📈 BATCH ANALYSIS SUMMARY");
            global::System.Console.WriteLine("==========================================");
            global::System.Console.WriteLine($"Total readings analyzed: {analyses.Count}");
            global::System.Console.WriteLine($"Critical alerts: {criticalCount}");
            global::System.Console.WriteLine($"Warning alerts: {warningCount}");
            global::System.Console.WriteLine();

            // Risk level distribution
            var riskDistribution = analyses.GroupBy(a => a.Assessment.OverallRisk)
                .ToDictionary(g => g.Key, g => g.Count());

            global::System.Console.WriteLine("Risk Level Distribution:");
            foreach (var risk in riskDistribution)
            {
                var percentage = (double)risk.Value / analyses.Count * 100;
                global::System.Console.WriteLine($"   {risk.Key}: {risk.Value} ({percentage:F1}%)");
            }

            global::System.Console.WriteLine();

            // Show critical readings
            var criticalReadings = analyses.Where(a => a.Assessment.OverallRisk == OverallRiskLevel.CriticalRisk).ToList();
            if (criticalReadings.Any())
            {
                global::System.Console.WriteLine("🚨 CRITICAL RISK READINGS:");
                foreach (var critical in criticalReadings)
                {
                    global::System.Console.WriteLine($"   Patient {critical.Reading.PatientId} - {critical.Reading.RecordedAt:yyyy-MM-dd HH:mm:ss}");
                    foreach (var alert in critical.Alerts.Where(a => a.Severity == AlertSeverity.Critical))
                    {
                        global::System.Console.WriteLine($"     • {alert.Parameter}: {alert.Message}");
                    }
                }
                global::System.Console.WriteLine();
            }

            global::System.Console.WriteLine("✅ Batch analysis completed.");
        }

        private async Task ExportVitalSignsDataAsync()
        {
            if (!_sessionReadings.Any())
            {
                global::System.Console.WriteLine("❌ No data to export.");
                return;
            }

            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("         EXPORT VITAL SIGNS DATA");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            var exportPath = Path.Combine(Environment.CurrentDirectory, "Exports", "VitalSigns");
            Directory.CreateDirectory(exportPath);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var filename = Path.Combine(exportPath, $"vital_signs_session_{timestamp}.json");

            var exportData = new
            {
                ExportDate = DateTime.Now,
                SessionData = new
                {
                    ReadingCount = _sessionReadings.Count,
                    Readings = _sessionReadings.Select(r => new
                    {
                        r.PatientId,
                        r.RecordedAt,
                        BloodPressure = r.BloodPressureFormatted,
                        r.HeartRate,
                        r.Temperature,
                        TemperatureUnit = r.TemperatureUnit.ToString(),
                        r.RespiratoryRate,
                        r.OxygenSaturation,
                        r.BloodGlucose,
                        BloodGlucoseUnit = r.BloodGlucoseUnit.ToString(),
                        r.Weight,
                        WeightUnit = r.WeightUnit.ToString(),
                        r.Height,
                        HeightUnit = r.HeightUnit.ToString(),
                        BMI = r.BMI?.ToString("F1"),
                        r.PainLevel,
                        r.Notes
                    })
                }
            };

            await File.WriteAllTextAsync(filename, System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

            global::System.Console.WriteLine($"✅ Vital signs data exported successfully!");
            global::System.Console.WriteLine($"📁 File location: {filename}");
            global::System.Console.WriteLine($"📊 Records exported: {_sessionReadings.Count}");
        }

        private void DisplayVitalSignsReference()
        {
            global::System.Console.Clear();
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine("         VITAL SIGNS REFERENCE RANGES");
            global::System.Console.WriteLine("==============================================");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("🩸 BLOOD PRESSURE (mmHg):");
            global::System.Console.WriteLine("   Normal: < 120/80");
            global::System.Console.WriteLine("   Elevated: 120-129 / < 80");
            global::System.Console.WriteLine("   High Stage 1: 130-139 / 80-89");
            global::System.Console.WriteLine("   High Stage 2: 140-179 / 90-119");
            global::System.Console.WriteLine("   Hypertensive Crisis: ≥ 180 / ≥ 120");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("❤️  HEART RATE (bpm):");
            global::System.Console.WriteLine("   Bradycardia: < 60");
            global::System.Console.WriteLine("   Normal: 60-100");
            global::System.Console.WriteLine("   Tachycardia: > 100");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("🌡️  TEMPERATURE:");
            global::System.Console.WriteLine("   Normal: 36.1-37.2°C (97-99°F)");
            global::System.Console.WriteLine("   Low-grade fever: 37.3-38.0°C (99.1-100.4°F)");
            global::System.Console.WriteLine("   Fever: 38.1-39.0°C (100.5-102.2°F)");
            global::System.Console.WriteLine("   High fever: > 39.0°C (> 102.2°F)");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("🫁 RESPIRATORY RATE (breaths/min):");
            global::System.Console.WriteLine("   Normal: 12-20");
            global::System.Console.WriteLine("   Bradypnea: < 12");
            global::System.Console.WriteLine("   Tachypnea: > 20");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("💨 OXYGEN SATURATION (%):");
            global::System.Console.WriteLine("   Normal: ≥ 95");
            global::System.Console.WriteLine("   Low: 88-94");
            global::System.Console.WriteLine("   Critical: < 88");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("🍭 BLOOD GLUCOSE (mg/dL):");
            global::System.Console.WriteLine("   Normal: 70-99");
            global::System.Console.WriteLine("   Prediabetes: 100-125");
            global::System.Console.WriteLine("   Diabetes: ≥ 126");
            global::System.Console.WriteLine("   Hypoglycemia: < 70");
            global::System.Console.WriteLine();

            global::System.Console.WriteLine("⚖️  BMI (kg/m²):");
            global::System.Console.WriteLine("   Underweight: < 18.5");
            global::System.Console.WriteLine("   Normal: 18.5-24.9");
            global::System.Console.WriteLine("   Overweight: 25.0-29.9");
            global::System.Console.WriteLine("   Obese Class I: 30.0-34.9");
            global::System.Console.WriteLine("   Obese Class II: 35.0-39.9");
            global::System.Console.WriteLine("   Obese Class III: ≥ 40.0");
        }

        private async Task ClearSessionDataAsync()
        {
            global::System.Console.Write($"Are you sure you want to clear all {_sessionReadings.Count} readings? (y/n): ");
            if (global::System.Console.ReadLine()?.ToLower() == "y")
            {
                _sessionReadings.Clear();
                global::System.Console.WriteLine("✅ Session data cleared successfully.");
            }
            else
            {
                global::System.Console.WriteLine("Operation cancelled.");
            }
            
            await Task.CompletedTask;
        }

        // Helper methods
        private int GetPatientId()
        {
            global::System.Console.Write("Patient ID: ");
            while (true)
            {
                if (int.TryParse(global::System.Console.ReadLine(), out int patientId) && patientId > 0)
                {
                    return patientId;
                }
                global::System.Console.Write("Please enter a valid Patient ID (positive number): ");
            }
        }

        private int? GetIntegerInput(string prompt, int? min = null, int? max = null)
        {
            global::System.Console.Write($"{prompt}: ");
            var input = global::System.Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (int.TryParse(input, out int value))
            {
                if (min.HasValue && value < min.Value)
                {
                    global::System.Console.WriteLine($"Value must be at least {min}");
                    return GetIntegerInput(prompt, min, max);
                }
                
                if (max.HasValue && value > max.Value)
                {
                    global::System.Console.WriteLine($"Value must be at most {max}");
                    return GetIntegerInput(prompt, min, max);
                }
                
                return value;
            }

            global::System.Console.WriteLine("Please enter a valid number or press Enter to skip");
            return GetIntegerInput(prompt, min, max);
        }

        private decimal? GetDecimalInput(string prompt)
        {
            global::System.Console.Write($"{prompt}: ");
            var input = global::System.Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (decimal.TryParse(input, out decimal value) && value >= 0)
            {
                return value;
            }

            global::System.Console.WriteLine("Please enter a valid positive number or press Enter to skip");
            return GetDecimalInput(prompt);
        }
    }
}
