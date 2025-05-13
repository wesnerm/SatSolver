using System.Text;

namespace SatSolver;

public record ImplExp(Exp Antecedent, Exp Consequent) : Exp
{
	public override ExprKind Kind => ExprKind.IMPL;

	protected internal override void PrettyPrint(StringBuilder b, string indent)
	{
		b.Append("(impl ");
		Antecedent.PrettyPrint(b, indent + "      ");
		b.Append("\n").Append(indent).Append("      ");
		Consequent.PrettyPrint(b, indent + "      ");
		b.Append(")");
	}
}