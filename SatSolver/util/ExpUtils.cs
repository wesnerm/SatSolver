using System.Diagnostics;
namespace SatSolver;
using static ExpFactory;
using static ExpUtils;

public static class ExpUtils
{
    public static void dfsWalk(Exp e, ExpASTListener listener)
    {
        bool visitChildren = true;
        switch (e)
        {
            case VarExp varExpr:
                listener.enterVAR(varExpr);
                listener.exitVAR(varExpr);
                break;
            case NegExp negExpr:
                visitChildren = listener.enterNEG(negExpr);

                if (visitChildren)
                {
                    dfsWalk(negExpr.Expr, listener);
                }

                listener.exitNEG(negExpr);
                break;
            case AndExp andExpr:
                visitChildren = listener.enterAND(andExpr);
                if (visitChildren)
                {
                    dfsWalk(andExpr.Left, listener);
                    dfsWalk(andExpr.Right, listener);
                }

                listener.exitAND(andExpr);
                break;
            case OrExp orExpr:
                visitChildren = listener.enterOR(orExpr);

                if (visitChildren)
                {
                    dfsWalk(orExpr.Left, listener);
                    dfsWalk(orExpr.Right, listener);
                }

                listener.exitOR(orExpr);
                break;
            case ImplExp implExpr:
                visitChildren = listener.enterIMPL(implExpr);

                if (visitChildren)
                {
                    dfsWalk(implExpr.Antecedent, listener);
                    dfsWalk(implExpr.Consequent, listener);
                }

                listener.exitIMPL(implExpr);
                break;
            case EquivExp equivExpr:
                visitChildren = listener.enterEQUIV(equivExpr);

                if (visitChildren)
                {
                    dfsWalk(equivExpr.Left, listener);
                    dfsWalk(equivExpr.Right, listener);
                }

                listener.exitEQUIV(equivExpr);
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }

    public static Exp toNNF(Exp expr)
    {
        ImplEquivTransformer tr1 = new ImplEquivTransformer();
        PushNegTransformer tr2 = new PushNegTransformer();

        dfsWalk(expr, tr1);
        Exp interExpr = tr1.newExp(expr);
        dfsWalk(interExpr, tr2);

        return tr2.getTransformedExpr(interExpr);
    }

    public static Exp toCNF(Exp expr)
    {
        ExprCNFTransformer cnfTransformer = new ExprCNFTransformer();
        Exp nnfExpr = toNNF(expr);
        dfsWalk(nnfExpr, cnfTransformer);
        return cnfTransformer.getTransformedExpr(nnfExpr);
    }

    public static bool isCNF(Exp expr)
    {
        return expr switch
        {
            AndExp e => isCNF(e.Left) && isCNF(e.Right),
            OrExp => isDisjunctionOfLiterals(expr),
            ImplExp or EquivExp => false,
            NegExp e => e.Expr.Kind == ExprKind.VAR,
            _ => true,
        };
    }

	public static bool isDisjunctionOfLiterals(Exp expr)
    {
        if (expr.Kind == ExprKind.OR)
        {
            OrExp e = (OrExp)expr;
            return isDisjunctionOfLiterals(e.Left) && isDisjunctionOfLiterals(e.Right);
        }
        else if (expr.Kind == ExprKind.VAR)
        {
            return true;
        }
        else if (expr.Kind == ExprKind.NEG)
        {
            NegExp e = (NegExp)expr;
            return e.Expr.Kind == ExprKind.VAR;
        }
        return false;
    }

    public static Exp toTseitin(Exp expr)
    {
        if (isCNF(expr))
        {
            return expr;
        }
        AuxVarCollectorListener auxVarCollectorListener = new AuxVarCollectorListener(getMaxVarID(expr) + 1);
        dfsWalk(expr, auxVarCollectorListener);

        IDictionary<Exp, VarExp> auxVarMap = auxVarCollectorListener.auxVarMap;
        TseitinClausesCollector tseitinClausesCollector = new TseitinClausesCollector(auxVarMap);
        dfsWalk(expr, tseitinClausesCollector);

        IList<Exp> tseitinClauses = tseitinClausesCollector.tseitinClauses;
        return tseitinClauses.Aggregate(auxVarMap.ContainsKey(expr) ? auxVarMap[expr] : expr, AND);
    }

    public static bool isLiteral(Exp e)
    {
        ExprKind eKind = e.Kind;
        if (eKind == ExprKind.VAR)
        {
            return true;
        }

        if (eKind == ExprKind.NEG)
        {
            return ((NegExp)e).Expr.Kind == ExprKind.VAR;
        }

        return false;
    }

    private static long getMaxVarID(Exp e)
    {
        MaxIDListener maxIDListener = new MaxIDListener();
        dfsWalk(e, maxIDListener);
        return maxIDListener.maxID;
    }
}

internal class ImplEquivTransformer : ExpBaseTransformASTListener
{
    public override void exitIMPL(ImplExp e)
    {
        replMap[e] = OR(NEG(newExp(e.Antecedent)), newExp(e.Consequent));
    }

