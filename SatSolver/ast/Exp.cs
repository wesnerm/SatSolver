using System.Text;

namespace SatSolver;

public abstract partial class Exp
{

    protected internal abstract void prettyPrint(StringBuilder b, string indent);

    public abstract ExprKind Kind { get; }

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        prettyPrint(b, "");
        return b.ToString();
    }


    public static Exp operator ~(Exp exp1) => exp1 is NegExp n ? n.Exp : ExpFactory.NEG(exp1);
    public static Exp operator &(Exp exp1, Exp exp2) => ExpFactory.AND(exp1, exp2);
    public static Exp operator |(Exp exp1, Exp exp2) => ExpFactory.OR(exp1, exp2);
    public static Exp operator ^(Exp exp1, Exp exp2) => ~ExpFactory.EQUIV(exp1, exp2);
    public Exp Implies(Exp exp2) => ExpFactory.IMPL(this, exp2);
    public Exp Iff(Exp exp2) => ExpFactory.EQUIV(this, exp2);
}