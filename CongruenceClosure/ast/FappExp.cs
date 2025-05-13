using System.Text;

namespace CongruenceClosure;

public class FappExp(long id, List<Exp> exprs) : Exp
{
    public long Id => id;

    public List<Exp> Exprs => exprs;

    private int hashcode = -1;

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
        FappExp fExpr = (FappExp)o;
        if (this.Id != fExpr.Id)
            return false;
        return Exprs.SequenceEqual(fExpr.Exprs);
    }

    public override int GetHashCode()
    {
        if (hashcode != -1)
            return hashcode;

        hashcode = Id.GetHashCode();
        for (int i = 0; i < Exprs.Count; i++)
            hashcode = HashCode.Combine(hashcode, Exprs[i]);
        return hashcode;
    }

    protected internal override void PrettyPrint(StringBuilder b, string indent)
    {
        b.Append("f");
        b.Append(Id);
        b.Append("(");
        if (Exprs.Count > 0)
        {
            b.Append(Exprs[0]);
        }
        if (Exprs.Count > 1)
        {
            for (int i = 1; i < Exprs.Count; i++)
            {
                b.Append(", ");
                Exprs[i].PrettyPrint(b, "");
            }
        }
        b.Append(")");
    }

    public override ExprKind Kind => ExprKind.FAPP;

}