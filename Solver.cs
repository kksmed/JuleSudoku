using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace JuleSudoku;

internal static class Solver
{
    private static readonly ConcurrentDictionary<Field, int> Updates = new();
    
    private static ulong _deadEnds;
    public static ulong DeadEnds => _deadEnds;

    public static async Task<bool> Solve(Board board)
    {
        var predeterminedValues = board.Locked.Select(board.GetField);
        var availableValues = Enumerable.Range(1, 25).ToList();
        availableValues.RemoveAll(x => predeterminedValues.Contains(x));

        // Initialize Updates
        for (var row = 0; row < Board.Size; row++)
        for (var column = 0; column < Board.Size; column++)
            Updates.TryAdd(new Field(row, column), 0);

        var result = TrySolve(board, new Field(0, 0), availableValues.ToImmutableArray());
        return await result;
    }

    private static async Task<bool> TrySolve(Board board, Field field, ImmutableArray<int> availableValues)
    {
        // Run backwards as we want to test biggest numbers first.
        for (var i = availableValues.Length - 1; i >= 0; i--)
        {
            var value = availableValues[i];
            var newBoard = board.SetField(value, field);
            if (!Validator.ValidateField(newBoard, field, availableValues.Take(5).ToList())) 
                continue;

            Updates.AddOrUpdate(field, 0, (_, x) => x + 1); // Always updates - '0' will never be added.
            
            var nextField = FindNextField(newBoard, field);
            var remainingValues = availableValues.RemoveAt(i);
            if (!nextField.HasValue || await TrySolve(newBoard, nextField.Value, remainingValues))
                return true;
        }
        Interlocked.Increment(ref _deadEnds);
        return false;
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