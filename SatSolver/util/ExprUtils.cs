using System.Diagnostics;
namespace SatSolver;
using static ExpFactory;
using static ExprUtils;

public static class ExprUtils
{

    public static void dfsWalk(Exp e, ExprASTListener listener)
    {
        bool visitChildren = true;
        switch (e.Kind)
        {
            case ExprKind.VAR:
                VarExp varExpr = (VarExp)e;
                listener.enterVAR(varExpr);
                listener.exitVAR(varExpr);
                break;
            case ExprKind.NEG:
                NegExp negExpr = (NegExp)e;

                visitChildren = listener.enterNEG(negExpr);

                if (visitChildren)
                {
                    dfsWalk(negExpr.Exp, listener);
                }

                listener.exitNEG(negExpr);
                break;
            case ExprKind.AND:
                AndExp andExpr = (AndExp)e;

                visitChildren = listener.enterAND(andExpr);

                if (visitChildren)
                {
                    dfsWalk(andExpr.Left, listener);
                    dfsWalk(andExpr.Right, listener);
                }

                listener.exitAND(andExpr);
                break;
            case ExprKind.OR:
                OrExp orExpr = (OrExp)e;

                visitChildren = listener.enterOR(orExpr);

                if (visitChildren)
                {
                    dfsWalk(orExpr.Left, listener);
                    dfsWalk(orExpr.Right, listener);
                }

                listener.exitOR(orExpr);
                break;
            case ExprKind.IMPL:
                ImplExp implExpr = (ImplExp)e;

                visitChildren = listener.enterIMPL(implExpr);

                if (visitChildren)
                {
                    dfsWalk(implExpr.Antecedent, listener);
                    dfsWalk(implExpr.Consequent, listener);
                }

                listener.exitIMPL(implExpr);
                break;
            case ExprKind.EQUIV:
                EquivExp equivExpr = (EquivExp)e;

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

    public static Exp parse(TextReader reader)
    {
        return ExpParser.Parse(reader);
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
		if (expr.Kind == ExprKind.AND)
		{
			AndExp e = (AndExp) expr;
			return isCNF(e.Left) && isCNF(e.Right);
		}
		else if (expr.Kind == ExprKind.IMPL)
		{
			return false;
		}
		else if (expr.Kind == ExprKind.EQUIV)
		{
			return false;
		}
		else if (expr.Kind == ExprKind.NEG)
		{
			NegExp e = (NegExp) expr;
			return e.Exp.Kind == ExprKind.VAR;
		}
		else if (expr.Kind == ExprKind.OR)
		{
			return isDisjunctionOfLiterals(expr);
		}
		return true;
	}

	public static bool isDisjunctionOfLiterals(Exp expr)
	{
		if (expr.Kind == ExprKind.OR)
		{
			OrExp e = (OrExp) expr;
			return isDisjunctionOfLiterals(e.Left) && isDisjunctionOfLiterals(e.Right);
		}
		else if (expr.Kind == ExprKind.VAR)
		{
			return true;
		}
		else if (expr.Kind == ExprKind.NEG)
		{
			NegExp e = (NegExp) expr;
			return e.Exp.Kind == ExprKind.VAR;
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

	public static Exp parseFromDimcas(Stream inStream)
	{
		var clauses = new HashSet<Exp>();
		var input = new StreamReader(inStream);

		string? l;
		while (( l = input.ReadLine()) != null)
		{
			if (!l.StartsWith("p cnf ", StringComparison.Ordinal))
			{
				clauses.Add(l.Split().Where(x=>long.TryParse(x, out var n) && n != 0).Select(long.Parse).Select<long, Exp>(lit => lit > 0 ? VAR(lit) : NEG(VAR(-lit))).Aggregate(OR));
			}
		}

		return clauses.Aggregate(AND);
	}

	public static void printDimcas(Exp expr, TextWriter writer)
	{
		var clauses = new HashSet<HashSet<long>>();
		var vars = new HashSet<long>();

		Stack<Exp> s = new Stack<Exp>();
		s.Push(expr);

		while (s.Count > 0)
		{
			Exp e = s.Pop();

			if (!canBeCNF(e))
			{
				throw new Exception("Expr is not in CNF.");
			}

			switch (e.Kind)
			{
				case ExprKind.AND:
					AndExp andExpr = (AndExp) e;
					s.Push(andExpr.Left);
					s.Push(andExpr.Right);
					break;
				case ExprKind.NEG:
					if (!isLiteral(e))
					{
						throw new Exception("Expr is not in CNF.");
					}

					VarExp childVarExpr = (VarExp)((NegExp) e).Exp;

					clauses.Add([ -childVarExpr.Id ]);
					vars.Add(childVarExpr.Id);
					break;
				case ExprKind.VAR:
					VarExp varExpr = (VarExp) e;
					clauses.Add([varExpr.Id]);
					vars.Add(varExpr.Id);
					break;
				case ExprKind.OR:
					clauses.Add(getLiteralsForClause((OrExp) e, vars));
					break;
				default:
					Debug.Assert(false);
				break;
			}
		}

		Console.WriteLine("p cnf " + vars.Count + " " + clauses.Count);

		foreach (var c in clauses)
		{
			foreach(var l in c)
				Console.Write(l + " ");
			Console.WriteLine(0);
		};
	}

	public static bool canBeCNF(Exp e)
	{
		ExprKind eKind = e.Kind;
		return eKind != ExprKind.EQUIV && eKind != ExprKind.IMPL;
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
			return ((NegExp) e).Exp.Kind == ExprKind.VAR;
		}

		return false;
	}

	public static HashSet<long> getLiteralsForClause(OrExp orExpr, ISet<long> vars)
	{
		var literals = new HashSet<long>();
		var s = new Stack<Exp>();
		s.Push(orExpr.Left);
		s.Push(orExpr.Right);

		while (s.Count > 0)
		{
			Exp e = s.Pop();

			if (e.Kind != ExprKind.OR && !isLiteral(e))
			{
				throw new Exception("Expr is not in CNF");
			}

			switch (e.Kind)
			{
				case ExprKind.OR:
					OrExp or = (OrExp) e;
					s.Push(or.Left);
					s.Push(or.Right);
					break;
				case ExprKind.VAR:
					long varId = ((VarExp) e).Id;
					literals.Add(varId);
					vars.Add(varId);
					break;
				case ExprKind.NEG:
					NegExp neg = (NegExp) e;
					long litId = -((VarExp)neg.Exp).Id;
					literals.Add(litId);
					vars.Add(-litId);
					break;
				default:
					Debug.Assert(false);
				break;
			}
		}
		return literals;
	}

	private static long getMaxVarID(Exp e)
	{
		MaxIDListener maxIDListener = new MaxIDListener();
		dfsWalk(e, maxIDListener);
		return maxIDListener.maxID;
	}
}

internal class ImplEquivTransformer : ExprBaseTransformASTListener
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

internal class PushNegTransformer : ExprBaseTransformASTListener
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
		Exp newExpr = newExp(e.Exp);

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

internal class ExprCNFTransformer : ExprBaseTransformASTListener
{
	public override bool enterNEG(NegExp e)
	{
		if (e.Exp.Kind != ExprKind.VAR)
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

			Exp newExpr = newClauses.GetRange(1, newClauses.Count-1).Aggregate(newClauses[0], AND);

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

internal class ClausesCollector : ExprBaseASTListener
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

internal class MaxIDListener : ExprBaseASTListener
{
	internal long maxID = 0;

	public override void exitVAR(VarExp e)
	{
		maxID = Math.Max(maxID, e.Id);
	}
}

internal class AuxVarCollectorListener : ExprBaseASTListener
{
	internal IDictionary<Exp, VarExp> auxVarMap = new Dictionary<Exp, VarExp>();

	internal long currId;

	public AuxVarCollectorListener(long startId)
	{
		this.currId = startId;
	}

	public override void exitNEG(NegExp e)
	{
		if (!ExprUtils.isLiteral(e) && !auxVarMap.ContainsKey(e))
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

internal class TseitinClausesCollector : ExprBaseASTListener
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
		if (!ExprUtils.isLiteral(e))
		{
			tseitinClauses.Add(ExprUtils.toCNF(EQUIV(getAuxVarOrExpr(e), NEG(getAuxVarOrExpr(e.Exp)))));
		}
	}

	public override void exitOR(OrExp e)
	{
		tseitinClauses.Add(ExprUtils.toCNF(EQUIV(getAuxVarOrExpr(e), OR(getAuxVarOrExpr(e.Left), getAuxVarOrExpr(e.Right)))));
	}

	public override void exitAND(AndExp e)
	{
		tseitinClauses.Add(ExprUtils.toCNF(EQUIV(getAuxVarOrExpr(e), AND(getAuxVarOrExpr(e.Left), getAuxVarOrExpr(e.Right)))));
	}

	public override void exitIMPL(ImplExp e)
	{
		tseitinClauses.Add(ExprUtils.toCNF(EQUIV(getAuxVarOrExpr(e), IMPL(getAuxVarOrExpr(e.Antecedent), getAuxVarOrExpr(e.Consequent)))));
	}

	public override void exitEQUIV(EquivExp e)
	{
		tseitinClauses.Add(ExprUtils.toCNF(EQUIV(getAuxVarOrExpr(e), EQUIV(getAuxVarOrExpr(e.Left), getAuxVarOrExpr(e.Right)))));
	}

}
