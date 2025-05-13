namespace CongruenceClosure;

public class CCSolver
{
    private Dictionary<Exp, Term> exprMap = new();
    private Queue<Term> unionQueue = new();
    public bool IsSat;

    public CCSolver(List<Exp> exprs)
    {
        IsSat = Solve(exprs);
    }

    private bool Solve(List<Exp> exprs)
    {
        foreach (var expr in exprs)
            if (expr is EqExp e1)
            {
                var term1 = ToTerm(e1.left);
                var term2 = ToTerm(e1.right);
                Queue(term1, term2);
            }
            else if (expr is NegExp e2)
            {
                _ = ToTerm(e2.left);
                _ = ToTerm(e2.right);
            }

        while (unionQueue.Count > 0)
        {
            Term t1 = unionQueue.Dequeue();
            Term t2 = unionQueue.Dequeue();
            Union(t1, t2);
        }

        foreach (var expr in exprs)
            if (expr is NegExp e)
            {
                var term1 = ToTerm(e.left);
                var term2 = ToTerm(e.right);
                if (term1.find() == term2.find())
                    return false;
            }

        return true;
    }

    private Term ToTerm(Exp expr)
    {
        Term? term;
        if (exprMap.TryGetValue(expr, out term))
            return term;

        switch (expr)
        {
            case FappExp e:
                term = new Term(e.Id, e.Exprs.Select(ToTerm).ToArray());
                break;
            case VarExpr e:
                term = new Term(e.Id);
                break;
            default:
                throw new InvalidOperationException("Clause must be in CNF");
        }

        exprMap[expr] = term;
        return term;
    }

    private void Queue(Term x1, Term x2)
    {
        x1 = x1.find();
        x2 = x2.find();
        if (x1 != x2)
        {
            unionQueue.Enqueue(x1);
            unionQueue.Enqueue(x2);
        }
    }

    private void Union(Term x1, Term x2)
    {
        x1 = x1.find();
        x2 = x2.find();
        if (x1 == x2)
            return;

        if (x2.count > x1.count)
            (x1, x2) = (x2, x1);

        x2.root = x1;
        x1.count += x2.count;

        foreach (var (f, list2) in x2.parents)
        {
            if (x1.parents.TryGetValue(f, out var list1))
            {
                foreach (var t2 in list2)
                {
                    bool add = true;
                    foreach (var t1 in list1)
                        if (Equivalent(t1, t2))
                        {
                            add = false;
                            Queue(t1, t2);
                        }
                   if (add)
                        list1.Add(t2);
                }
            }
            else
            {
                x1.parents[f] = list2;
            }
        }
        x2.parents.Clear();
    }

    private bool Equivalent(Term x1, Term x2)
    {
        if (x1 == x2)
            return true;

        if (x1.number != x2.number
            || x1.args.Length != x2.args.Length)
            return false;

        for (int i = 0; i < x1.args.Length; i++)
        {
            if (x1.args[i].find() != x2.args[i].find())
                return false;
        }
        return true;
    }
}
