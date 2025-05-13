namespace CongruenceClosure;

using System.Runtime.CompilerServices;
using Xunit;

public class CCTests
{
    public string ResourceDir([CallerFilePath] string filePath = "")
    {
        var dir = Path.GetDirectoryName(filePath);
        if (dir == null)
            throw new Exception("Cannot get directory name");
        return Path.Combine(dir, "../resources");
    }

    [Theory]
    [InlineData("(x1=x3),(f1(x1)!=f1(x3))")]
    [InlineData("(f1(x1,x2)=x3),(x1=x4),(x1=x2),(f1(x1,x2)!=f1(x1,x1))")]
    public void UnsatNormalCase(string text)
    {
        CheckUnsat(text);
    }

    [Theory]
    [InlineData(@"sat/sat1.txt")]
    [InlineData(@"sat/sat2.txt")]
    [InlineData(@"sat/sat3.txt")]
    [InlineData(@"sat/sat4.txt")]
    [InlineData(@"sat/sat5.txt")]
    [InlineData(@"sat/sat6.txt")]
    [InlineData(@"sat/sat7.txt")]
    [InlineData(@"sat/sat8.txt")]
    [InlineData(@"sat/sat9.txt")]
    [InlineData(@"sat/sat10.txt")]
    [InlineData(@"sat/sat11.txt")]
    [InlineData(@"sat/sat12.txt")]
    [InlineData(@"sat/sat13.txt")]
    [InlineData(@"sat/sat14.txt")]
    [InlineData(@"sat/sat15.txt")]
    [InlineData(@"sat/sat16.txt")]
    [InlineData(@"sat/sat17.txt")]

    public void Sat(string file)
    {
        var text = File.ReadAllText(Path.Combine(ResourceDir(), file));
        CheckSat(text);
    }

    [Theory]
    [InlineData(@"injective/injective1.txt")]
    [InlineData(@"injective/injective2.txt")]
    [InlineData(@"injective/injective3.txt")]
    [InlineData(@"injective/injective4.txt")]
    [InlineData(@"injective/injective5.txt")]
    [InlineData(@"injective/injective6.txt")]
    [InlineData(@"injective/injective7.txt")]
    [InlineData(@"injective/injective8.txt")]
    [InlineData(@"injective/injective9.txt")]
    [InlineData(@"injective/injective10.txt")]
    [InlineData(@"injective/injective11.txt")]
    [InlineData(@"injective/injective12.txt")]
    [InlineData(@"injective/injective13.txt")]
    [InlineData(@"injective/injective14.txt")]
    [InlineData(@"injective/injective15.txt")]
    [InlineData(@"injective/injective16.txt")]
    [InlineData(@"injective/injective17.txt")]
    public void SatUnsat(string file)
    {
        var text = File.ReadAllText(Path.Combine(ResourceDir(), file));
        CheckSat(text);
        // Unsat if functions are injective
    }

    [Theory]
    [InlineData(@"unsat/unsat1.txt")]
    [InlineData(@"unsat/unsat2.txt")]
    [InlineData(@"unsat/unsat3.txt")]
    [InlineData(@"unsat/unsat4.txt")]
    [InlineData(@"unsat/unsat5.txt")]
    [InlineData(@"unsat/unsat6.txt")]
    [InlineData(@"unsat/unsat7.txt")]
    [InlineData(@"unsat/unsat8.txt")]
    [InlineData(@"unsat/unsat9.txt")]
    [InlineData(@"unsat/unsat10.txt")]
    [InlineData(@"unsat/unsat11.txt")]
    public void Unsat(string file)
    {
        var text = File.ReadAllText(Path.Combine(ResourceDir(), file));
        CheckUnsat(text);
    }

    private void CheckSat(string text)
    {
        var e = ExpParser.Parse(text);
        var sat = ExpUtils.CheckSat(e);
        Assert.True(sat);
    }

    private void CheckUnsat(string text)
    {
        var e = ExpParser.Parse(text);
        var sat = ExpUtils.CheckSat(e);
        Assert.False(sat);
    }

}
