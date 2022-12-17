using System.Collections.Immutable;

namespace JuleSudoku;

internal static class Solver
{
    private static readonly Dictionary<Field, int> Updates = new();

    public static ulong DeadEnds { get; private set; }

    public static bool Solve(Board board)
    {
        var predeterminedValues = board.Locked.Select(board.GetField);
        var availableValues = Enumerable.Range(1, 25).ToList();
        availableValues.RemoveAll(x => predeterminedValues.Contains(x));

        // Initialize Updates
        for (var row = 0; row < Board.Size; row++)
        for (var column = 0; column < Board.Size; column++)
            Updates.Add(new Field(row, column), 0);

        return TrySolve(board, new Field(0, 0), availableValues.ToImmutableArray());
    }

    private static bool TrySolve(Board board, Field field, ImmutableArray<int> availableValues)
    {
        // Run backwards as we want to test biggest numbers first.
        for (var i = availableValues.Length - 1; i >= 0; i--)
        {
            var value = availableValues[i];
            board.SetField(value, field);
            if (!Validator.ValidateField(board, field, availableValues.Take(5).ToList())) 
                continue;

            Updates[field]++;
            
            var nextField = FindNextField(board, field);
            var remainingValues = availableValues.RemoveAt(i);
            if (!nextField.HasValue || TrySolve(board, nextField.Value, remainingValues))
                return true;
        }
        board.ResetField(field);
        DeadEnds++;
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