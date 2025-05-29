using System.Text;

namespace SVGRender.Services
{
    public class LivingHingeService
    {
        public class HingeParameters
        {
            public double Width { get; set; } = 200;
            public double Height { get; set; } = 100;
            public double SlitLength { get; set; } = 15;
            public double SlitWidth { get; set; } = 1;
            public double SlitSpacing { get; set; } = 3;
            public double RowOffset { get; set; } = 8;
            public string MaterialColor { get; set; } = "#8B4513";
            public string CutColor { get; set; } = "#FF0000";
            public bool AlternateRows { get; set; } = true;
            public string HingeType { get; set; } = "Standard"; // Standard, Dense, Sparse, Custom
        }

        private readonly IWebHostEnvironment _environment;

        public LivingHingeService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }        public async Task<string> GenerateLivingHingeAsync(HingeParameters parameters, string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = GenerateDescriptiveFileName(parameters);
            }

            if (!fileName.EndsWith(".svg"))
            {
                fileName += ".svg";
            }

            // Apply preset parameters based on hinge type
            ApplyHingeTypePresets(parameters);

            var svgContent = GenerateSvgContent(parameters);
            var filePath = Path.Combine(_environment.WebRootPath, fileName);

            await File.WriteAllTextAsync(filePath, svgContent);

            return fileName; // Return the filename for loading into the viewer
        }        private void ApplyHingeTypePresets(HingeParameters parameters)
        {
            switch (parameters.HingeType.ToLower())
            {
                case "dense":
                    parameters.SlitLength = 12;
                    parameters.SlitSpacing = 2;
                    parameters.RowOffset = 6;
                    break;
                case "sparse":
                    parameters.SlitLength = 20;
                    parameters.SlitSpacing = 5;
                    parameters.RowOffset = 12;
                    break;
                case "standard":
                default:
                    // Keep the provided parameters
                    break;
            }
        }

        private string GenerateDescriptiveFileName(HingeParameters parameters)
        {
            // Create a descriptive filename based on the hinge parameters
            var dimensions = $"{parameters.Width:F0}x{parameters.Height:F0}";
            var slitInfo = $"slit{parameters.SlitLength:F0}x{parameters.SlitWidth:F1}";
            var spacing = $"sp{parameters.SlitSpacing:F0}";
            var offset = $"off{parameters.RowOffset:F0}";
            var type = parameters.HingeType.ToLower();
            var alternate = parameters.AlternateRows ? "alt" : "reg";
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            
            return $"hinge_{type}_{dimensions}_{slitInfo}_{spacing}_{offset}_{alternate}_{timestamp}";
        }

        private string GenerateSvgContent(HingeParameters parameters)
        {
            var svg = new StringBuilder();

            // SVG header
            svg.AppendLine($"<svg width=\"{parameters.Width}\" height=\"{parameters.Height}\" xmlns=\"http://www.w3.org/2000/svg\">");
            svg.AppendLine("  <defs>");
            svg.AppendLine("    <style>");
            svg.AppendLine("      .material { fill: " + parameters.MaterialColor + "; stroke: #654321; stroke-width: 0.5; }");
            svg.AppendLine("      .cut-line { fill: none; stroke: " + parameters.CutColor + "; stroke-width: " + parameters.SlitWidth + "; }");
            svg.AppendLine("      .hinge-info { font-family: Arial, sans-serif; font-size: 8px; fill: #333; }");
            svg.AppendLine("    </style>");
            svg.AppendLine("  </defs>");

            // Background material
            svg.AppendLine($"  <rect x=\"0\" y=\"0\" width=\"{parameters.Width}\" height=\"{parameters.Height}\" class=\"material\" />");

            // Generate hinge pattern
            GenerateHingePattern(svg, parameters);

            // Add title and parameters info
            svg.AppendLine($"  <text x=\"5\" y=\"{parameters.Height - 25}\" class=\"hinge-info\">Living Hinge - {parameters.HingeType}</text>");
            svg.AppendLine($"  <text x=\"5\" y=\"{parameters.Height - 15}\" class=\"hinge-info\">Slit: {parameters.SlitLength}x{parameters.SlitWidth}, Spacing: {parameters.SlitSpacing}, Offset: {parameters.RowOffset}</text>");
            svg.AppendLine($"  <text x=\"5\" y=\"{parameters.Height - 5}\" class=\"hinge-info\">Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</text>");

            svg.AppendLine("</svg>");

            return svg.ToString();
        }        private void GenerateHingePattern(StringBuilder svg, HingeParameters parameters)
        {
            var currentY = 10.0; // Start with some margin
            var rowIndex = 0;

            while (currentY + parameters.SlitLength < parameters.Height - 30) // Leave space for info text
            {
                var offsetX = parameters.AlternateRows && (rowIndex % 2 == 1) 
                    ? parameters.SlitSpacing + (parameters.SlitLength / 2) 
                    : parameters.SlitSpacing;

                var currentX = offsetX;

                // Generate slits for current row
                while (currentX + parameters.SlitLength < parameters.Width - 5)
                {
                    // Create a slit (vertical line)
                    svg.AppendLine($"  <line x1=\"{currentX:F1}\" y1=\"{currentY:F1}\" x2=\"{currentX + parameters.SlitLength:F1}\" y2=\"{currentY:F1}\" class=\"cut-line\" />");

                    currentX += parameters.SlitLength + parameters.SlitSpacing;
                }

                currentY += parameters.RowOffset;
                rowIndex++;
            }
        }

        public static Dictionary<string, string> GetHingeTypes()
        {
            return new Dictionary<string, string>
            {
                { "Standard", "Standard Living Hinge" },
                { "Dense", "Dense Pattern (More Flexible)" },
                { "Sparse", "Sparse Pattern (Less Flexible)" }
            };
        }

        public static HingeParameters GetDefaultParameters()
        {
            return new HingeParameters();
        }        public List<string> GetGeneratedHinges()
        {
            var hingeFiles = new List<string>();
            var wwwrootPath = _environment.WebRootPath;
            
            if (Directory.Exists(wwwrootPath))
            {
                var svgFiles = Directory.GetFiles(wwwrootPath, "living_hinge_*.svg")
                    .Select(Path.GetFileName)
                    .Where(f => f != null)
                    .Cast<string>()
                    .OrderByDescending(f => f)
                    .ToList();

                hingeFiles.AddRange(svgFiles);
            }

            return hingeFiles;
        }
    }
}
