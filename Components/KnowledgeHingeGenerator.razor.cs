using Microsoft.AspNetCore.Components;
using SVGRender.Knowledge;
using SVGRender.Services;

namespace SVGRender.Components
{
    public partial class KnowledgeHingeGenerator : ComponentBase
    {
        [Inject] private KnowledgeHingeService KnowledgeService { get; set; } = default!;
        
        [Parameter] public EventCallback<string> OnFileGenerated { get; set; }

        private LivingHingeComponent? _currentHinge;
        private string _instanceName = "";
        private string _previewSvg = "";
        private List<string> _availablePresets = new();
        private List<string> _validationErrors = new();
        private Timer? _debounceTimer;

        protected override void OnInitialized()
        {
            _availablePresets = KnowledgeService.GetAvailablePresets();
            CreateNewInstance();
        }

        private void CreateNewInstance()
        {
            try
            {
                var name = string.IsNullOrEmpty(_instanceName) ? $"Hinge_{DateTime.Now:HHmmss}" : _instanceName;
                _currentHinge = KnowledgeService.CreateHingeInstance(name);
                
                if (_currentHinge != null)
                {
                    ValidateParameters();
                    GeneratePreview();
                }
                
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating hinge instance: {ex.Message}");
            }
        }

        private void OnPresetChanged(ChangeEventArgs e)
        {
            var presetName = e.Value?.ToString();
            if (!string.IsNullOrEmpty(presetName))
            {
                try
                {
                    var name = string.IsNullOrEmpty(_instanceName) ? $"{presetName}_{DateTime.Now:HHmmss}" : _instanceName;
                    _currentHinge = KnowledgeService.CreatePresetHinge(presetName, name);
                    
                    if (_currentHinge != null)
                    {
                        ValidateParameters();
                        GeneratePreview();
                    }
                    
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating preset hinge: {ex.Message}");
                }
            }
        }

        private void UpdateParameter(string parameterName, object? value)
        {
            if (_currentHinge == null || value == null) return;

            try
            {
                // Convert value to appropriate type
                object convertedValue = value;
                if (value is string strValue && double.TryParse(strValue, out double numValue))
                {
                    convertedValue = numValue;
                }

                _currentHinge.UpdateParameter(parameterName, convertedValue);
                
                // Debounce validation and preview generation
                _debounceTimer?.Dispose();
                _debounceTimer = new Timer((_) =>
                {
                    InvokeAsync(() =>
                    {
                        ValidateParameters();
                        if (!_validationErrors.Any())
                        {
                            GeneratePreview();
                        }
                        StateHasChanged();
                    });
                }, null, 300, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating parameter {parameterName}: {ex.Message}");
            }
        }

        private void ValidateParameters()
        {
            if (_currentHinge == null)
            {
                _validationErrors = new List<string> { "No hinge instance available" };
                return;
            }

            try
            {
                _validationErrors = _currentHinge.ValidateParameters();
            }
            catch (Exception ex)
            {
                _validationErrors = new List<string> { $"Validation error: {ex.Message}" };
            }
        }

        private void GeneratePreview()
        {
            if (_currentHinge == null || _validationErrors.Any())
            {
                _previewSvg = "";
                return;
            }

            try
            {
                _previewSvg = _currentHinge.GeneratePreviewSVG(0.3); // Scale down for preview
            }
            catch (Exception ex)
            {
                _previewSvg = $"<svg><text x='10' y='20' fill='red'>Preview Error: {ex.Message}</text></svg>";
                Console.WriteLine($"Error generating preview: {ex.Message}");
            }
        }

        private async Task GenerateAndSave()
        {
            if (_currentHinge == null || _validationErrors.Any()) return;

            try
            {
                var filename = await _currentHinge.SaveSVGToFile("wwwroot");
                
                // Notify parent component
                if (OnFileGenerated.HasDelegate)
                {
                    await OnFileGenerated.InvokeAsync(filename);
                }

                // Show success message (you might want to inject a toast service)
                Console.WriteLine($"SVG saved as: {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving SVG: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _debounceTimer?.Dispose();
        }
    }
}
