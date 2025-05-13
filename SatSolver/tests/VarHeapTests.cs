namespace SatSolver;

using Xunit;
using Xunit.Abstractions;

public class VarHeapTests
{
    private readonly ITestOutputHelper output;

    public VarHeapTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void ConstructionTest()
    {
        var varHeap = new VarHeap(5);
        Assert.Equal(0, varHeap.size);
    }

    [Fact]
    public void AddTest()
    {
        var varHeap = new VarHeap(5);

        Assert.Equal(0, varHeap.size);

        for (int i = 0; i < 5; i++)
        {
            varHeap.scores[i] = i;

            Assert.True(varHeap.add(i));
            Assert.Equal(i+1, varHeap.size);

            Assert.False(varHeap.add(i));
            Assert.Equal(i+1, varHeap.size);

            Assert.Equal(i, varHeap.peek());
        }
    }

    [Fact]
    public void ContainsTest()
    {
        var varHeap = new VarHeap(5);

        Assert.Equal(0, varHeap.size);

        for (int i = 0; i < 5; i++)
        {
            Assert.False(varHeap.contains(i));
            Assert.True(varHeap.add(i));
            Assert.True(varHeap.contains(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.True(varHeap.contains(i));
            Assert.True(varHeap.remove(i));
            Assert.False(varHeap.contains(i));
        }
    }

    [Fact]
    public void SizeTest()
    {
        var varHeap = new VarHeap(5);

        Assert.Equal(0, varHeap.size);

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(i, varHeap.size);
            Assert.True(varHeap.add(i));
            Assert.Equal(i+1, varHeap.size);
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(5 - i, varHeap.size);
            Assert.True(varHeap.remove(i));
            Assert.Equal(4 - i, varHeap.size);
        }
    }

    [Fact]
    public void RemoveTest()
    {
        var varHeap = new VarHeap(5);

        for (int i = 0; i < 5; i++)
        {
            Assert.False(varHeap.remove(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.True(varHeap.add(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.True(varHeap.remove(i));
            Assert.False(varHeap.remove(i));
        }
    }

    [Fact]
    public void PeekTest()
    {
        var varHeap = new VarHeap(5);

        for (int i = 0; i < 5; i++)
            varHeap.scores[i] = i;

        for (int i = 0; i < 5; i++)
        {
            Assert.True(varHeap.add(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal( 4-i, varHeap.peek());
            varHeap.remove(varHeap.peek());
        }

        Assert.Equal(-1, varHeap.peek());
    }

    [Fact]
    public void ClearTest()
    {
        var varHeap = new VarHeap(5);

        for (int i = 0; i < 5; i++)
        {
            Assert.True(varHeap.add(i));
        }

        varHeap.clear();

        Assert.Equal(0, varHeap.size);

        for (int i = 0; i < 5; i++)
        {
            Assert.False(varHeap.contains(i));
        }
    }

}
