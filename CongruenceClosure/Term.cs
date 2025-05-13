namespace CongruenceClosure;

internal class Term : IEquatable<Term>
{
    public static int idCounter = 0;
    public int id, count;
    public long number;
    public Term[] args;
    public Dictionary<long, List<Term>> parents = new ();
    public Dictionary<long, Term> injectiveFunctions = new ();
    public Term root;
        
    public Term(long number, params Term[] args)
    {
        this.id = idCounter++;
        this.number = number;
        this.args = args;
        this.root = this;
        this.count = 1;
        foreach (var arg in args)
        {
            if (!arg.parents.ContainsKey(number))
                arg.parents[number] = new List<Term>();

            var list = arg.parents[number];
            if (list.Count == 0 || list[list.Count-1] != this)
                list.Add(this);
        }

        if (args.Length > 0)
            this.injectiveFunctions[number] = this;
    }

    public Term find()
    {
        return root == this ? this : (root = root.find());
    }

    public override int GetHashCode()
    {
        return id;
    }

    public override bool Equals(object? obj)
    {
        return obj == this;
    }

    public override string ToString()
    {
        if (args.Length == 0)
            return $"x{number}";

        return $"f{number}({string.Join(",", (object[])args)})";
    }

    public bool Equals(Term? other)
    {
        return other == this;
    }
}
