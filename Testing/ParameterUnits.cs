
using NUnit.Framework;

using FoundryRulesAndUnits.Units;
using FoundryRulesAndUnits.Extensions;

using FoundryCore;

using FoundryMentorModeler.Model;

namespace SVGRender.Test;


[TestFixture]
public class ParameterUnits : TestingRoot
{
    private UnitSystem units { get; set; }

    [SetUp]
    public void Setup()
    {
        units = new UnitSystem();
        units.Apply(UnitSystemType.MKS);
    }


    [Test]
    public void Length_Inches()
    {
        var input = 100;
        var length = new Length(100, "in");

        var result = length.As("cm");
        $"length = {length} {result}".WriteInfo();

        Assert.That(length.Value(), Is.Not.EqualTo(input));
        Assert.That(length.As("in"), Is.EqualTo(input));

    }

    [Test]
    public void Length_Default()
    {
        var input = 100;
        var length = new Length(input, "m");

        $"length = {length}".WriteInfo();

        var result = length.As("cm");

        Assert.That(result, Is.EqualTo(input * 100));

    }

    [Test]
    public void OPResult_setget()
    {

        var input = 100;
        var length = new Length(input, "cm");
        var op = new OPResult("testing");

        op.SetDefaultValue(length);

        var value = op.AsLength();

        $"length = {value}".WriteInfo();


        Assert.That(value.Value(), Is.EqualTo(1.0));

    }

    [Test]
    public void Parameter_setget()
    {

        var input = 100;
        var length = new Length(input, "cm");

        var comp = new KnComponent("part");
        var param = comp.Parameter("Length", input, "cm");

        $"param = {param.GetTreeNodeTitle()}".WriteInfo();

        var answer = param.GetValue().AsLength();

        Assert.That(answer.Value(), Is.EqualTo(length.Value()));

    }


    // [Test]
    // public void Unit_Function()
    // {
    //     var input = "units(100,'cm')";
          
    //     var parser = new ExpressionParser(input);
    //     var formula = parser.ReadFormula();

    //     var data = formula.Decompile();
    //     PrintToConsole(formula);

    //     var context = new KnInstance("context");
    //     formula.Evaluate(context, OPResult.R(), (x) =>
    //     {
    //         Assert.That(x.IsNumberWithUnits(), Is.EqualTo(true));
    //         var value = x.AsNumber();
    //         Assert.That(value, Is.EqualTo(1));
    //     });

    // }
}
