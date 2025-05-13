using System.Text;

namespace SatSolver;

public record AndExp(Exp Left, Exp Right) : Exp
{
    public override ExprKind Kind => ExprKind.AND;

    protected internal override void PrettyPrint(StringBuilder b, string indent)
    {
        b.Append("(and ");
        Left.PrettyPrint(b, indent + "     ");
        b.Append("\n").Append(indent).Append("     ");
        Right.PrettyPrint(b, indent + "     ");
        b.Append(")");
    }

}