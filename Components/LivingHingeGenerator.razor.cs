using Microsoft.AspNetCore.Components;
using SVGRender.Services;

namespace SVGRender.Components
{
    public partial class LivingHingeGenerator : ComponentBase
    {
        [Inject] private LivingHingeService HingeService { get; set; } = default!;
        [Parameter] public EventCallback<string> OnFileGenerated { get; set; }

        protected LivingHingeService.HingeParameters CurrentParameters { get; set; } = new();
        protected string CustomFileName { get; set; } = "";
        protected string LastGeneratedFile { get; set; } = "";
        protected string StatusMessage { get; set; } = "";
        protected bool IsGenerating { get; set; } = false;
        protected List<string> GeneratedFiles { get; set; } = new();
        protected override void OnInitialized()
        {
            CurrentParameters = LivingHingeService.GetDefaultParameters();
            LoadGeneratedFiles();
        }
        private async Task GenerateHinge()
        {
            try
            {
                IsGenerating = true;
                StatusMessage = "Generating living hinge pattern...";
                StateHasChanged();

                var fileName = await HingeService.GenerateLivingHingeAsync(CurrentParameters, CustomFileName);
                
                LastGeneratedFile = fileName;
                StatusMessage = $"Successfully generated: {fileName}";
                
                // Update the available SVG files in the main renderer
                if (OnFileGenerated.HasDelegate)
                {
                    await OnFileGenerated.InvokeAsync(fileName);
                }
                LoadGeneratedFiles();
                CustomFileName = ""; // Clear custom filename after generation
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error generating hinge: {ex.Message}";
            }
            finally
            {
                IsGenerating = false;
                StateHasChanged();
            }
        }

        private async Task LoadIntoViewer()
        {
            if (!string.IsNullOrEmpty(LastGeneratedFile) && OnFileGenerated.HasDelegate)
            {
                await OnFileGenerated.InvokeAsync(LastGeneratedFile);
                StatusMessage = $"Loaded {LastGeneratedFile} into viewer";
            }
        }

        private async Task LoadSpecificFile(string fileName)
        {
            LastGeneratedFile = fileName;
            if (OnFileGenerated.HasDelegate)
            {
                await OnFileGenerated.InvokeAsync(fileName);
            }
            StatusMessage = $"Loaded {fileName} into viewer";
            StateHasChanged();
        }

        private void ResetToDefaults()
        {
            CurrentParameters = LivingHingeService.GetDefaultParameters();
            CustomFileName = "";
            StatusMessage = "Reset to default parameters";
            StateHasChanged();
        }

        private void LoadGeneratedFiles()
        {
            try
            {
                GeneratedFiles = HingeService.GetGeneratedHinges();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading generated files: {ex.Message}";
            }
        }
    }
}
