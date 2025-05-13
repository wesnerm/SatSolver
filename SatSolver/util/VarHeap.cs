using System.Collections;

namespace SatSolver;

public class VarHeap
{
    int[] indices;
    public int[] list;
    public double[] scores;
    public int size { get; private set; } = 0;

    public VarHeap(int size, double[]? scoresToUse = null)
    {
        indices = new int[size];
        list = new int[size];
        scores = scoresToUse ?? new double[size];
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

        int i = size++;
        indices[n] = i;
        list[i] = n;
        while (i > 0)
        {
            int parent = (i - 1) >> 1;
            var value = list[parent];
            if (scores[value] >= scores[n]) break;
            indices[value] = i;
            list[i] = value;
            i = parent;
        }
        indices[n] = i;
        list[i] = n;
        return true;
    }

    public bool remove(int n)
    {
        var index = indices[n];
        if (index < 0 || index >= size || list[index] != n)
            return false;

        var elem = list[--size];
        indices[elem] = index;
        list[index] = elem;
        indices[n] = -1;
        list[size] = -1;

        while (true)
        {
            int child = 2 * index + 1;
            if (child >= size) break;

            if (child + 1 < size && scores[list[child]] < scores[list[child + 1]])
                child++;

            int childElem = list[child];
            if (scores[childElem] <= scores[elem])
                break;

            indices[childElem] = index;
            list[index] = childElem;

            index = child;
        }

        indices[elem] = index;
        list[index] = elem;
        return true;
    }

    public int peek()
    {
        if (size <= 0) return -1;
        return list[0];
    }

    public bool contains(int n)
    {
        var index = indices[n];
        return index >= 0 && index < size && list[index] == n;
    }

}
