using System.Text;

namespace SatSolver;
public record NegExp(Exp Expr) : Exp
{
    public override ExprKind Kind => ExprKind.NEG;

    protected internal override void PrettyPrint(StringBuilder b, string indent)
	{
		b.Append("(not ");
		Expr.PrettyPrint(b, indent + "     ");
		b.Append(")");
	}
}