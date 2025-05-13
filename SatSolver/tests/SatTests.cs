namespace SatSolver;

using static ExpFactory;
using Xunit;
using Xunit.Abstractions;
using System.Runtime.CompilerServices;

public class SatTests
{
    private readonly ITestOutputHelper output;

    public static string GetResourceDir([CallerFilePath] string filePath = "") =>
        Path.GetDirectoryName(filePath) + "/../resources/" ?? throw new InvalidOperationException("Unable to get directory name");

    string ResourceDir = GetResourceDir();

    public SatTests(ITestOutputHelper output)
    {
        this.output = output;
    }
    
    [Theory]
    [InlineData(@"sat1.txt")]
    [InlineData(@"sat2.txt")]
    [InlineData(@"sat3.txt")]
    [InlineData(@"sat4.txt")]
    [InlineData(@"sat5.txt")]
    [InlineData(@"sat6.txt")]
    [InlineData(@"sat7.txt")]
    [InlineData(@"sat8.txt")]
    [InlineData(@"sat9.txt")]
    [InlineData(@"sat10.txt")]
    [InlineData(@"sat11.txt")]
    [InlineData(@"sat12.txt")]
    [InlineData(@"sat13.txt")]
    [InlineData(@"sat14.txt")]
    [InlineData(@"sat15.txt")]
    [InlineData(@"sat16.txt")]
    [InlineData(@"sat17.txt")]
    [InlineData(@"sat18.txt")]
    [InlineData(@"sat19.txt")]
    [InlineData(@"ex1.txt")]
    [InlineData(@"ex2.txt")]
    [InlineData(@"ex3.txt")]
    [InlineData(@"ex4.txt")]
    [InlineData(@"ex5.txt")]
    [InlineData(@"ex6.txt")]
    public void SatFast(string test)
    {
        Sat(test);
    }

    [Theory]
    [InlineData(@"satslow1.txt")]
    [InlineData(@"satslow2.txt")]
    [InlineData(@"satslow3.txt")]
    [InlineData(@"satslow4.txt")]
    [InlineData(@"satslow5.txt")]
    [InlineData(@"satslow6.txt")]
    [InlineData(@"satslow7.txt")]
    [InlineData(@"satslow8.txt")]
    [InlineData(@"satslow9.txt")]
    [InlineData(@"satslow10.txt")]
    public void SatSlow(string test)
    {
        Sat(test);
    }

    private void Sat(string test)
    {
        string path = ResourceDir + @"\sat\" + test;
        using TextReader file = File.OpenText(path);

        //var sat = SatUtil.checkSAT(file);

        Exp e = ExpParser.Parse(file);
        Exp cnfExpr = ExpUtils.toTseitin(e);
        var assignments = SatUtil.FindSatisfyingAssignment(cnfExpr);
        Assert.NotNull(assignments);

        var sat = SatUtil.Evaluate(cnfExpr, assignments);
        Assert.True(sat);
    }

    [Theory]
    [InlineData(@"unsat1.txt")]
    [InlineData(@"unsat2.txt")]
    [InlineData(@"unsat3.txt")]
    [InlineData(@"unsat4.txt")]
    [InlineData(@"unsat5.txt")]
    [InlineData(@"unsat6.txt")]
    [InlineData(@"unsat7.txt")]
    [InlineData(@"unsat8.txt")]
    [InlineData(@"unsat9.txt")]
    [InlineData(@"unsat10.txt")]
    [InlineData(@"unsat11.txt")]
    [InlineData(@"unsat12.txt")]
    public void UnsatFast(string test)
    {
        Unsat(test);
    }

    [Theory]
    [InlineData(@"unsatslow1.txt")]
    [InlineData(@"unsatslow2.txt")]
    [InlineData(@"unsatslow3.txt")]
    [InlineData(@"unsatslow4.txt")]
    public void UnsatSlow(string test)
    {
        Unsat(test);
    }

