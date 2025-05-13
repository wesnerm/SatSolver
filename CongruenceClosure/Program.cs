using CongruenceClosure;

try
{
    var e = ExpParser.Parse(Console.In);
    Console.WriteLine(ExpUtils.CheckSat(e) ? "SAT" : "UNSAT");
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
    Console.WriteLine(ex.ToString());
    Console.Write(ex.StackTrace);
    Environment.Exit(1);
}
