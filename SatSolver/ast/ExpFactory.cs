using System.Collections.Concurrent;

namespace SatSolver;

public class ExpFactory
{
    private static readonly ConcurrentDictionary<Exp, Exp> cache = new ConcurrentDictionary<Exp, Exp>();

    public static VarExp VAR(long id)
    {
        VarExp v = new VarExp(id);
        cache.GetOrAdd(v, v);
        return (VarExp)cache[v];
    }

    public static NegExp NEG(Exp e)
    {
        NegExp neg = new NegExp(e);
        cache.GetOrAdd(neg, neg);
        return (NegExp)cache[neg];
    }

    public static AndExp AND(Exp left, Exp right)
    {
        AndExp and = new AndExp(left, right);
        cache.GetOrAdd(and, and);
        return (AndExp)cache[and];
    }

    public static OrExp OR(Exp left, Exp right)
    {
        OrExp or = new OrExp(left, right);
        cache.GetOrAdd(or, or);
        return (OrExp)cache[or];
    }

    public static ImplExp IMPL(Exp antecedent, Exp consequent)
    {
        ImplExp impl = new ImplExp(antecedent, consequent);
        cache.GetOrAdd(impl, impl);
        return (ImplExp)cache[impl];
    }

    public static EquivExp EQUIV(Exp left, Exp right)
    {
        EquivExp equiv = new EquivExp(left, right);
        cache.GetOrAdd(equiv, equiv);
        return (EquivExp)cache[equiv];
    }
}
