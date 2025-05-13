using System.Text;

namespace SatSolver;

public class AndExp : Exp
{
    private readonly Exp leftExpr;

    private readonly Exp rightExpr;

    internal AndExp(Exp left, Exp right)
    {
        if (left == null)
        {
            throw new System.ArgumentException("Expr left cannot be null");
        }
        if (right == null)
        {
            throw new System.ArgumentException("Expr right cannot be null");
        }

        this.leftExpr = left;
        this.rightExpr = right;
    }

    public virtual Exp Left
    {
        get
        {
            return leftExpr;
        }
    }

    public virtual Exp Right
    {
        get
        {
            return rightExpr;
        }
    }

    public override ExprKind Kind => ExprKind.AND;

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
        AndExp andExpr = (AndExp)o;
        return (leftExpr.Equals(andExpr.leftExpr) && rightExpr.Equals(andExpr.rightExpr));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(leftExpr, rightExpr);
    }

    protected internal override void prettyPrint(StringBuilder b, string indent)
    {
        b.Append("(and ");
        leftExpr.prettyPrint(b, indent + "     ");
        b.Append("\n").Append(indent).Append("     ");
        rightExpr.prettyPrint(b, indent + "     ");
        b.Append(")");
    }

}