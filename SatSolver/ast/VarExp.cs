using System.Text;

namespace SatSolver;

public class VarExp : Exp
{
	private readonly long id;

	internal VarExp(long id)
	{
		if (id <= 0)
		{
			throw new System.ArgumentException("id must be a positive number");
		}
		this.id = id;
	}

	public virtual long Id
	{
		get
		{
			return id;
		}
	}

	public override ExprKind Kind
	{
		get
		{
			return ExprKind.VAR;
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
		VarExp varExpr = (VarExp) o;
		return id == varExpr.id;
	}

	public override int GetHashCode()
	{
		return id.GetHashCode();
	}

	protected internal override void prettyPrint(StringBuilder b, string indent)
	{
		b.Append("x").Append(id);
	}
}