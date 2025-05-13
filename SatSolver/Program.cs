using SatSolver;

var sat = SatUtil.CheckSat(Console.In);
Console.WriteLine(sat ? "SAT" : "UNSAT");
