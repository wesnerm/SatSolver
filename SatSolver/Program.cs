using SatSolver;

var sat = SatUtil.checkSAT(Console.In);
Console.WriteLine(sat ? "SAT" : "UNSAT");
