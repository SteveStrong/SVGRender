using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FoundryMentorModeler.Model;

namespace SVGRender.Knowledge
{
    /// <summary>
    /// Living hinge component that inherits from KnComponent
    /// Contains all methods needed to generate SVG based on calculated parameters
    /// </summary>
    public class LivingHingeComponent : KnComponent
    {
        public LivingHingeComponent(string name) 
            : base(name)
        {
        }

        private static JsonSerializerOptions JsonOptions = new()
        {
            IncludeFields = false,
            IgnoreReadOnlyFields = true,
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };

        public string ToJSON()
        {
            return JsonSerializer.Serialize(this, typeof(LivingHingeComponent), JsonOptions);
        }

        public List<KnParameter> GetParameters()
        {
            return Members<KnParameter>().ToList();
        }

        /// <summary>
        /// Gets the current value of a parameter as a double
        /// </summary>
        public double GetParameterValue(string parameterName)
        {
            var value = GetValueOf(parameterName);
            return value.AsNumber();
        }

        /// <summary>
        /// Gets the current value of a parameter as a string
        /// </summary>
        public string GetParameterString(string parameterName)
        {
            var value = GetValueOf(parameterName);
            return value.AsString();
        }

        /// <summary>
        /// Updates a parameter value and triggers recalculation
        /// </summary>
        public void UpdateParameter(string parameterName, object value)
        {
            SetValueOf(parameterName, value);
        }

        /// <summary>
        /// Generates the complete SVG for the living hinge
        /// </summary>
        public string GenerateSVG()
        {
            // Get calculated parameter values
            var length = GetParameterValue("Length");
            var width = GetParameterValue("Width");
            var slitLength = GetParameterValue("SlitLength");
            var slitWidth = GetParameterValue("SlitWidth");
            var slitSpacing = GetParameterValue("SlitSpacing");
            var rowOffset = GetParameterValue("RowOffset");
            var numberOfRows = (int)GetParameterValue("NumberOfRows");
            var slitsPerRow = (int)GetParameterValue("SlitsPerRow");
            var materialColor = GetParameterString("MaterialColor");
            var cutColor = GetParameterString("CutColor");
            var alternateRows = GetParameterValue("AlternateRows") > 0;

            var svg = new StringBuilder();
            
            // SVG header with proper dimensions
            svg.AppendLine($"<svg width=\"{length + 20}\" height=\"{width + 20}\" xmlns=\"http://www.w3.org/2000/svg\">");
            svg.AppendLine($"  <defs>");
            svg.AppendLine($"    <style>");
            svg.AppendLine($"      .hinge-material {{ fill: {materialColor}; stroke: #333; stroke-width: 0.5; }}");
            svg.AppendLine($"      .hinge-cut {{ stroke: {cutColor}; stroke-width: {slitWidth}; }}");
            svg.AppendLine($"    </style>");
            svg.AppendLine($"  </defs>");
            
            // Draw the base material rectangle
            svg.AppendLine($"  <rect x=\"10\" y=\"10\" width=\"{length}\" height=\"{width}\" class=\"hinge-material\"/>");
            
            // Generate the hinge cuts
            GenerateHingeCuts(svg, length, width, slitLength, slitSpacing, rowOffset, 
                            numberOfRows, slitsPerRow, alternateRows);
            
            // Add labels and dimensions
            GenerateLabels(svg, length, width);
            
            svg.AppendLine("</svg>");
            
            return svg.ToString();
        }

        /// <summary>
        /// Generates the hinge cut patterns based on calculated parameters
        /// </summary>
        private void GenerateHingeCuts(StringBuilder svg, double length, double width, 
            double slitLength, double slitSpacing, double rowOffset,
            int numberOfRows, int slitsPerRow, bool alternateRows)
        {
            var marginTop = (width - (numberOfRows * rowOffset)) / 2;
            var marginLeft = 10;

            for (int row = 0; row < numberOfRows; row++)
            {
                var y = 10 + marginTop + (row * rowOffset);
                var offsetX = alternateRows && (row % 2 == 1) ? slitSpacing / 2 : 0;

                for (int slit = 0; slit < slitsPerRow; slit++)
                {
                    var x = marginLeft + offsetX + (slit * (slitLength + slitSpacing));
                    
                    // Make sure the slit fits within the material bounds
                    if (x + slitLength <= 10 + length)
                    {
                        svg.AppendLine($"  <line x1=\"{x}\" y1=\"{y}\" x2=\"{x + slitLength}\" y2=\"{y}\" class=\"hinge-cut\"/>");
                    }
                }
            }
        }

        /// <summary>
        /// Generates dimension labels and parameter information
        /// </summary>
        private void GenerateLabels(StringBuilder svg, double length, double width)
        {
            // Length dimension
            svg.AppendLine($"  <line x1=\"10\" y1=\"{width + 15}\" x2=\"{10 + length}\" y2=\"{width + 15}\" stroke=\"#666\" stroke-width=\"0.5\"/>");
            svg.AppendLine($"  <text x=\"{10 + length/2}\" y=\"{width + 25}\" text-anchor=\"middle\" font-size=\"8\" fill=\"#666\">{length}mm</text>");
            
            // Width dimension
            svg.AppendLine($"  <line x1=\"5\" y1=\"10\" x2=\"5\" y2=\"{10 + width}\" stroke=\"#666\" stroke-width=\"0.5\"/>");
            svg.AppendLine($"  <text x=\"0\" y=\"{10 + width/2}\" text-anchor=\"middle\" font-size=\"8\" fill=\"#666\" transform=\"rotate(-90, 0, {10 + width/2})\">{width}mm</text>");
        }

