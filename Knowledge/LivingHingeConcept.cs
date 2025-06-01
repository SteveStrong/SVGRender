using System.Text;
using FoundryMentorModeler.Model;

namespace SVGRender.Knowledge
{
    /// <summary>
    /// Knowledge concept that defines the template for living hinges
    /// This class defines the reusable pattern and formulas for creating living hinge instances
    /// </summary>
    public class LivingHingeConcept : KnConcept
    {
        public LivingHingeConcept() : base("LivingHinge")
        {
            //InitializeHingeKnowledge();
        }

        private void InitializeHingeKnowledge()
        {
            // Define calculation formulas for hinge parameters
            // These will automatically create KnVariable objects

            Calculations([
                "Length=100",
                "Width=50",
                "MaterialThickness=3",
                "SlitLength=15",
                "SlitWidth=1.0",
                "SlitSpacing=3",
                "RowOffset=8",
                "NumberOfRows=(Width - 20) / RowOffset",
                "SlitsPerRow=(Length - 10) / (SlitLength + SlitSpacing)",
                "TotalArea=Length * Width",
                "FlexibilityFactor=(SlitLength * SlitsPerRow * NumberOfRows) / TotalArea",
                "MaterialColor='#F5F5DC'",
                "CutColor='#FF0000'",
                "AlternateRows=1"
            ]);

            // Define units for measurements
            Units([
                "Length:mm",
                "Width:mm",
                "MaterialThickness:mm",
                "SlitLength:mm",
                "SlitWidth:mm",
                "SlitSpacing:mm",
                "RowOffset:mm",
                "NumberOfRows:count",
                "SlitsPerRow:count",
                "TotalArea:mm2",
                "FlexibilityFactor:ratio",
                "MaterialColor:color",
                "CutColor:color",
                "AlternateRows:boolean"
            ]);
        }        


        /// <summary>
        /// Override to create our specific component type instead of generic KnInstance
        /// </summary>
        public override KnInstance MakeInstance(KnInstance context, string name = "")
        {
            return new LivingHingeComponent(name);
        }
    }
}