    public override void exitEQUIV(EquivExp e)
    {
        Exp newLeft = newExp(e.Left);
        Exp newRight = newExp(e.Right);
        replMap[e] = AND(OR(NEG(newExp(newLeft)), newRight), OR(NEG(newExp(newRight)), newLeft));
    }
}

internal class PushNegTransformer : ExpBaseTransformASTListener
{
    internal IDictionary<Exp, Exp> negReplMap = new Dictionary<Exp, Exp>();

    internal Stack<bool> inNeg = new Stack<bool>();

    protected internal override Exp newExp(Exp e)
    {
        Debug.Assert(!InNeg || negReplMap.ContainsKey(e));
        return InNeg ? negReplMap[e] : base.newExp(e);
    }

    private bool InNeg
    {
        get
        {
            return inNeg.Count > 0 && inNeg.Peek();
        }
    }

    public override void exitVAR(VarExp e)
    {
        if (InNeg)
        {
            negReplMap[e] = NEG(e);
        }
        else
        {
            base.exitVAR(e);
        }
    }

    public override bool enterNEG(NegExp e)
    {
        inNeg.Push(!InNeg);
        return true;
    }

    public override void exitNEG(NegExp e)
    {
        Exp newExpr = newExp(e.Expr);

        if (InNeg)
        {
            replMap[e] = newExpr;
        }
        else
        {
            negReplMap[e] = newExpr;
        }

        inNeg.Pop();
    }

    public override void exitOR(OrExp e)
    {
        Exp newLeft = newExp(e.Left);
        Exp newRight = newExp(e.Right);

        if (InNeg)
        {
            negReplMap[e] = AND(newLeft, newRight);
        }
        else
        {
            replMap[e] = OR(newLeft, newRight);
        }
    }

    public override void exitAND(AndExp e)
    {
        Exp newLeft = newExp(e.Left);
        Exp newRight = newExp(e.Right);

        if (InNeg)
        {
            negReplMap[e] = OR(newLeft, newRight);
        }
        else
        {
            replMap[e] = AND(newLeft, newRight);
        }
    }

    public override void exitIMPL(ImplExp e)
    {
        throw new System.InvalidOperationException("Formula needs to be transformed by ImplEquivTransformer first");
    }

    public override void exitEQUIV(EquivExp e)
    {
        throw new System.InvalidOperationException("Formula needs to be transformed by ImplEquivTransformer first");
    }
}

internal class ExprCNFTransformer : ExpBaseTransformASTListener
{
    public override bool enterNEG(NegExp e)
    {
        if (e.Expr.Kind != ExprKind.VAR)
        {
            throw new System.InvalidOperationException("Expr is not in NNF");
        }
        return base.enterNEG(e);
    }

    public override bool enterIMPL(ImplExp e)
    {
        throw new System.InvalidOperationException("Expr is not in NNF");
    }

    public override bool enterEQUIV(EquivExp e)
    {
        throw new System.InvalidOperationException("Expr is not in NNF");
    }

    public override void exitOR(OrExp e)
    {
        Exp left = e.Left;
        Exp right = e.Right;

        Exp newLeft = base.newExp(left), newRight = base.newExp(right);

        if (newLeft.Kind == ExprKind.AND || newRight.Kind == ExprKind.AND)
        {
            HashSet<Exp> leftClauses = clausesOf(newLeft), rightClauses = clausesOf(newRight);

            var newClauses = new List<Exp>();
            foreach (Exp cl1 in leftClauses)
            {
                foreach (Exp cl2 in rightClauses)
                {
                    newClauses.Add(OR(cl1, cl2));
                }
            }

            Exp newExpr = newClauses.GetRange(1, newClauses.Count - 1).Aggregate(newClauses[0], AND);

            replMap[e] = newExpr;
        }
        else
        {
            base.exitOR(e);
        }
    }

