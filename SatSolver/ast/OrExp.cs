using System.Text;

namespace SatSolver;

public class OrExp : Exp
{
	private readonly Exp leftExpr;

	private readonly Exp rightExpr;

	internal OrExp(Exp left, Exp right)
	{
		if (left==null)
		{
			throw new System.ArgumentException("Expr left cannot be null");
		}
		if (right==null)
		{
			throw new System.ArgumentException("Expr right cannot be null");
		}

		this.leftExpr = left;
		this.rightExpr = right;
	}

    public virtual Exp Left => leftExpr;

    public virtual Exp Right => rightExpr;

    public override ExprKind Kind
	{
		get
		{
			return ExprKind.OR;
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
		OrExp orExpr = (OrExp) o;
		return (leftExpr.Equals(orExpr.leftExpr) && rightExpr.Equals(orExpr.rightExpr));
	}

    public override int GetHashCode() => HashCode.Combine(leftExpr, rightExpr);

    protected internal override void prettyPrint(StringBuilder b, string indent)
	{
		b.Append("(or ");
		leftExpr.prettyPrint(b, indent + "    ");
		b.Append("\n").Append(indent).Append("    ");
		rightExpr.prettyPrint(b, indent + "    ");
		b.Append(")");
	}
}