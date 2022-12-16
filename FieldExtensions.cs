namespace JuleSudoku;

internal static class FieldExtensions
{
    public static Diagonals GetDiagonals(this Field field)
    {
        var diagonals = Diagonals.None;

        if (field.Row == field.Column)
        {
            diagonals |= Diagonals.BottomLeftToTopRight;
        }

        if (field.Row == Board.Size - field.Column)
        {
            diagonals |= Diagonals.TopLeftToBottomRight;
        }

        return diagonals;
    }
}