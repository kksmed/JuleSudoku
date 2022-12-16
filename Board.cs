namespace JuleSudoku;

internal class Board
{
    public const int Size = 5;
    
    public int?[][] Rows { get; }
    public int?[][] Columns { get; }
    public int?[][] Diagonals { get; }

    public Board()
    {
        Rows = new[]
        {
            new int?[] { null, null, 1, null, 7 },
            new int?[] { 16, null, null, null, 3 },
            new int?[] { null, 5, 18, null, null },
            new int?[] { null, 21, null, null, null },
            new int?[] { null, null, null, null, 11 },
        };

        Columns  = new[]
        {
            new int?[] { null, 16, null, null, null },
            new int?[] { null, null, 5, 21, null },
            new int?[] { 1, null, 18, null, null },
            new int?[] { null, null, null, null, null },
            new int?[] { 7, 3, null, null, 11 },
        };
        Diagonals  = new[]
        {
            new int?[] { null, null, null, null, 11 },
            new int?[] { null, 21, 18, null, 7 },
        };
    }

    public void SetField(int value, Field field)
    {
        Rows[field.Row][field.Column] = value;
        Columns[field.Column][field.Row] = value;

        if (field.TryGetDiagonal(out var diagonal))
            Diagonals[diagonal.Value][field.Column] = value;
    }

    public void ResetField(Field field)
    {
        Rows[field.Row][field.Column] = null;
        Columns[field.Column][field.Row] = null;

        if (field.TryGetDiagonal(out var diagonal))
            Diagonals[diagonal.Value][field.Column] = null;
        
    }
}