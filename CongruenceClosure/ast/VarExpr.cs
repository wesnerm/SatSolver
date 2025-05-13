using System.Text;

namespace CongruenceClosure;
public class VarExpr : Exp
{
	private readonly long id;

	internal VarExpr(long id)
	{
		if (id <= 0)
		{
			throw new System.ArgumentException("id must be a positive number");
		}
		this.id = id;
	}

        public virtual long Id => id;

        public override ExprKind Kind => ExprKind.VAR;

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
		VarExpr varExpr = (VarExpr) o;
		return id == varExpr.id;
	}

        public override int GetHashCode() => id.GetHashCode();

        protected internal override void prettyPrint(StringBuilder b, string indent)
	{
		b.Append(indent + "x").Append(id);
	}
}