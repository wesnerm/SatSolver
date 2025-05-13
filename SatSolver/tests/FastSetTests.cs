namespace SatSolver;

using static ExpFactory;
using Xunit;
using Xunit.Abstractions;

public class FastSetTests
{
    private readonly ITestOutputHelper output;

    public FastSetTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void ConstructionTest()
    {
        var fastSet = new FastSet(5);
        Assert.Equal(0, fastSet.size);
    }

    [Fact]
    public void AddTest()
    {
        var fastSet = new FastSet(5);

        Assert.Equal(0, fastSet.size);

        for (int i = 0; i < 5; i++)
        {
            Assert.True(fastSet.add(i));
            Assert.Equal(i+1, fastSet.size);

            Assert.False(fastSet.add(i));
            Assert.Equal(i+1, fastSet.size);
        }
    }

    [Fact]
    public void ContainsTest()
    {
        var fastSet = new FastSet(5);

        Assert.Equal(0, fastSet.size);

        for (int i = 0; i < 5; i++)
        {
            Assert.False(fastSet.contains(i));
            Assert.True(fastSet.add(i));
            Assert.True(fastSet.contains(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.True(fastSet.contains(i));
            Assert.True(fastSet.remove(i));
            Assert.False(fastSet.contains(i));
        }
    }

    [Fact]
    public void SizeTest()
    {
        var fastSet = new FastSet(5);

        Assert.Equal(0, fastSet.size);

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(i, fastSet.size);
            Assert.True(fastSet.add(i));
            Assert.Equal(i+1, fastSet.size);
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(5 - i, fastSet.size);
            Assert.True(fastSet.remove(i));
            Assert.Equal(4 - i, fastSet.size);
        }
    }

    [Fact]
    public void RemoveTest()
    {
        var fastSet = new FastSet(5);

        for (int i = 0; i < 5; i++)
        {
            Assert.False(fastSet.remove(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.True(fastSet.add(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.True(fastSet.remove(i));
            Assert.False(fastSet.remove(i));
        }
    }

    [Fact]
    public void PopTest()
    {
        var fastSet = new FastSet(5);

        for (int i = 0; i < 5; i++)
        {
            Assert.True(fastSet.add(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal( 4-i, fastSet.pop());
        }

        Assert.Equal(-1, fastSet.pop());
    }

    [Fact]
    public void ClearTest()
    {
        var fastSet = new FastSet(5);

        for (int i = 0; i < 5; i++)
        {
            Assert.True(fastSet.add(i));
        }

        fastSet.clear();

        Assert.Equal(0, fastSet.size);

        for (int i = 0; i < 5; i++)
        {
            Assert.False(fastSet.contains(i));
        }
    }

}
