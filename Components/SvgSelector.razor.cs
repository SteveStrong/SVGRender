using Microsoft.AspNetCore.Components;

namespace SVGRender.Components
{
    public partial class SvgSelector : ComponentBase
    {
        private string? SelectedSvgFile { get; set; }
        private string ContainerStyle { get; set; } = "";
        private string CssClass { get; set; } = "";

        private string EffectiveContainerStyle => 
            string.IsNullOrEmpty(ContainerStyle) 
                ? "border: 2px solid #ccc; padding: 20px; border-radius: 8px; background-color: #f8f9fa;" 
                : ContainerStyle;

        protected override void OnInitialized()
        {
            // Set the first SVG as default
            SelectedSvgFile = SvgRenderer.AvailableSvgFiles.Keys.FirstOrDefault();
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
        }
    }
}