    private HashSet<Exp> clausesOf(Exp e)
    {
        ClausesCollector clausesCollector = new ClausesCollector();
        dfsWalk(e, clausesCollector);
        return clausesCollector.clauses;
    }
}

internal class ClausesCollector : ExpBaseASTListener
{
    internal HashSet<Exp> clauses = new HashSet<Exp>();

    public override void enterVAR(VarExp e)
    {
        clauses.Add(e);
    }

    public override bool enterNEG(NegExp e)
    {
        clauses.Add(e);
        return false;
    }

    public override bool enterOR(OrExp e)
    {
        clauses.Add(e);
        return false;
    }
}

internal class MaxIDListener : ExpBaseASTListener
{
    internal long maxID = 0;

    public override void exitVAR(VarExp e)
    {
        maxID = Math.Max(maxID, e.Id);
    }
}

internal class AuxVarCollectorListener : ExpBaseASTListener
{
    internal IDictionary<Exp, VarExp> auxVarMap = new Dictionary<Exp, VarExp>();

    internal long currId;

    public AuxVarCollectorListener(long startId)
    {
        this.currId = startId;
    }

    public override void exitNEG(NegExp e)
    {
        if (!ExpUtils.isLiteral(e) && !auxVarMap.ContainsKey(e))
        {
            auxVarMap[e] = VAR(currId++);
        }
    }

    public override void exitOR(OrExp e)
    {
        if (!auxVarMap.ContainsKey(e))
        {
            auxVarMap[e] = VAR(currId++);
        }
    }

    public override void exitAND(AndExp e)
    {
        if (!auxVarMap.ContainsKey(e))
        {
            auxVarMap[e] = VAR(currId++);
        }
    }

    public override void exitIMPL(ImplExp e)
    {
        if (!auxVarMap.ContainsKey(e))
        {
            auxVarMap[e] = VAR(currId++);
        }
    }

    public override void exitEQUIV(EquivExp e)
    {
        if (!auxVarMap.ContainsKey(e))
        {
            auxVarMap[e] = VAR(currId++);
        }
    }
}

internal class TseitinClausesCollector : ExpBaseASTListener
{
    internal IDictionary<Exp, VarExp> auxVarMap;

    internal IList<Exp> tseitinClauses = new List<Exp>();

    public TseitinClausesCollector(IDictionary<Exp, VarExp> auxVarMap)
    {
        this.auxVarMap = auxVarMap;
    }

    internal virtual Exp getAuxVarOrExpr(Exp e)
    {
        return auxVarMap.ContainsKey(e) ? auxVarMap[e] : e;
    }

    public override void exitNEG(NegExp e)
    {
        if (!isLiteral(e))
        {
            tseitinClauses.Add(ExpUtils.toCNF(EQUIV(getAuxVarOrExpr(e), NEG(getAuxVarOrExpr(e.Expr)))));
        }
    }

    public override void exitOR(OrExp e)
    {
        tseitinClauses.Add(ExpUtils.toCNF(EQUIV(getAuxVarOrExpr(e), OR(getAuxVarOrExpr(e.Left), getAuxVarOrExpr(e.Right)))));
    }

    public override void exitAND(AndExp e)
    {
        tseitinClauses.Add(ExpUtils.toCNF(EQUIV(getAuxVarOrExpr(e), AND(getAuxVarOrExpr(e.Left), getAuxVarOrExpr(e.Right)))));
    }

    public override void exitIMPL(ImplExp e)
    {
        tseitinClauses.Add(ExpUtils.toCNF(EQUIV(getAuxVarOrExpr(e), IMPL(getAuxVarOrExpr(e.Antecedent), getAuxVarOrExpr(e.Consequent)))));
    }

    public override void exitEQUIV(EquivExp e)
    {
        tseitinClauses.Add(ExpUtils.toCNF(EQUIV(getAuxVarOrExpr(e), EQUIV(getAuxVarOrExpr(e.Left), getAuxVarOrExpr(e.Right)))));
    }

}
