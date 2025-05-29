using Microsoft.AspNetCore.Components;

namespace SVGRender.Components
{
    public partial class SvgRenderer : ComponentBase
    {
        [Parameter] public string? FilePath { get; set; }
        [Parameter] public string? ContainerStyle { get; set; } = "";
        [Parameter] public string? CssClass { get; set; } = "";

        [Inject] private IWebHostEnvironment Environment { get; set; } = default!;

        private string? SvgContent { get; set; }
        private string? ErrorMessage { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await LoadSvgFile();
        }

        private async Task LoadSvgFile()
        {
            try
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    ErrorMessage = "No file path provided";
                    SvgContent = null;
                    return;
                }

                // Construct the full path relative to wwwroot
                var fullPath = Path.Combine(Environment.WebRootPath, FilePath.TrimStart('/'));

                if (!File.Exists(fullPath))
                {
                    ErrorMessage = $"SVG file not found: {FilePath}";
                    SvgContent = null;
                    return;
                }

                // Read the SVG content
                var content = await File.ReadAllTextAsync(fullPath);

                // Basic validation to ensure it's an SVG file
                if (!content.TrimStart().StartsWith("<svg", StringComparison.OrdinalIgnoreCase))
                {
                    ErrorMessage = "File does not appear to be a valid SVG";
                    SvgContent = null;
                    return;
                }

                // Apply CSS class if provided
                if (!string.IsNullOrEmpty(CssClass))
                {
                    // Add class to the svg element
                    var svgTagEnd = content.IndexOf('>');
                    if (svgTagEnd > 0)
                    {
                        var beforeClose = content.Substring(0, svgTagEnd);
                        var afterOpen = content.Substring(svgTagEnd);
                        
                        // Check if class attribute already exists
                        if (beforeClose.Contains("class=", StringComparison.OrdinalIgnoreCase))
                        {
                            // Add to existing class
                            content = content.Replace("class=\"", $"class=\"{CssClass} ");
                        }
                        else
                        {
                            // Add new class attribute
                            content = beforeClose + $" class=\"{CssClass}\"" + afterOpen;
                        }
                    }
                }

                SvgContent = content;
                ErrorMessage = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading SVG file: {ex.Message}";
                SvgContent = null;
            }
        }
    }
}
