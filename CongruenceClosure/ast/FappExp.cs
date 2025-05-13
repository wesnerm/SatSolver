using System.Text;

namespace CongruenceClosure;

public class FappExp : Exp
{
	private int hashcode = -1;

	private readonly long id;

	private readonly List<Exp> exprs;

        public virtual List<Exp> Exprs => this.exprs;

        public virtual long Id => id;

        public FappExp(long id, List<Exp> exprs)
	{
		if (id <= 0)
			throw new System.ArgumentException("id must be a positive number");
		this.id = id;
		this.exprs = exprs;
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
		FappExp fExpr = (FappExp) o;
		if (this.id != fExpr.id)
			return false;
		return exprs.SequenceEqual(fExpr.exprs);
	}

	public override int GetHashCode()
	{
		if (hashcode != -1)
			return hashcode;

		hashcode = id.GetHashCode();
		for (int i = 0; i < exprs.Count; i++)
			hashcode = HashCode.Combine(hashcode, exprs[i]);
		return hashcode;
	}

	protected internal override void prettyPrint(StringBuilder b, string indent)
	{
		b.Append("f");
		b.Append(id);
		b.Append("(");
		if (exprs.Count > 0)
		{
			b.Append(exprs[0]);
		}
		if (exprs.Count > 1)
		{
			for (int i = 1; i < exprs.Count; i++)
			{
				b.Append(", ");
				exprs[i].prettyPrint(b, "");
			}
		}
		b.Append(")");
	}

        public override ExprKind Kind => ExprKind.FAPP;

    }