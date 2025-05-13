namespace SatSolver;

public class Variable
{
    public int id, originalId;
    public int decisionLevel = -1;
    public int value, previousValue = 1;
    public int[]? reason;

    public List<int[]> watches { get; } = new List<int[]>();
    public List<int[]> watchesNeg { get; } = new List<int[]>();

    public override string ToString() => $"{id} - {value}";
}
