
using NUnit.Framework;

using FoundryRulesAndUnits.Extensions;

using FoundryRulesAndUnits.Models;

namespace SVGRender.Test;

//dotnet test -v n


public class TestingRoot
{
    public void PrintToConsole(ITreeNode root,  int indent = 1)
    {
        $"{root.GetTreeNodeTitle()}".WriteSuccess(indent);
        foreach (var child in root.GetTreeChildren())
            PrintToConsole(child, indent + 1);
    }
}
