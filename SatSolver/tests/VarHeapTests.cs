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

            Assert.True(varHeap.Add(i));
            Assert.Equal(i+1, varHeap.size);

            Assert.False(varHeap.Add(i));
            Assert.Equal(i+1, varHeap.size);

            Assert.Equal(i, varHeap.Peek());
        }
    }

    [Fact]
    public void ContainsTest()
    {
        var varHeap = new VarHeap(5);

        Assert.Equal(0, varHeap.size);

        for (int i = 0; i < 5; i++)
        {
            Assert.False(varHeap.Contains(i));
            Assert.True(varHeap.Add(i));
            Assert.True(varHeap.Contains(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.True(varHeap.Contains(i));
            Assert.True(varHeap.Remove(i));
            Assert.False(varHeap.Contains(i));
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
            Assert.True(varHeap.Add(i));
            Assert.Equal(i+1, varHeap.size);
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(5 - i, varHeap.size);
            Assert.True(varHeap.Remove(i));
            Assert.Equal(4 - i, varHeap.size);
        }
    }

    [Fact]
    public void RemoveTest()
    {
        var varHeap = new VarHeap(5);

        for (int i = 0; i < 5; i++)
        {
            Assert.False(varHeap.Remove(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.True(varHeap.Add(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.True(varHeap.Remove(i));
            Assert.False(varHeap.Remove(i));
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
            Assert.True(varHeap.Add(i));
        }

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal( 4-i, varHeap.Peek());
            varHeap.Remove(varHeap.Peek());
        }

        Assert.Equal(-1, varHeap.Peek());
    }

    [Fact]
    public void ClearTest()
    {
        var varHeap = new VarHeap(5);

        for (int i = 0; i < 5; i++)
        {
            Assert.True(varHeap.Add(i));
        }

        varHeap.Clear();

        Assert.Equal(0, varHeap.size);

        for (int i = 0; i < 5; i++)
        {
            Assert.False(varHeap.Contains(i));
        }
    }

}
