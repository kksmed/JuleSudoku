namespace JuleSudoku;

internal static class Validator
{
    /// <summary>
    /// 65.
    /// </summary>
    private static readonly int ExpectedSum = Enumerable.Range(1, Board.Size * Board.Size).Sum() / Board.Size;

    public static bool ValidateBoard(Board board)
        => ValidateLocked(board) && ValidateRows(board) && ValidateColumns(board) && ValidateDiagonals(board) && ValidateInitialBoard(board);

    private static bool ValidateInitialBoard(Board board)
    {
        for (var row = 0; row < Board.Size; row++)
        for (var column = 0; column < Board.Size; column++)
            if (board.Rows[row][column] != board.Columns[column][row] ||
                board.GetDiagonals(new Point(row, column)).Any(x => x[column] != board.Rows[row][column]))
                return false;
        
        return true;
    }

    public static bool ValidateField(Board board, Point point, List<int> lowestAvailableValues) =>
        ValidateLine(board.Rows[point.Row], lowestAvailableValues) && 
        ValidateLine(board.Columns[point.Column], lowestAvailableValues) &&
        board.GetDiagonals(point).All(x => ValidateLine(x, lowestAvailableValues));

    private static bool ValidateRows(Board board) => board.Rows.All(x => ValidateLine(x, Enumerable.Range(1, 5)));

    private static bool ValidateColumns(Board board) => board.Columns.All(x => ValidateLine(x, Enumerable.Range(1, 5)));

    private static bool ValidateDiagonals(Board board) =>
        board.Diagonals.All(x => ValidateLine(x, Enumerable.Range(1, 5)));
    
    private static bool ValidateLine(Field[] line, IEnumerable<int> lowestAvailableValues)
    {
        var sum = line.Select(x => x.Value).Sum();
        if (sum > ExpectedSum) return false;

        var unassignedMinSum = lowestAvailableValues.Take(line.Count(x => !x.Value.HasValue)).Sum();
        if (sum + unassignedMinSum > ExpectedSum) return false;
        
        return true;
    }

    private static bool ValidateLocked(Board board) => board.Locked.All(x => x.IsLocked);
}