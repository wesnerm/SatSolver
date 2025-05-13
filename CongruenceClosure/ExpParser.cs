namespace CongruenceClosure;

using static ExpFactory;

public class ExpParser
{
	string? line = null;
    int lineNo = 0;
    int col = 0;
    TextReader reader;

    public static List<Exp> Parse(string text) => Parse(new StringReader(text));

    public static List<Exp> Parse(TextReader reader, bool single = true)
    {
        var parser = new ExpParser(reader);

        var result = parser.ParseExpression();

        if (single && parser.Read().Length > 0)
            throw parser.ThrowParserException($"Additional formula found");

        return result;
    }

    public ExpParser(TextReader reader)
	{
        this.reader = reader;
	}

    private ReadOnlySpan<char> Peek()
    {
        int lineSav = lineNo;
        int colSav = col;
        var span = Read();
        col = lineSav == lineNo ? colSav : 0;
        return span;
    }

    private ReadOnlySpan<char> Read()
    {
        while (true)
        {
            if (line == null || col >= line.Length)
            {
                line = reader.ReadLine();
                if (line == null) return ReadOnlySpan<char>.Empty;
                lineNo++;
                col = 0;
                continue;
            }

            int origCol = col;
            char ch = line[col++];
            if (char.IsWhiteSpace(ch))
                continue;

            if (char.IsAsciiLetterOrDigit(ch))
            {
                while (col < line.Length && char.IsAsciiLetterOrDigit(line[col]))
                    col++;
            }
            else if (ch == '!' && col < line.Length && line[col]=='=')
            {
                col++;
            }

            return line.AsSpan(origCol, col - origCol);
        }
    }

    private void Expect(char ch)
    {
        var read = Read();

        if (read.Length != 1 || read[0] != ch)
            throw ThrowParserException($"Expected '{ch}' but found '{read.ToString()}'");
    }

    private List<Exp> ParseExpression()
    {
        List<Exp> list = new();

        while (true)
        {
            Expect('(');
            var term1 = ParseTerm();
            var op = Read();
            switch (op)
            {
                case "=":
                    var term2 = ParseTerm();
                    list.Add(mkEq(term1, term2));
                    break;
                case "!=":
                    term2 = ParseTerm();
                    list.Add(mkNeq(term1, term2));
                    break;
                default:
                    throw ThrowParserException($"Expected = or != but got '{op}'");
            }
            Expect(')');

            var read = Peek();
            if (read.Length != 1 || read[0] != ',')
                return list;

            Read();
        }
    }

    private Exp ParseTerm()
    {
        var read = Read();

        if (read.Length == 0)
            throw ThrowParserException($"Found newline instead of expression");

        long id = long.Parse(read.Slice(1));
        if (read[0] == 'x')
            return mkVAR(id);
        
        if (read[0] != 'f')
            throw ThrowParserException($"Expected term but found {read}");

        List<Exp> args = new ();
        Expect('(');
        while (true)
        {
            var arg = ParseTerm();
            args.Add(arg);

            var op = Read();
            if (op is ")") return mkFAPP(id, args);
            if (op is not ",") throw ThrowParserException($"Expected ',' but found {op}");
        }
    }

    private Exception ThrowParserException(string message)
    {
        message = $"{message} at line {lineNo} col {col}: {line?.Substring(0,col)}\n";
        throw new FormatException(message);
    }

}
