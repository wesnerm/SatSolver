using System.Text;

namespace CongruenceClosure;

public abstract class Exp
{
    public enum ExprKind
    {
        VAR,
        EQ,
        NEQ,
        FAPP,
    }

    protected internal abstract void prettyPrint(StringBuilder b, string indent);

    public abstract ExprKind Kind { get; }

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        prettyPrint(b, "");
        return b.ToString();
    }
}