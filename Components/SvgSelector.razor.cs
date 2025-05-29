using Microsoft.AspNetCore.Components;
using SVGRender.Services;

namespace SVGRender.Components
{
    public partial class SvgSelector : ComponentBase    {
        [Inject] private LivingHingeService HingeService { get; set; } = default!;
        
        private string? SelectedSvgFile { get; set; }
        private string ContainerStyle { get; set; } = "";
        private string CssClass { get; set; } = "";
        private Dictionary<string, string> AllAvailableFiles { get; set; } = new();

        private string EffectiveContainerStyle => 
            string.IsNullOrEmpty(ContainerStyle) 
                ? "border: 2px solid #ccc; padding: 20px; border-radius: 8px; background-color: #f8f9fa;" 
                : ContainerStyle;

        protected override async Task OnInitializedAsync()
        {
            await RefreshAvailableFiles();
            // Set the first SVG as default
            SelectedSvgFile = AllAvailableFiles.Keys.FirstOrDefault();        }        private Task RefreshAvailableFiles()
        {
            // Start with the base SVG files
            AllAvailableFiles = new Dictionary<string, string>(SvgRenderer.AvailableSvgFiles);
            
            // Add generated living hinge files
            var generatedFiles = HingeService.GetGeneratedHinges();
            foreach (var file in generatedFiles)
            {
                var displayName = ParseHingeFileName(file);
                AllAvailableFiles[file] = displayName;
            }
            
            return Task.CompletedTask;
        }

        private string ParseHingeFileName(string fileName)
        {
            // Parse the new descriptive filename format: hinge_type_dimensions_slit_spacing_offset_alternate_timestamp
            if (fileName.StartsWith("hinge_"))
            {
                var parts = fileName.Replace(".svg", "").Split('_');
                if (parts.Length >= 7)
                {
                    var type = parts[1];
                    var dimensions = parts[2];
                    var slitInfo = parts[3];
                    var spacing = parts[4];
                    var offset = parts[5];
                    var alternate = parts[6];
                    
                    return $"Living Hinge: {type} ({dimensions}) - {slitInfo}, {spacing}, {offset}, {alternate}";
                }
            }
            
            // Fallback for old format or unrecognized format
            if (fileName.StartsWith("living_hinge_"))
            {
                var displayName = fileName.Replace("living_hinge_", "").Replace(".svg", "").Replace("_", " ");
                return $"Generated Hinge: {displayName}";
            }
            
            return fileName.Replace(".svg", "");
        }

        private void OnSvgSelectionChanged(ChangeEventArgs e)
        {
            SelectedSvgFile = e.Value?.ToString();
            StateHasChanged();
        }

        private void SelectSvg(string svgFile)
        {
            SelectedSvgFile = svgFile;
            StateHasChanged();
        }        public async Task OnHingeGenerated(string fileName)
        {
            await RefreshAvailableFiles();
            SelectedSvgFile = fileName;
            StateHasChanged();
        }

        private string GetDownloadFileName()
        {
            if (string.IsNullOrEmpty(SelectedSvgFile))
                return "download.svg";
            
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(SelectedSvgFile);
            return $"{fileNameWithoutExt}_{DateTime.Now:yyyyMMdd_HHmmss}.svg";
        }
    }
}
