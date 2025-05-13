using System.Text;

namespace CongruenceClosure;

public class NegExp(Exp left, Exp right) : Exp
{
    public Exp Left => left;

    public Exp Right => right;

    public override ExprKind Kind => ExprKind.NEQ;

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        return obj is NegExp other && left.Equals(other.Left) && right.Equals(other.Right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(left, right);
    }

    protected internal override void PrettyPrint(StringBuilder b, string indent)
    {
        b.Append("(");
        Left.PrettyPrint(b, "");
        b.Append(" != ");
        Right.PrettyPrint(b, "");
        b.Append(")");
    }
}