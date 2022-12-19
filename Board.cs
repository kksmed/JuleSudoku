using System.Globalization;

namespace JuleSudoku;

internal class Board
{
    public const int Size = 5;
    
    public Field[][] Rows { get; }
    public Field[][] Columns { get; }
    public Field[][] Diagonals { get; }
    public Field[] Locked { get; }
    
    public Board(params Field[] predeterminedFields)
    {
        Rows = InitializeRows(predeterminedFields);
        Columns = InitializeColumns();
        Diagonals = InitializeDiagonals();
        Locked = predeterminedFields;
    }

    private static Field[][] InitializeRows(Field[] predeterminedFields)
    {
        var listOfRows = new List<Field[]>(Size);
        for (var row = 0; row < Size; row++)
        {
            var rowList = new List<Field>(Size);
            for (var column = 0; column < Size; column++)
            {
                var point = new Point(row, column);
                var predetermined = predeterminedFields.FirstOrDefault(x => x.Point == point);
                var field = predetermined ?? new Field(point);
                rowList.Add(field);
            }

            listOfRows.Add(rowList.ToArray());
        }

        return listOfRows.ToArray();
    }
    
    private Field[][] InitializeColumns()
    {
        var listOfColumns = new List<Field[]>(Size);
        for (var column = 0; column < Size; column++)
        {
            var columnList = new List<Field>(Size);
            for (var row = 0; row < Size; row++)
            {
                var field = Rows[row][column];
                columnList.Add(field);
            }

            listOfColumns.Add(columnList.ToArray());
        }

        return listOfColumns.ToArray();
    }

    private Field[][] InitializeDiagonals()
    {
        var listOfDiagonals = new List<Field[]>(2);
        for (var d = 0; d < 2; d++)
        {
            var diagonalList = new List<Field>(Size);
            for (var i = 0; i < Size; i++)
            {
                var row = d == 0 ? i : Size - 1 - i;
                var field = Rows[row][i];
                diagonalList.Add(field);
            }

            listOfDiagonals.Add(diagonalList.ToArray());
        }

        return listOfDiagonals.ToArray();
    }

    public Field this[Point point] => Rows[point.Row][point.Column];

    public IEnumerable<Field[]> GetDiagonals(Point point)
    {
        if (point.Row == point.Column)
            yield return Diagonals[0];

        if (point.Row == Size - 1 - point.Column)
            yield return Diagonals[1];
    }

    public override string ToString() => Printer.FieldsToString(Rows);
}