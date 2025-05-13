using System.Text;

namespace CongruenceClosure;

public class EqExp : Exp
{
    internal Exp leftExpr;
    internal Exp rightExpr;

    internal EqExp(Exp left, Exp right)
    {
        if (left==null)
            throw new System.ArgumentException("Expr left cannot be null");

        if (right==null)
            throw new System.ArgumentException("Expr right cannot be null");

        this.leftExpr = left;
        this.rightExpr = right;
    }

    public Exp left => leftExpr;

    public Exp right => rightExpr;

    public override ExprKind Kind => ExprKind.EQ;

    public override bool Equals(object? o)
    {
        if (this == o)
        {
            return true;
        }
        if (o == null || this.GetType() != o.GetType())
        {
            return false;
        }
        EqExp eqExpr = (EqExp)o;
        return (leftExpr.Equals(eqExpr.leftExpr) && rightExpr.Equals(eqExpr.rightExpr));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(leftExpr, rightExpr);
    }

    protected internal override void prettyPrint(StringBuilder b, string indent)
    {
        b.Append("(");
        leftExpr.prettyPrint(b, "");
        b.Append(" = ");
        rightExpr.prettyPrint(b, "");
        b.Append(")");
    }
}