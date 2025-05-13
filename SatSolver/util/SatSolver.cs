using System;
using System.Diagnostics;

namespace SatSolver;

public class SatSolver
{
    public List<int[]> clauses = new();
    public List<int[]> learnedClauses = new();
    public Variable[] variables = Array.Empty<Variable>();
    public Variable?[] trail = Array.Empty<Variable>();
    public int trailLength = 0, trailPropagated = 0;
    public int decisionLevel = 0;
    public Random random = new Random();
    private FastSet conflicted = new FastSet(0);
    private VarHeap freeVariables = new VarHeap(0);
    public double[] scores = Array.Empty<double>();

    public bool solve(Exp expr)
    {
        buildClauses(expr);
        fixVariables();
        foreach (var clause in clauses)
            if (!addWatches(clause))
                return false;

        //return dpll();
        return cdcl();
    }

    public Dictionary<long, bool>? findSatisfyingAssignment(Exp expr)
    {
        return solve(expr) ? trail.ToDictionary(x => (long)x!.originalId, x => x!.value == 1) : null;
    }

    private int chooseRandom()
    {
        int size = freeVariables.size;
        if (size <= 0)
            return 0;

        int index = random.Next(size);
        int v = freeVariables.list[index];
        return v * variables[v].previousValue;
    }

    private int chooseMax()
    {
        int size = freeVariables.size;
        if (size == 0)
            return 0;

        //var list = freeVariables.list;
        //int maxV = list[0];
        //double maxScore = variables[maxV].activity;
        //for (int i = 1; i < size; i++)
        //{
        //    int v = list[i];
        //    var vv = variables[v];
        //    double score = vv.activity;
        //    if (score > maxScore)
        //    {
        //        maxV = v;
        //        maxScore = score;
        //    }
        //}

        int maxV = freeVariables.peek();
        double maxScore = scores[maxV];

        if (maxScore > 1e10)
        {
            Array.Clear(scores);
        }

        return maxV * variables[maxV].previousValue;
    }

    private bool dpll()
    {
        while (unitPropagation() == null)
        {
            var v = chooseRandom();
            if (v == 0) return true;

            int savedLevel = decisionLevel;
            if (assign(v, null) && dpll())
                return true;

            rollbackTo(savedLevel);
            if (!assign(-v, null))
                return false;
        }

        return false;
    }

    private bool cdcl()
    {
        while (true)
        {
            var conflict = unitPropagation();
            if (conflict != null)
            {
                if (!analyzeConflict(conflict))
                    return false;
            }
            else
            {
                var v = chooseMax();
                if (v == 0) return true;
                if (!assign(v, null))
                    return false;
            }
        }
    }

    private int[]? unitPropagation()
    {
        for (; trailPropagated < trailLength; trailPropagated++)
        {
            var assignment = trail[trailPropagated]!;
            var literal = -assignment.id * assignment.value;
            var watches = literal >= 0 ? assignment.watches : assignment.watchesNeg;
            for (int i = watches.Count - 1; i >= 0; i--)
            {
                int[]? clause = watches[i];

                if (clause[0] == literal)
                {
                    clause[0] = clause[1];
                    clause[1] = literal;
                }

                // If the other watch is true, then the clause is satisfied
                int literal2 = clause[0];
                if (getValue(literal2) == 1)
                    continue;

                bool found = false;
                for (int j = 2; j < clause.Length; j++)
                    if (getValue(clause[j]) != -1)
                    {
                        // Replace watch with a literal that is not false
                        int literal3 = clause[j];
                        clause[1] = literal3;
                        clause[j] = literal;
                        watches[i] = watches[watches.Count - 1];
                        watches.RemoveAt(watches.Count - 1);
                        addWatch(clause, literal3);
                        found = true;
                        break;
                    }

                if (!found && !assign(literal2, clause))
                    return clause;
            }
        }

        return null;
    }

