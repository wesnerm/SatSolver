namespace SatSolver;
using static ExpFactory;

public class ExpBaseTransformASTListener : ExpBaseASTListener
{
	internal Dictionary<Exp, Exp> replMap = new();

	protected internal virtual Exp newExp(Exp e)
	{
		return replMap.GetValueOrDefault(e, e);
	}

	public override void exitVAR(VarExp e)
	{
		newExp(e);
	}

	public override void exitNEG(NegExp e)
	{
		Exp inExpr = e.Expr;
		Exp newInExpr = newExp(inExpr);

		if (newInExpr != inExpr)
		{
			replMap[e] = NEG(newInExpr);
		}
	}

	public override void exitOR(OrExp e)
	{
		Exp left = e.Left, right = e.Right;

		Exp newLeft = newExp(left), newRight = newExp(right);

		if ((left != newLeft) || (right != newRight))
		{
			replMap[e] = OR(newLeft, newRight);
		}
	}

	public override void exitAND(AndExp e)
	{
		Exp left = e.Left, right = e.Right;

		Exp newLeft = newExp(left), newRight = newExp(right);

		if ((left != newLeft) || (right != newRight))
		{
			replMap[e] = AND(newLeft, newRight);
		}
	}

	public override void exitIMPL(ImplExp e)
	{
		Exp antecedent = e.Antecedent, consequent = e.Consequent;

		Exp newAntecedent = newExp(antecedent), newConsequent = newExp(consequent);

		if ((newAntecedent != antecedent) || (newConsequent != consequent))
		{
			replMap[e] = IMPL(newAntecedent, newConsequent);
		}
	}

	public override void exitEQUIV(EquivExp e)
	{
		Exp left = e.Left, right = e.Right;

		Exp newLeft = newExp(left), newRight = newExp(right);

		if ((left != newLeft) || (right != newRight))
		{
			replMap[e] = EQUIV(newLeft, newRight);
		}
	}

	public virtual Exp getTransformedExpr(Exp e)
	{
		return newExp(e);
	}
}