using Microsoft.AspNetCore.Components;
using SVGRender.Services;
using SVGRender.Knowledge;
using System.Text;

namespace SVGRender.Components
{
    public partial class KnowledgeHingeGenerator : ComponentBase
    {
        [Inject] private KnowledgeHingeService KnowledgeService { get; set; } = default!;
        [Inject] private LivingHingeService HingeService { get; set; } = default!;
        [Parameter] public EventCallback<string> OnFileGenerated { get; set; }

        protected string SelectedPreset { get; set; } = "";
        protected string InstanceName { get; set; } = "";
        protected LivingHingeComponent? CurrentHingeInstance { get; set; }
        protected string StatusMessage { get; set; } = "";
        protected bool IsCreating { get; set; } = false;
        protected bool IsGenerating { get; set; } = false;
        protected string GeneratedSVGFile { get; set; } = "";
        protected List<string> ValidationErrors { get; set; } = new();
        protected List<string> AvailablePresets { get; set; } = new();

        protected override void OnInitialized()
        {
            AvailablePresets = KnowledgeService.GetAvailablePresets();
        }

        protected void OnPresetChanged(ChangeEventArgs e)
        {
            SelectedPreset = e.Value?.ToString() ?? "";
            ValidationErrors.Clear();
            StatusMessage = string.IsNullOrEmpty(SelectedPreset)
                ? ""
                : $"Selected preset: {SelectedPreset}";
            StateHasChanged();
        }
        
        public string RenderAsJson()
        {
            if (CurrentHingeInstance == null) return "{}";

            try
            {
                // Serialize the current hinge instance to JSON
                
                return CurrentHingeInstance.ToJSON();
            }
            catch (Exception ex)
            {
                ValidationErrors.Add($"Error serializing instance: {ex.Message}");
                return "{}";
            }
        }
        protected void CreateHingeInstance()
        {
            try
            {
                IsCreating = true;
                ValidationErrors.Clear();
                StatusMessage = "Creating hinge instance...";
                StateHasChanged();

                if (string.IsNullOrEmpty(SelectedPreset))
                {
                    ValidationErrors.Add("Please select a preset configuration.");
                    return;
                }

                // Create hinge instance using the knowledge service
                var instanceName = string.IsNullOrEmpty(InstanceName)
                    ? $"hinge_{SelectedPreset}_{DateTime.Now:yyyyMMdd_HHmmss}"
                    : InstanceName;

                CurrentHingeInstance = KnowledgeService.CreatePresetHinge(SelectedPreset, instanceName);

                if (CurrentHingeInstance == null)
                {
                    ValidationErrors.Add("Failed to create hinge instance.");
                    return;
                }



                // Validate the instance
                var validationResults = CurrentHingeInstance.ValidateParameters();
                if (validationResults.Any())
                {
                    ValidationErrors.AddRange(validationResults);
                }

                StatusMessage = $"Successfully created hinge instance: {CurrentHingeInstance.Name}";
            }
            catch (Exception ex)
            {
                ValidationErrors.Add($"Error creating hinge instance: {ex.Message}");
                StatusMessage = "Failed to create hinge instance";
            }
            finally
            {
                IsCreating = false;
                StateHasChanged();
            }
        }

