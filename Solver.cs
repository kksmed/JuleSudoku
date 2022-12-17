using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace JuleSudoku;

internal static class Solver
{
    private static readonly ConcurrentDictionary<Field, int> Updates = new();

    private static Board? _solution;
    
    private static (ParallelOptions, CancellationTokenSource) SetupParallelism()
    {
        var cts = new CancellationTokenSource();
        var po = new ParallelOptions
        {
            CancellationToken = cts.Token,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
        Console.WriteLine($"Running in parallel: {Environment.ProcessorCount}");

        return (po, cts);
    }
    
    public static Board? Solve(Board board)
    {
        var predeterminedValues = board.Locked.Select(board.GetField);
        var availableValues = Enumerable.Range(1, 25).ToList();
        availableValues.RemoveAll(x => predeterminedValues.Contains(x));

        // Initialize Updates
        for (var row = 0; row < Board.Size; row++)
        for (var column = 0; column < Board.Size; column++)
            Updates.TryAdd(new Field(row, column), 0);

        using var tokenSource = new CancellationTokenSource();
        var po = new ParallelOptions
        {
            CancellationToken = tokenSource.Token,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
        Console.WriteLine($"Running in parallel: {Environment.ProcessorCount}");

        try
        {
            TrySolve(board, new Field(0, 0), availableValues.ToImmutableArray(), tokenSource, po);
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e.Message);
        }

        return _solution;
    }

    private static void TrySolve(Board board, Field field, ImmutableArray<int> availableValues,
        CancellationTokenSource tokenSource, ParallelOptions parallelOptions)
    {
        var subTasks = new List<Task<Board?>>(availableValues.Length);

        Parallel.For(0, availableValues.Length, parallelOptions, i =>
        {
            // Run backwards as we want to test biggest numbers first.
            var value = availableValues[availableValues.Length - 1 - i];
            var newBoard = board.SetField(value, field);
            if (!Validator.ValidateField(newBoard, field, availableValues.Take(5).ToList()))
                return;

            Updates.AddOrUpdate(field, 0, (_, x) => x + 1); // Always updates - '0' will never be added.

            var nextField = FindNextField(newBoard, field);
            var remainingValues = availableValues.RemoveAt(i);
            if (!nextField.HasValue)
            {
                _solution = newBoard;
                tokenSource.Cancel();
                return;
            }

            TrySolve(newBoard, nextField.Value, remainingValues, tokenSource, parallelOptions);
        });
    }

    private static Field? FindNextField(Board board, Field field)
    {
        var nextRow = field.Row;
        var nextColumn = field.Column;
        while (true)
        {
            nextColumn++;
            if (nextColumn >= Board.Size)
            {
                nextColumn = 0;
                nextRow++;
                
                if (nextRow >= Board.Size)
                    return null;
            }

            var nextField = new Field(nextRow, nextColumn);
            if (board.Locked.Contains(nextField))
                continue;

            return nextField;
        }
    }
}