using System.Text;

namespace SatSolver;

public record VarExp(long Id) : Exp
{
	public override ExprKind Kind => ExprKind.VAR;
	protected internal override void PrettyPrint(StringBuilder b, string indent)
	{
		b.Append("x").Append(Id);
	}
}