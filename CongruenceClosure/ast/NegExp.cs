using System.Text;

namespace CongruenceClosure;

public class NegExp : Exp
{
	internal Exp leftExpr;
	internal Exp rightExpr;

	internal NegExp(Exp left, Exp right)
	{
		if (left is null)
			throw new System.ArgumentException("Expr left cannot be null");

		if (right is null)
			throw new System.ArgumentException("Expr right cannot be null");

		this.leftExpr = left;
		this.rightExpr = right;
	}

        public virtual Exp left => leftExpr;

        public virtual Exp right => rightExpr;


        public override bool Equals(object? o)
	{
		if (this == o)
			return true;
		if (o == null || this.GetType() != o.GetType())
			return false;
		NegExp neqExpr = (NegExp) o;
		return (leftExpr.Equals(neqExpr.leftExpr) && rightExpr.Equals(neqExpr.rightExpr));
	}

        public override int GetHashCode() => HashCode.Combine(leftExpr, rightExpr);

        protected internal override void prettyPrint(StringBuilder b, string indent)
	{
		b.Append("(");
		leftExpr.prettyPrint(b, "");
		b.Append(" != ");
		rightExpr.prettyPrint(b, "");
		b.Append(")");
	}

        public override ExprKind Kind => ExprKind.NEQ;
    }