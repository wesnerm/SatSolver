namespace SatSolver;

public interface ExpASTListener
{
	void enterVAR(VarExp e);

	void exitVAR(VarExp e);

	bool enterNEG(NegExp e);

	void exitNEG(NegExp e);

	bool enterOR(OrExp e);

	void exitOR(OrExp e);

	bool enterAND(AndExp e);

	void exitAND(AndExp e);

	bool enterIMPL(ImplExp e);

	void exitIMPL(ImplExp e);

	bool enterEQUIV(EquivExp e);

	void exitEQUIV(EquivExp e);
}