    private bool analyzeConflict(int[] conflictClause)
    {
        if (decisionLevel == 0) return false;

        // Identify conflict variables
        conflicted.clear();
        int uip = 0, conflictCount = 0;
        int[]? reason = conflictClause;
        for (int level = trailLength - 1; true; level--)
        {
            if (reason != null)
            {
                for (int i = uip != 0 ? 1 : 0; i < reason.Length; i++)
                {
                    var term = Math.Abs(reason[i]);
                    if (conflicted.add(term))
                    {
                        var v = variables[term];
                        scores[term] = (scores[term] + 2.0) * (100.0 / 75.0);
                        if (v.decisionLevel < decisionLevel)
                            conflictCount++;
                    }
                }
            }

            var u = trail[level]!;
            uip = -u.id * u.value;
            if (conflicted.remove(u.id))
            {
                if (conflicted.size <= conflictCount)
                    break;
                reason = u.reason;
            }
            else
            {
                reason = null;
            }
        }

        // Simplify conflicts
        var learned = new List<int>(conflictCount + 1) { uip };
        var backtrackLevel = 0;
        while (conflicted.size > 0)
        {
            var id = conflicted.pop();
            var v = variables[id];
            if (v.decisionLevel <= 0) //|| subsumedClause(v.reason))
                continue;
            int literal = -id * v.value;
            learned.Add(literal);
            if (backtrackLevel < v.decisionLevel)
            {
                backtrackLevel = v.decisionLevel;
                learned[learned.Count - 1] = learned[1];
                learned[1] = literal;
            }
        }

        rollbackTo(backtrackLevel);

        var learnedClause = learned.ToArray();
        learnedClauses.Add(learnedClause);
        return addWatches(learnedClause) && assign(uip, learnedClause);
    }

    private bool subsumedClause(int[]? clause)
    {
        if (clause == null)
            return false;
        for (int i = 1; i < clause.Length; i++)
            if (!conflicted.contains(Math.Abs(clause[i])))
                return false;
        return true;
    }


    private int getValue(int literal)
    {
        var variable = variables[Math.Abs(literal)];
        return variable.value * Math.Sign(literal);
    }

    private bool addWatches(int[] clause)
    {
        if (clause.Length < 2)
            return clause.Length > 0 && assign(clause[0], clause);

        addWatch(clause, clause[0]);
        addWatch(clause, clause[1]);
        return true;
    }

    private void addWatch(int[] clause, int literal)
    {
        int variableId = Math.Abs(literal);
        var variable = variables[variableId];
        var watches = literal >= 0 ? variable.watches : variable.watchesNeg;
        watches.Add(clause);
    }

    private bool assign(int literal, int[]? clause)
    {
        var id = Math.Abs(literal);
        var value = Math.Sign(literal);
        var assignment = variables[id];
        if (assignment.value != 0)
            return assignment.value == value;

        // Is Decision
        if (clause == null)
            decisionLevel = trailLength + 1; // +1 to avoid confusion with unit clauses

        assignment.value = assignment.previousValue = value;
        assignment.decisionLevel = decisionLevel;
        assignment.reason = clause;
        trail[trailLength++] = assignment;
        freeVariables.remove(id);
        return true;
    }

    private void rollbackTo(int level)
    {
        for (; trailLength > 0; trailLength--)
        {
            var assignment = trail[trailLength - 1]!;
            if (assignment.decisionLevel <= level)
                break;
            assignment.value = 0;
            assignment.decisionLevel = -1;
            assignment.reason = null;
            freeVariables.add(assignment.id);
            trail[trailLength - 1] = null;
        }

        trailPropagated = trailLength; // Math.Min(trailPropagated, trailLength);
        decisionLevel = level; // trailLength > 0 ? trail[trailLength - 1]!.decisionLevel : 0;
    }

    private void fixVariables()
    {
        var ids = clauses.SelectMany(x => x).Select(Math.Abs).Distinct().Order().ToList();

        variables = new Variable[ids.Count + 1];
        trail = new Variable[ids.Count];
        scores = new double[variables.Length];
        freeVariables = new VarHeap(variables.Length, scores);
        // freeVariables = new FastSet(variables.Length);
        conflicted = new FastSet(variables.Length);

        for (int i = 0; i < ids.Count; i++)
        {
            var v = i + 1;
            variables[v] = new Variable { id = v, originalId = ids[i], };
            freeVariables.add(v);
        }

        var newAssignments = ids.Select((x, i) => new { x, i = i + 1 }).ToDictionary(x => x.x, x => x.i);
        foreach (var clause in clauses)
        {
            for (int i = 0; i < clause.Length; i++)
            {
                clause[i] = newAssignments[Math.Abs(clause[i])] * Math.Sign(clause[i]);
            }
        }
    }

    private void buildClauses(Exp exp)
    {
        if (exp is AndExp andExp)
        {
            buildClauses(andExp.Left);
            buildClauses(andExp.Right);
            return;
        }

        var clause = new HashSet<long>();
        buildClause(exp);
        clauses.Add(clause.Select(x => (int)x).ToArray());

        void buildClause(Exp orClause)
        {
            switch (orClause)
            {
                case OrExp orExp:
                    buildClause(orExp.Left);
                    buildClause(orExp.Right);
                    return;

                case NegExp { Exp: VarExp v }:
                    clause.Add(-v.Id);
                    return;

                case VarExp v2:
                    clause.Add(v2.Id);
                    return;

                default:
                    throw new InvalidOperationException("Clause must be in CNF");
            }
        }
    }
}
