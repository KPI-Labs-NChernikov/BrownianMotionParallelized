namespace Cells1;

public sealed class Crystal1D
{
    private readonly int[] _cells;
    private readonly int _k;
    private readonly double _p;
    private readonly Lock _lockObject = new ();
    private bool _isTouched;
    public bool IsRunning { get; private set; }

    public Crystal1D(int n, int k, double p)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(n, 1);
        _cells = new int[n];
        
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(k, 0);
        _k = k;
        _cells[0] = k;

        if (p is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(p), p, "p must be between 0 and 1");
        }
        _p = p;
    }

    public void Reset()
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("Cannot reset while running. Stop the BrownianMotion first.");
        }
        
        _cells[0] = _k;
        for (var i = 1; i < _cells.Length; i++)
        {
            _cells[i] = 0;
        }
        _isTouched = false;
    }

    public Task StartBrownianMotion(TimeSpan modellingTime, CancellationToken cancellationToken)
    {
        if (_isTouched)
        {
            throw new InvalidOperationException("Reset the crystal first.");
        }
        _isTouched =  true;
        IsRunning = true;
        var tasks = new Task[_k];
        for (var i = 0; i < _k; i++)
        {
            tasks[i] = Task.Run(() => StartBrownianMotionForAtom(modellingTime, cancellationToken), cancellationToken);
        }
        
        // In order not to throw TaskCancelledException.
        // ReSharper disable once MethodSupportsCancellation
        return Task.WhenAll(tasks).ContinueWith(_ => IsRunning = false);
    }

    private Task StartBrownianMotionForAtom(TimeSpan modellingTime, CancellationToken cancellationToken)
    {
        var cellIndex = 0;
        var startTime = DateTime.Now;
        while (!cancellationToken.IsCancellationRequested 
               && (DateTime.Now - startTime < modellingTime 
                    || modellingTime == Timeout.InfiniteTimeSpan))
        {
            var m = Random.Shared.NextDouble();
            if (m > _p)
            {
                if (cellIndex != _cells.Length - 1)
                {
                    lock (_lockObject)
                    {
                        _cells[cellIndex]--;
                        _cells[cellIndex + 1]++;
                    }
                    cellIndex++;
                }
            }
            else if (cellIndex != 0)
            {
                lock (_lockObject)
                {
                    _cells[cellIndex]--;
                    _cells[cellIndex - 1]++;
                }
                cellIndex--;
            }
        }
        return Task.CompletedTask;
    }

    public Crystal1DSnapshot CreateSnapshot()
    {
        var newCells = new int[_cells.Length];
        
        lock (_lockObject)
        {
            _cells.CopyTo(newCells, 0);
        }
        
        return new Crystal1DSnapshot(newCells, _k, _p, _isTouched);
    }
}