    private void Unsat(string test)
    {
        string path = ResourceDir + @"\unsat\" + test;
        using TextReader file = File.OpenText(path);
        var sat = SatUtil.CheckSat(file);
        Assert.False(sat);
    }

    [Theory]
    [InlineData("x1")]
    [InlineData("(not x1)")]
    [InlineData("(and x1 x1)")]
    [InlineData("(and (not x1) (not x2))")]
    [InlineData("(or x1 x1)")]
    [InlineData("(impl x1 x1)")]
    [InlineData("(equiv x1 x1)")]
    [InlineData("(and x1 x2)")]
    [InlineData("(or x1 x2)")]
    [InlineData("(impl x1 x2)")]
    [InlineData("(equiv x1 x2)")]
    [InlineData( "(not (equiv (and (or x1 x2) (or x3 (not x2)) ) (or x1 x2) ))" )]
    public void BasicSat(string text)
    {
        var file = new StringReader(text);

        Exp e = ExpParser.Parse(file);
        Exp cnfExpr = ExpUtils.toTseitin(e);
        var assignments = SatUtil.FindSatisfyingAssignment(cnfExpr);
        Assert.NotNull(assignments);

        var sat = SatUtil.Evaluate(cnfExpr, assignments);
        Assert.True(sat);
    }

    [Theory]
    [InlineData("(and x1 (not x1))")]
    [InlineData("(equiv x1 (not x1))")]
    public void BasicUnsat(string text)
    {
        var file = new StringReader(text);
        var sat = SatUtil.CheckSat(file);
        Assert.False(sat);
    }


    [Fact]
    public void CheckSmallSat()
    {
        string path = ResourceDir + @"\sat\satsmall.txt";
        using TextReader file = File.OpenText(path);

        Exp e = ExpParser.Parse(file);
        Exp cnfExpr = ExpUtils.toTseitin(e);

        var solve = SatSolve(6, e);
        // var solve = SatSolve(21, cnfExpr); 
        if (solve != null)
        {
            output.WriteLine("Solved");
            output.WriteLine(string.Join(" ", solve.OrderBy(x => x.Key).Select(x => $"x{x.Key}={x.Value.ToString()[0]}")));
        }

        // All false, All false but x1, All true but x3
        var assignments = SatUtil.FindSatisfyingAssignment(cnfExpr);
        // var assignments = Enumerable.Range(1, 6).ToDictionary(x => (long)x, x => false);
        Assert.NotNull(assignments);

        var sat = SatUtil.Evaluate(e, assignments);
        Assert.True(sat);

        sat = SatUtil.Evaluate(cnfExpr, assignments);
        Assert.True(sat);
    }

    [Fact]
    public void CheckRandomStatement()
    {
        Exp a1 = VAR(1), a2 = VAR(2), a3 = VAR(3);
        Exp b1 = VAR(4), b2 = VAR(5), b3 = VAR(6);
        Exp c1 = VAR(7), c2 = VAR(8), c3 = VAR(9);

        var p = ~a1 & ~((a2 & c1) | (a2 & c3) | (a3 & c2)) & ~((c1 & b2) | (c1 & b3) | (c2 & b3));
        var c = (a1 | a2 | a3) & (b1 | b2 | b3) & (c1 | c2 | c3);

        var solve = SatSolve(9, p & c);

        if (solve != null)
        {
            output.WriteLine("Solved");
            output.WriteLine(string.Join(" ", solve.OrderBy(x=>x.Key).Select(x => $"x{x.Key}={x.Value.ToString()[0]}")));
        }
    }

    public Dictionary<long, bool>? SatSolve(int vars, Exp exp, Dictionary<long, bool>? dict = null)
    {
        if (dict == null)
            dict = new Dictionary<long, bool>();

        if (vars == 0)
            return SatUtil.Evaluate(exp, dict) ? dict : null;

        dict[vars] = true;
        var result = SatSolve(vars - 1, exp, dict);
        if (result != null)
            return result;

        dict[vars] = false;
        return SatSolve(vars - 1, exp, dict);
    }

}