        protected async Task GenerateSVG()
        {
            if (CurrentHingeInstance == null) return;

            try
            {
                IsGenerating = true;
                StatusMessage = "Generating SVG...";
                StateHasChanged();

                // Generate the SVG using the hinge instance
                var svgContent = CurrentHingeInstance.GenerateSVG();
                
                // Save to file
                var fileName = await CurrentHingeInstance.SaveSVGToFile("wwwroot");
                GeneratedSVGFile = fileName;

                StatusMessage = $"Successfully generated SVG: {fileName}";

                // Notify parent component
                if (OnFileGenerated.HasDelegate)
                {
                    await OnFileGenerated.InvokeAsync(fileName);
                }
            }
            catch (Exception ex)
            {
                ValidationErrors.Add($"Error generating SVG: {ex.Message}");
                StatusMessage = "Failed to generate SVG";
            }
            finally
            {
                IsGenerating = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Generates the detailed information about the hinge instance for the pre tag
        /// </summary>
        protected string GetHingeInstanceDetails()
        {
            if (CurrentHingeInstance == null) return "";

            var details = new StringBuilder();

            try
            {
                details.AppendLine("=== KNOWLEDGE-BASED LIVING HINGE INSTANCE ===");
                details.AppendLine($"Instance Name: {CurrentHingeInstance.Name}");
                details.AppendLine($"Preset Configuration: {SelectedPreset}");
                details.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                details.AppendLine();

                details.AppendLine("--- PARAMETERS ---");
                
                // Get parameter summary
                var parameterSummary = CurrentHingeInstance.GetParameters();
                foreach (var param in parameterSummary)
                {
                    details.AppendLine($"{param.Key}: {param.CurrentValue} {param.GetUnits()}");
                }

                details.AppendLine();
                details.AppendLine("--- CALCULATED VALUES ---");
                
                // Get specific calculated parameters
                details.AppendLine($"Length: {CurrentHingeInstance.GetParameterValue("Length"):F2} mm");
                details.AppendLine($"Width: {CurrentHingeInstance.GetParameterValue("Width"):F2} mm");
                details.AppendLine($"Material Thickness: {CurrentHingeInstance.GetParameterValue("MaterialThickness"):F2} mm");
                details.AppendLine($"Slit Length: {CurrentHingeInstance.GetParameterValue("SlitLength"):F2} mm");
                details.AppendLine($"Slit Width: {CurrentHingeInstance.GetParameterValue("SlitWidth"):F2} mm");
                details.AppendLine($"Slit Spacing: {CurrentHingeInstance.GetParameterValue("SlitSpacing"):F2} mm");
                details.AppendLine($"Row Offset: {CurrentHingeInstance.GetParameterValue("RowOffset"):F2} mm");
                details.AppendLine($"Number of Rows: {(int)CurrentHingeInstance.GetParameterValue("NumberOfRows")}");
                details.AppendLine($"Slits per Row: {(int)CurrentHingeInstance.GetParameterValue("SlitsPerRow")}");
                details.AppendLine($"Total Area: {CurrentHingeInstance.GetParameterValue("TotalArea"):F2} mm²");
                details.AppendLine($"Flexibility Factor: {CurrentHingeInstance.GetParameterValue("FlexibilityFactor"):F4}");
                details.AppendLine($"Material Color: {CurrentHingeInstance.GetParameterString("MaterialColor")}");
                details.AppendLine($"Cut Color: {CurrentHingeInstance.GetParameterString("CutColor")}");
                details.AppendLine($"Alternate Rows: {(CurrentHingeInstance.GetParameterValue("AlternateRows") > 0 ? "Yes" : "No")}");

                details.AppendLine();
                details.AppendLine("--- ADVANCED INFORMATION ---");
                details.AppendLine($"Knowledge Model: LivingHingeConcept");
                details.AppendLine($"Component Type: {CurrentHingeInstance.GetType().Name}");
                details.AppendLine($"Preset Applied: {SelectedPreset}");
                
                // Add computational formula information
                details.AppendLine();
                details.AppendLine("--- CALCULATION FORMULAS ---");
                details.AppendLine("NumberOfRows = (Width - 20) / RowOffset");
                details.AppendLine("SlitsPerRow = (Length - 10) / (SlitLength + SlitSpacing)");
                details.AppendLine("TotalArea = Length * Width");
                details.AppendLine("FlexibilityFactor = (SlitLength * SlitsPerRow * NumberOfRows) / TotalArea");

                if (ValidationErrors.Any())
                {
                    details.AppendLine();
                    details.AppendLine("--- VALIDATION WARNINGS ---");
                    foreach (var error in ValidationErrors)
                    {
                        details.AppendLine($"⚠ {error}");
                    }
                }

                return details.ToString();
            }
            catch (Exception ex)
            {
                return $"Error generating instance details: {ex.Message}";
            }
        }
    }
}