        /// <summary>
        /// Generates a preview SVG with smaller dimensions for UI display
        /// </summary>
        public string GeneratePreviewSVG(double scale = 0.5)
        {
            var length = GetParameterValue("Length") * scale;
            var width = GetParameterValue("Width") * scale;
            var slitLength = GetParameterValue("SlitLength") * scale;
            var slitWidth = GetParameterValue("SlitWidth");
            var slitSpacing = GetParameterValue("SlitSpacing") * scale;
            var rowOffset = GetParameterValue("RowOffset") * scale;
            var numberOfRows = (int)GetParameterValue("NumberOfRows");
            var slitsPerRow = (int)GetParameterValue("SlitsPerRow");
            var materialColor = GetParameterString("MaterialColor");
            var cutColor = GetParameterString("CutColor");
            var alternateRows = GetParameterValue("AlternateRows") > 0;

            var svg = new StringBuilder();
            
            svg.AppendLine($"<svg width=\"{length + 20}\" height=\"{width + 20}\" xmlns=\"http://www.w3.org/2000/svg\">");
            svg.AppendLine($"  <rect x=\"10\" y=\"10\" width=\"{length}\" height=\"{width}\" fill=\"{materialColor}\" stroke=\"#333\" stroke-width=\"0.5\"/>");
            
            // Simplified cut generation for preview
            var marginTop = (width - (numberOfRows * rowOffset)) / 2;
            for (int row = 0; row < numberOfRows; row++)
            {
                var y = 10 + marginTop + (row * rowOffset);
                var offsetX = alternateRows && (row % 2 == 1) ? slitSpacing / 2 : 0;

                for (int slit = 0; slit < slitsPerRow; slit++)
                {
                    var x = 10 + offsetX + (slit * (slitLength + slitSpacing));
                    if (x + slitLength <= 10 + length)
                    {
                        svg.AppendLine($"  <line x1=\"{x}\" y1=\"{y}\" x2=\"{x + slitLength}\" y2=\"{y}\" stroke=\"{cutColor}\" stroke-width=\"{slitWidth}\"/>");
                    }
                }
            }
            
            svg.AppendLine("</svg>");
            return svg.ToString();
        }

       public LivingHingeComponent? CreatePresetHinge(string presetName)
        {
            var instance = this;

            try
            {
                switch (presetName.ToLower())
                {
                    case "standard":
                        instance.UpdateParameter("Length", 100.0);
                        instance.UpdateParameter("Width", 50.0);
                        instance.UpdateParameter("SlitLength", 15.0);
                        instance.UpdateParameter("SlitSpacing", 3.0);
                        instance.UpdateParameter("RowOffset", 8.0);
                        break;

                    case "dense":
                        instance.UpdateParameter("Length", 80.0);
                        instance.UpdateParameter("Width", 40.0);
                        instance.UpdateParameter("SlitLength", 12.0);
                        instance.UpdateParameter("SlitSpacing", 2.0);
                        instance.UpdateParameter("RowOffset", 6.0);
                        break;

                    case "sparse":
                        instance.UpdateParameter("Length", 150.0);
                        instance.UpdateParameter("Width", 75.0);
                        instance.UpdateParameter("SlitLength", 20.0);
                        instance.UpdateParameter("SlitSpacing", 5.0);
                        instance.UpdateParameter("RowOffset", 12.0);
                        break;

                    case "flexible":
                        instance.UpdateParameter("Length", 120.0);
                        instance.UpdateParameter("Width", 60.0);
                        instance.UpdateParameter("SlitLength", 25.0);
                        instance.UpdateParameter("SlitSpacing", 2.5);
                        instance.UpdateParameter("RowOffset", 7.0);
                        break;

                    default:
                        // Keep default values from concept
                        break;
                }

                return instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create preset hinge '{presetName}': {ex.Message}");
                return instance; // Return with default values
            }
        }



        /// <summary>
        /// Validates that all parameters have valid values
        /// </summary>
        public List<string> ValidateParameters()
        {
            var errors = new List<string>();

            if (GetParameterValue("Length") <= 0)
                errors.Add("Length must be greater than 0");
            
            if (GetParameterValue("Width") <= 0)
                errors.Add("Width must be greater than 0");
            
            if (GetParameterValue("SlitLength") <= 0)
                errors.Add("Slit length must be greater than 0");
            
            if (GetParameterValue("SlitSpacing") <= 0)
                errors.Add("Slit spacing must be greater than 0");
            
            if (GetParameterValue("RowOffset") <= 0)
                errors.Add("Row offset must be greater than 0");

            var numberOfRows = GetParameterValue("NumberOfRows");
            if (numberOfRows < 1)
                errors.Add("Must have at least 1 row of cuts");

            var slitsPerRow = GetParameterValue("SlitsPerRow");
            if (slitsPerRow < 1)
                errors.Add("Must have at least 1 slit per row");

            return errors;
        }

        /// <summary>
        /// Saves the generated SVG to a file
        /// </summary>
        public async Task<string> SaveSVGToFile(string basePath = "wwwroot")
        {
            var svg = GenerateSVG();
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var filename = $"hinge_{Name}_{timestamp}.svg";
            var filepath = Path.Combine(basePath, filename);
            
            await File.WriteAllTextAsync(filepath, svg);
            return filename;
        }
    }
}
