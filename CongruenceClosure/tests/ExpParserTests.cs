namespace CongruenceClosure;

using Xunit;

public class ExpParserTests
{
    [Fact]
    public void EqualTest()
    {
        var exp = ExpParser.Parse(new StringReader("(x1=x2)"));

        Assert.True(exp is [EqExp { Left: VarExp { Id: 1 }, Right: VarExp { Id: 2 } }]);
    }

    [Fact]
    public void NotEqualTest()
    {
        var exp = ExpParser.Parse(new StringReader("(x1!=x2)"));

        Assert.True(exp is [NegExp { Left: VarExp { Id: 1 }, Right: VarExp { Id: 2 } }]);
    }

    [Fact]
    public void ClausesTest()
    {
        var exp = ExpParser.Parse(new StringReader("(x1=x2),(x1!=x2)"));

        Assert.True(exp is [EqExp { Left: VarExp { Id: 1 }, Right: VarExp { Id: 2 } },
                            NegExp { Left: VarExp { Id: 1 }, Right: VarExp { Id: 2 } }]);
    }

    [Fact]
    public void FunctionUnaryTest()
    {
        var exp = ExpParser.Parse(new StringReader("(x1=f1(x2))"));

        Assert.True(exp is [
            EqExp
        {
            Left: VarExp,
            Right: FappExp
            {
                Exprs: [VarExp { Id: 2 }]
            }
        }
         ]);
    }

    [Fact]
    public void FunctionNestedTest()
    {
        var exp = ExpParser.Parse(new StringReader("(x1=f1(f1(x2)))"));

        Assert.True(exp is [
            EqExp
        {
            Left: VarExp,
            Right: FappExp
            {
                Exprs: [FappExp { Exprs: [VarExp { Id: 2 }] }]
            }
        }
         ]);
    }


    [Fact]
    public void FunctionBinaryTest()
    {
        var exp = ExpParser.Parse(new StringReader("(x1=f1(x2,x3))"));

        Assert.True(exp is [
            EqExp 
            { 
                Left: VarExp,
                Right: FappExp { 
                    Exprs: [ VarExp { Id: 2 }, VarExp { Id : 3 } ] 
                }
            }
         ]);
    }


    [Fact]
    public void ExtraFailsWhenSingleTest()
    {
        Assert.Throws<FormatException>(() =>
        {
            var exp = ExpParser.Parse(new StringReader("(x1 != x2) x3"));
        });

        Assert.Throws<FormatException>(() =>
        {
            var exp = ExpParser.Parse(new StringReader("(x1 != x2) x3"), true);
        });
    }

    [Fact]
    public void ExtraSucceedsWhenNotSingleTest()
    {
        var exp = ExpParser.Parse(new StringReader("(x1=x2) x3"), false);

        Assert.True(exp is [EqExp { Left: VarExp { Id: 1 }, Right: VarExp { Id: 2 } }]);
    }
}
