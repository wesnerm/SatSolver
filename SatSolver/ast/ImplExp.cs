using System.Text;

namespace SatSolver;

public class ImplExp : Exp
{
	private readonly Exp antecedent;

	private readonly Exp consequent;

	internal ImplExp(Exp antecedent, Exp consequent)
	{
		if (antecedent == null)
		{
			throw new System.ArgumentException("antecedent expr cannot be null");
		}
		if (consequent == null)
		{
			throw new System.ArgumentException("consequent expr cannot be null");
		}

		this.antecedent = antecedent;
		this.consequent = consequent;
	}

	public virtual Exp Antecedent
	{
		get
		{
			return antecedent;
		}
	}

	public virtual Exp Consequent
	{
		get
		{
			return consequent;
		}
	}

	public override ExprKind Kind
	{
		get
		{
			return ExprKind.IMPL;
		}
	}

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
		ImplExp implExpr = (ImplExp) o;
		return antecedent.Equals(implExpr.antecedent) && consequent.Equals(implExpr.consequent);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(antecedent, consequent);
	}

	protected internal override void prettyPrint(StringBuilder b, string indent)
	{
		b.Append("(impl ");
		antecedent.prettyPrint(b, indent + "      ");
		b.Append("\n").Append(indent).Append("      ");
		consequent.prettyPrint(b, indent + "      ");
		b.Append(")");
	}
}