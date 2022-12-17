using System.Collections.Immutable;

namespace JuleSudoku;

internal static class Solver
{
    private static readonly Dictionary<Field, int> Updates = new();

    public static ulong DeadEnds { get; private set; }

    public static void Solve(Board board)
    {
        var predeterminedValues = board.Locked.Select(board.GetField);
        var availableValues = Enumerable.Range(1, 25).Reverse().ToList();
        availableValues.RemoveAll(x => predeterminedValues.Contains(x));

        // Initialize Updates
        for (var row = 0; row < Board.Size; row++)
        for (var column = 0; column < Board.Size; column++)
            Updates.Add(new Field(row, column), 0);

        Console.WriteLine(TrySolve(board, new Field(0, 0), availableValues.ToImmutableArray()) ? "Solved!" : "Failure!");
    }

    private static bool TrySolve(Board board, Field field, ImmutableArray<int> availableValues)
    {
        for (var i = 0; i < availableValues.Length; i++)
        {
            var value = availableValues[i];
            board.SetField(value, field);
            if (!Validator.ValidateField(board, field, availableValues.TakeLast(5).Reverse().ToList())) 
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