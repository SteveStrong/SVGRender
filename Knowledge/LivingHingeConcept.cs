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
            InitializeHingeKnowledge();
        }

        private void InitializeHingeKnowledge()
        {
            // Define calculation formulas for hinge parameters
            // These will automatically create KnVariable objects            
            Calculations([
                "Length|mm: 100",
                "Width|mm: 50",
                "MaterialThickness|mm: 3",
                "SlitLength|mm: 15",
                "SlitWidth|mm: 1.0",
                "SlitSpacing|mm: 3",
                "RowOffset|mm: 8",
                "NumberOfRows: (Width - 20) / RowOffset",
                "SlitsPerRow: (Length - 10) / (SlitLength + SlitSpacing)",
                "TotalArea|mm2: Length * Width",
                "FlexibilityFactor: (SlitLength * SlitsPerRow * NumberOfRows) / TotalArea",
                "MaterialColor|color: '#F5F5DC' ",
                "CutColor|color: '#FF0000'",
                "AlternateRows: 1",
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
