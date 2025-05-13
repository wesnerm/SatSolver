using System.Text;

namespace CongruenceClosure;

public class EqExp(Exp left, Exp right) : Exp
{
    public Exp Left => left;

    public Exp Right => right;

    public override ExprKind Kind => ExprKind.EQ;

    public override bool Equals(object? obj)
    {
        if (obj is not EqExp other)
            return false;
        return left.Equals(other.Left) && right.Equals(other.Right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(left, right);
    }

    protected internal override void PrettyPrint(StringBuilder b, string indent)
    {
        b.Append("(");
        Left.PrettyPrint(b, "");
        b.Append(" = ");
        Right.PrettyPrint(b, "");
        b.Append(")");
    }
}