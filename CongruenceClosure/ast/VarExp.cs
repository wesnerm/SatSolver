using System.Text;

namespace CongruenceClosure;
public class VarExp(long id) : Exp
{
    public long Id => id;

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
        VarExp varExpr = (VarExp)o;
        return id == varExpr.Id;
    }

    public override int GetHashCode() => id.GetHashCode();

    protected internal override void PrettyPrint(StringBuilder b, string indent)
    {
        b.Append(indent + "x").Append(id);
    }
}