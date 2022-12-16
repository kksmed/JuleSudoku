namespace JuleSudoku;

internal static class Validator
{
    /// <summary>
    /// 65.
    /// </summary>
    private static readonly int ExpectedSum = Enumerable.Range(1, Board.Size * Board.Size).Sum() / Board.Size;

    public static bool ValidateBoard(Board board)
        => ValidateLocked(board) && ValidateRows(board) && ValidateColumns(board) && ValidatesDiagonals(board);

    public static bool ValidateField(Board board, Field field, List<int> lowestAvailableValues) =>
        ValidateLine(board.Rows[field.Row], lowestAvailableValues) && 
        ValidateLine(board.Columns[field.Column], lowestAvailableValues) &&
        board.GetDiagonals(field).All(x => ValidateLine(x, lowestAvailableValues));

    private static bool ValidateRows(Board board) => board.Rows.All(x => ValidateLine(x, Enumerable.Range(1, 5)));

    private static bool ValidateColumns(Board board) => board.Columns.All(x => ValidateLine(x, Enumerable.Range(1, 5)));

    private static bool ValidatesDiagonals(Board board) =>
        board.Diagonals.All(x => ValidateLine(x, Enumerable.Range(1, 5)));
    
    private static bool ValidateLine(int?[] line, IEnumerable<int> lowestAvailableValues)
    {
        var sum = line.Sum();
        if (sum > ExpectedSum) return false;

        var unassignedMinSum = lowestAvailableValues.Take(line.Count(x => !x.HasValue)).Sum();
        if (sum + unassignedMinSum > ExpectedSum) return false;
        
        return true;
    }

    private static bool ValidateLocked(Board board) => board.Locked.All(x => board.Rows[x.Row][x.Column].HasValue);
}