using System.Collections;

namespace Cells1;

public class Crystal1DSnapshot : IEnumerable<int>
{
    private readonly int[] _cells;
    public int N => _cells.Length;
    public int K { get; }
    public double P { get; }
    private bool _isTouched;

    public Crystal1DSnapshot(int[] cells, int k, double p, bool isTouched)
    {
        ArgumentNullException.ThrowIfNull(cells);
        _cells = cells;
        K = k;
        P = p;
        _isTouched = isTouched;
    }
    
    public int GetCell(int i)
    {
        if (i < 0 || i >= _cells.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(i), i, $"i must be between 0 and {_cells.Length}");
        }
        
        return _cells[i];
    }

    public int GetAtomsCount()
    {
        return _cells.Sum();
    }

    public IEnumerator<int> GetEnumerator()
    {
        return ((IEnumerable<int>)_cells).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
