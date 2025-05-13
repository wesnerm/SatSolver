using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace SatSolver;

public class SatUtil
{
    public static bool CheckSat(TextReader reader)
    {
        Exp e = ExpParser.Parse(reader);
        //Tseitin's Transformation
        Exp cnfExpr = ExpUtils.toTseitin(e);
        return SatUtil.CheckSat(cnfExpr);
    }

    public static bool CheckSat(Exp expr)
    {
        var solver = new SatSolver();
        return solver.solve(expr);
    }

    public static Dictionary<long, bool>? FindSatisfyingAssignment(Exp expr)
    {
        var solver = new SatSolver();
        return solver.findSatisfyingAssignment(expr);
    }

    public static bool Evaluate(Exp expr, Dictionary<long, bool> assignments)
    {
        bool eval(Exp e) => e switch
        {
            VarExp v => assignments[v.Id],
            NegExp notExpr => !eval(notExpr.Expr),
            OrExp orExpr => eval(orExpr.Left) | eval(orExpr.Right),
            AndExp andExpr => eval(andExpr.Left) & eval(andExpr.Right),
            ImplExp implExpr => !eval(implExpr.Antecedent) | eval(implExpr.Consequent),
            EquivExp equivExpr => eval(equivExpr.Left) == eval(equivExpr.Right),
            _ => throw new InvalidOperationException(),
        };
        return eval(expr);
    }
}
