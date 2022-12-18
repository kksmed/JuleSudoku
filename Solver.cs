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
        for (var i = 0; i < availableValues.Count; i++)
        {
            var value = availableValues[i];
            field.Set(value);
            if (!Validator.ValidateField(board, field.Point, availableValues.Take(5).ToList())) 
                continue;

            var nextField = FindNextField(board);
            availableValues.RemoveAt(i);
            if (nextField == null || TrySolve(board, nextField, availableValues))
                return true;
            
            availableValues.Insert(i, value);
        }
        
        field.Reset();
        return false;
    }

    private static Stack<Field> _nextQueue = new (0);
    
    private static Field? FindNextField(Board board)
    {
        if (_nextQueue.Any())
            return _nextQueue.Pop();

        var lineWithManyNumbers = board.Rows.Concat(board.Columns).Concat(board.Diagonals)
            .Where(x => x.Any(y => !y.HasValue)).MinBy(x => x.Count(y => !y.HasValue));

        _nextQueue = new Stack<Field>(lineWithManyNumbers?.Where(x => !x.HasValue) ?? Enumerable.Empty<Field>());
        return _nextQueue.Any() ? _nextQueue.Pop() : null;
    }
}