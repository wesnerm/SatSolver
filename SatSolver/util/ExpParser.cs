namespace SatSolver;
using static ExpFactory;

public class ExpParser
{
	string? line = null;
    int lineNo = 0;
    int col = 0;
    TextReader reader;

    public static Exp Parse(string text) => Parse(new StringReader(text));

    public static Exp Parse(TextReader reader, bool single = true)
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

            if (!char.IsAsciiLetterOrDigit(ch))
                return line.AsSpan(origCol, 1);

            while (col < line.Length && char.IsAsciiLetterOrDigit(line[col]))
            {
                col++;
            }

            return line.AsSpan(origCol, col-origCol);
        }
    }

    private Exp ParseExpression()
    {
        var read = Read();

        if (read.Length == 0)
        {
            throw ThrowParserException($"Found newline instead of expression");
        }

        if (read[0] == 'x')
        {
            if (long.TryParse(read.Slice(1), out long id))
                return VAR(id);

            throw ThrowParserException($"Bad variable '{read}'");
        }

        if (read[0] != '(')
        {
            throw ThrowParserException("Missing '('");
        }

        var op = Read();
        Exp result = op switch
        {
            "not" => NEG(ParseExpression()),
            "and" => AND(ParseExpression(), ParseExpression()),
            "or" => OR(ParseExpression(), ParseExpression()),
            "impl" => IMPL(ParseExpression(), ParseExpression()),
            "equiv" => EQUIV(ParseExpression(), ParseExpression()),
            _ => throw new FormatException()
        };

        var read2 = Read();
        if (read.Length != 1 || read2[0] != ')')
        {
            throw ThrowParserException("Missing ')'");
        }

        return result;
    }

    private Exception ThrowParserException(string message)
    {
        message = $"{message} at line {lineNo} col {col}: {line?.Substring(0,col)}\n";
        throw new FormatException(message);
    }

}
