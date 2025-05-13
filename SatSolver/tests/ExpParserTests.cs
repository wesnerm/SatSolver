using Xunit;

namespace SatSolver;

public class ExpParserTests
{
    [Fact]
    public void VarTest()
    {
        var exp = ExpParser.Parse(new StringReader("x1"));

        Assert.True(exp is VarExp { Id: 1 });
    }

    [Fact]
    public void NotVarTest()
    {
        Assert.Throws<FormatException>(() =>
        {
            var exp = ExpParser.Parse(new StringReader("y1"));
        });
    }

    [Fact]
    public void NotTest()
    {
        var exp = ExpParser.Parse(new StringReader("(not x1)"));

        Assert.True(exp is NegExp { Exp: VarExp{ Id: 1 } });

    }

    [Fact]
    public void AndTest()
    {
        var exp = ExpParser.Parse(new StringReader("(and x1 x2)"));

        Assert.True(exp is AndExp { Left: VarExp { Id: 1 }, Right: VarExp { Id: 2 } });
    }

    [Fact]
    public void OrTest()
    {
        var exp = ExpParser.Parse(new StringReader("(or x1 x2)"));

        Assert.True(exp is OrExp { Left: VarExp { Id: 1 }, Right: VarExp { Id: 2 } });
    }

    [Fact]
    public void ImplTest()
    {
        var exp = ExpParser.Parse(new StringReader("(impl x1 x2)"));

        Assert.True(exp is ImplExp { Antecedent: VarExp { Id: 1 }, Consequent: VarExp { Id: 2 } });
    }

    [Fact]
    public void EquivTest()
    {
        var exp = ExpParser.Parse(new StringReader("(equiv x1 x2)"));

        Assert.True(exp is EquivExp { Left: VarExp { Id: 1 }, Right: VarExp { Id: 2 } });
    }

    [Fact]
    public void ExtraFailsWhenSingleTest()
    {
        Assert.Throws<FormatException>(() =>
        {
            var exp = ExpParser.Parse(new StringReader("(and x1 x2) x3"));
        });

        Assert.Throws<FormatException>(() =>
        {
            var exp = ExpParser.Parse(new StringReader("(and x1 x2) x3"), true);
        });
    }

    [Fact]
    public void ExtraSucceedsWhenNotSingleTest()
    {
        var exp = ExpParser.Parse(new StringReader("(and x1 x2) x3"), false);

        Assert.True(exp is AndExp { Kind: ExprKind.AND, Left: VarExp { Id: 1 }, Right: VarExp { Id: 2 } });
    }

    [Fact]
    public void NestedTest()
    {
        var exp = ExpParser.Parse(new StringReader("(and (or x1 x2) (or x3 x4))"));

        Assert.True(exp is AndExp { 
            Left: OrExp { Left: VarExp { Id: 1 }, Right: VarExp { Id: 2 } },
            Right: OrExp { Left: VarExp { Id: 3 }, Right: VarExp { Id: 4 } },
        });
    }
}
