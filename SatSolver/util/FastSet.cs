using System.Collections;

namespace SatSolver;

public class FastSet : IEnumerable<int>
{
    int[] indices;
    public int[] list;
    public int size { get; private set; } = 0;
    private static Random random = new Random();

    public FastSet(int size)
    {
        indices = new int[size];
        list = new int[size];
        this.size = 0;
    }

    public void clear()
    {
        size = 0;
    }

    public bool add(int n)
    {
        if (contains(n))
            return false;
        list[size] = n;
        indices[n] = size++;
        return true;
    }

    public bool remove(int n)
    {
        var index = indices[n];
        if (index < 0 || index >= size || list[index] != n)
            return false;

        var tmp = list[--size];
        indices[tmp] = index;
        list[index] = tmp;
        indices[n] = -1;
        list[size] = -1;
        return true;
    }

    public int pop()
    {
        if (size <= 0) return -1;
        var n = list[--size];
        indices[n] = -1;
        return n;
    }

    public bool contains(int n)
    {
        var index = indices[n];
        return index >= 0 && index < size && list[index] == n;
    }

    public IEnumerator<int> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
            yield return list[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
