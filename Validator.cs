namespace JuleSudoku;

internal static class Validator
{
    /// <summary>
    /// 65.
    /// </summary>
    private static readonly int ExpectedSum = Enumerable.Range(1, Board.Size * Board.Size).Sum() / Board.Size;

    public static bool ValidateBoard(Board board)
        => ValidateLocked(board) && ValidateRows(board) && ValidateColumns(board) && ValidatesDiagonals(board);

    public static bool ValidateField(Board board, Field field) =>
        ValidateLine(board.Rows[field.Row]) && 
        ValidateLine(board.Columns[field.Column]) &&
        board.GetDiagonals(field).All(ValidateLine);

    private static bool ValidateRows(Board board) => board.Rows.All(ValidateLine);

    private static bool ValidateColumns(Board board) => board.Columns.All(ValidateLine);

    private static bool ValidatesDiagonals(Board board) => board.Diagonals.All(ValidateLine);
    
    private static bool ValidateLine(int?[] line)
    {
        var sum = line.Sum();
        if (sum > ExpectedSum) return false;

        var i = 1;
        var unassignedMinSum = line.Where(x => !x.HasValue).Select(_ => i++).Sum();
        if (sum + unassignedMinSum > ExpectedSum) return false;
        
        return true;
    }

    private static bool ValidateLocked(Board board) => board.Locked.All(x => board.Rows[x.Row][x.Column].HasValue);
}