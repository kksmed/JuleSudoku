using System.Collections.Immutable;

namespace JuleSudoku;

internal static class Solver
{
    private static Board? _solution;

    public static Board? Solve(Board board)
    {
        var predeterminedValues = board.Locked.Select(board.GetField);
        var availableValues = Enumerable.Range(1, 25).ToList();
        availableValues.RemoveAll(x => predeterminedValues.Contains(x));

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
        Parallel.For(0, availableValues.Length, parallelOptions, i =>
        {
            // Run backwards as we want to test biggest numbers first.
            var value = availableValues[availableValues.Length - 1 - i];
            var newBoard = board.SetField(value, field);
            if (!Validator.ValidateField(newBoard, field, availableValues.Take(5).ToList()))
                return;

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