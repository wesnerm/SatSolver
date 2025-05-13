using System.Text;

namespace SatSolver;

public record EquivExp(Exp Left, Exp Right) : Exp
{
    public override ExprKind Kind => ExprKind.EQUIV;

    protected internal override void PrettyPrint(StringBuilder b, string indent)
    {
        b.Append("(equiv ");
        Left.PrettyPrint(b, indent + "       ");
        b.Append("\n").Append(indent).Append("       ");
        Right.PrettyPrint(b, indent + "       ");
        b.Append(")");
    }
}