namespace SatSolver;

public class ExprBaseASTListener : ExprASTListener
{
	public virtual void enterVAR(VarExp e)
	{

	}

	public virtual void exitVAR(VarExp e)
	{

	}

	public virtual bool enterNEG(NegExp e)
	{
		return true;
	}

	public virtual void exitNEG(NegExp e)
	{

	}

	public virtual bool enterOR(OrExp e)
	{
		return true;
	}

	public virtual void exitOR(OrExp e)
	{

	}

	public virtual bool enterAND(AndExp e)
	{
		return true;
	}

	public virtual void exitAND(AndExp e)
	{

	}

	public virtual bool enterIMPL(ImplExp e)
	{
		return true;
	}

	public virtual void exitIMPL(ImplExp e)
	{

	}

	public virtual bool enterEQUIV(EquivExp e)
	{
		return true;
	}

	public virtual void exitEQUIV(EquivExp e)
	{

	}
}