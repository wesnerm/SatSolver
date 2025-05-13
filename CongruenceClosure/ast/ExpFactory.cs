using System.Collections.Generic;
using System.Collections.Concurrent;

namespace CongruenceClosure;
public class ExpFactory
{
    private static readonly ConcurrentDictionary<Exp, Exp> cache = new ConcurrentDictionary<Exp, Exp>();

    public static VarExpr mkVAR(long id)
    {
        VarExpr v = new VarExpr(id);
        cache.GetOrAdd(v, v);
        return (VarExpr)cache[v];
    }

    public static FappExp mkFAPP(long id, List<Exp> args)
    {
        FappExp f = new FappExp(id, args);
        cache.GetOrAdd(f, f);
        return (FappExp)cache[f];
    }

    public static EqExp mkEq(Exp left, Exp right)
    {
        EqExp eq = new EqExp(left, right);
        cache.GetOrAdd(eq, eq);
        return (EqExp)cache[eq];
    }

    public static NegExp mkNeq(Exp left, Exp right)
    {
        NegExp eq = new NegExp(left, right);
        cache.GetOrAdd(eq, eq);
        return (NegExp)cache[eq];
    }
}