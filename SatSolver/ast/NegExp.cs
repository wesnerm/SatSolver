using System.Text;

namespace SatSolver;
public class NegExp : Exp
{
	private readonly Exp expr;

	internal NegExp(Exp expr)
	{
		if (expr == null)
		{
			throw new System.ArgumentException("expr cannot be null");
		}

		this.expr = expr;
	}

    public virtual Exp Exp => expr;

    public override ExprKind Kind => ExprKind.NEG;

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
		NegExp negExpr = (NegExp) o;
		return expr.Equals(negExpr.expr);
	}

    public override int GetHashCode() => expr.GetHashCode();

    protected internal override void prettyPrint(StringBuilder b, string indent)
	{
		b.Append("(not ");
		expr.prettyPrint(b, indent + "     ");
		b.Append(")");
	}
}