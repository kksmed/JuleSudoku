namespace JuleSudoku;

internal static class Solver
{
    public static bool Solve(Board board)
    {
        var predeterminedValues = board.Locked.Select(x => x.Value!.Value);
        var availableValues = Enumerable.Range(1, 25).ToList();
        availableValues.RemoveAll(x => predeterminedValues.Contains(x));

        return TrySolve(board, board[new Point(0, 0)], availableValues);
    }

    private static bool TrySolve(Board board, Field field, List<int> availableValues)
    {
        // Run backwards as we want to test biggest numbers first.
        for (var i = availableValues.Count - 1; i >= 0; i--)
        {
            var value = availableValues[i];
            field.Set(value);
            if (!Validator.ValidateField(board, field.Point, availableValues.Take(5).ToList())) 
                continue;

            var nextField = FindNextField(board, field.Point);
            availableValues.RemoveAt(i);
            if (nextField == null || TrySolve(board, nextField, availableValues))
                return true;
            
            availableValues.Insert(i, value);
        }
        
        field.Reset();
        return false;
    }

    private static Field? FindNextField(Board board, Point point)
    {
        var nextRow = point.Row;
        var nextColumn = point.Column;
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

            var nextPoint = new Point(nextRow, nextColumn);
            var nextField = board[nextPoint];
            if (nextField.IsLocked)
                continue;

            return nextField;
        }
    }
}