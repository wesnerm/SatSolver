namespace CongruenceClosure;

public class ExpUtils
{
    public static void Validate(ISet<Exp> exprs)
    {
        var func2arg = new Dictionary<long, int>();

        void Check(Exp e)
        {
            switch (e)
            {
                case EqExp { Left: Exp left, Right: Exp right }:
                    Check(left);
                    Check(right);
                    break;
                case NegExp { Left: Exp left, Right: Exp right }:
                    Check(left);
                    Check(right);
                    break;
                case FappExp fExpr:
                    int argCnt = func2arg[fExpr.Id] = func2arg.GetValueOrDefault(fExpr.Id, fExpr.Exprs.Count);
                    if (argCnt != fExpr.Exprs.Count)
                        throw new InvalidOperationException("Function argument mismatch: " + "f" + fExpr.Id + " " + argCnt + " != " + fExpr.Exprs.Count);
                    foreach (Exp arg in fExpr.Exprs)
                        Check(arg);
                    break;
            }
        }

        foreach (Exp e in exprs)
            Check(e);
    }

    public static bool CheckSat(List<Exp> exprs)
    {
        var solver = new CCSolver(exprs);
        return solver.IsSat;
    }
}
