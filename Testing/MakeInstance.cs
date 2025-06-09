
using NUnit.Framework;

using FoundryRulesAndUnits.Units;
using FoundryRulesAndUnits.Extensions;

using FoundryCore;

using FoundryMentorModeler.Model;
using SVGRender.Knowledge;

namespace SVGRender.Test;


[TestFixture]
public class MakeInstanceTest : TestingRoot
{
    private UnitSystem units { get; set; }

    [SetUp]
    public void Setup()
    {
        units = new UnitSystem();
        units.Apply(UnitSystemType.MKS);
    }


    [Test]
    public void LivingHingeConcept()
    {
        var model = new KnModel();
        var concept = new LivingHingeConcept();
        var result = concept.MakeInstance(model, "TestHinge");

        Assert.That(result, Is.InstanceOf<LivingHingeComponent>());
        Assert.That(result.Name, Is.EqualTo("TestHinge"));
    }



}
