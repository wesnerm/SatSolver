using System.Text;

namespace SatSolver;

public record OrExp(Exp Left, Exp Right) : Exp
{
    public override ExprKind Kind => ExprKind.OR;

    protected internal override void PrettyPrint(StringBuilder b, string indent)
	{
		b.Append("(or ");
		Left.PrettyPrint(b, indent + "    ");
		b.Append("\n").Append(indent).Append("    ");
		Right.PrettyPrint(b, indent + "    ");
		b.Append(")");
	}
}