using FoundryMentorModeler.Model;
using SVGRender.Knowledge;


namespace SVGRender.Services
{
    /// <summary>
    /// Service for managing knowledge-based living hinge generation using our simple knowledge system
    /// </summary>
    public class KnowledgeHingeService
    {
        private KnModel _hingeModel;
        private LivingHingeConcept _hingeConcept;

        public KnowledgeHingeService()
        {
            InitializeKnowledgeModel();
        }

        private void InitializeKnowledgeModel()
        {
            try
            {
                // Create the main hinge model
                _hingeModel = new KnModel("LivingHingeModel");
                
                // Create the living hinge concept
                _hingeConcept = new LivingHingeConcept();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize knowledge model: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new living hinge component instance
        /// </summary>
        public LivingHingeComponent CreateHingeInstance(string name = "")
        {
            var instance = _hingeConcept.MakeInstance(_hingeModel, name);
            return (instance as LivingHingeComponent)!;
        }

        /// <summary>
        /// Gets all hinge instances from the model
        /// </summary>
        public List<LivingHingeComponent> GetAllHingeInstances()
        {
            if (_hingeModel == null) return new List<LivingHingeComponent>();

            try
            {
                return _hingeModel.Members<LivingHingeComponent>().ToList();
            }
            catch (Exception)
            {
                return new List<LivingHingeComponent>();
            }
        }

        /// <summary>
        /// Finds a specific hinge instance by name
        /// </summary>
        public LivingHingeComponent? FindHingeInstance(string name)
        {
            if (_hingeModel == null) return null;

            try
            {
                return _hingeModel.Establish<LivingHingeComponent>(name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a preset hinge configuration
        /// </summary>
        public LivingHingeComponent? CreatePresetHinge(string presetName, string instanceName = "")
        {
            var instance = CreateHingeInstance(instanceName);
            if (instance == null) return null;

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
        /// Gets available preset configurations
        /// </summary>
        public List<string> GetAvailablePresets()
        {
            return new List<string>
            {
                "Standard",
                "Dense", 
                "Sparse",
                "Flexible"
            };
        }

        /// <summary>
        /// Computes all values for a hinge instance
        /// </summary>
        public void ComputeAllValues(LivingHingeComponent hingeInstance)
        {
            try
            {
                hingeInstance.ComputeAll<KnComponent>(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to compute all values: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the knowledge model for advanced operations
        /// </summary>
        public KnModel? GetKnowledgeModel() => _hingeModel;

        /// <summary>
        /// Gets the hinge concept for advanced operations
        /// </summary>
        //public LivingHingeConcept? GetHingeConcept() => _hingeConcept;
    }
}
