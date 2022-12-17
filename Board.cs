using System.Collections.Immutable;
using System.Globalization;

namespace JuleSudoku;

internal class Board
{
    public const int Size = 5;
    
    public ImmutableArray<ImmutableArray<int?>> Rows { get; }
    public ImmutableArray<ImmutableArray<int?>> Columns { get; }
    public ImmutableArray<ImmutableArray<int?>> Diagonals { get; }
    public ImmutableArray<Field> Locked { get; }

    private Board(ImmutableArray<ImmutableArray<int?>> rows, ImmutableArray<ImmutableArray<int?>> columns,
        ImmutableArray<ImmutableArray<int?>> diagonals, ImmutableArray<Field> locked)
    {
        Rows = rows;
        Columns = columns;
        Diagonals = diagonals;
        Locked = locked;
    }

    public static Board Create(params (int value, Field field)[] predeterminedFields)
    {
        var rows = Enumerable.Range(0, Size).Select(_ => Enumerable.Repeat<int?>(null, Size).ToImmutableArray())
            .ToImmutableArray();
        var columns = Enumerable.Range(0, Size).Select(_ => Enumerable.Repeat<int?>(null, Size).ToImmutableArray())
            .ToImmutableArray();
        var diagonals = Enumerable.Range(0, 2).Select(_ => Enumerable.Repeat<int?>(null, Size).ToImmutableArray())
            .ToImmutableArray();
        
        var locked = predeterminedFields.Select(x => x.field).ToImmutableArray();
        
        var board = new Board(rows, columns, diagonals, locked);
        
        foreach (var (value, field) in predeterminedFields) 
            board = board.InternalSetField(value, field);
        
        return board;
    }

    public Board SetField(int value, Field field)
    {
        if (Locked.Contains(field))
            throw new ArgumentException("Field is predetermined and cannot be set.", nameof(field));

        return InternalSetField(value, field);
    }

    private Board InternalSetField(int value, Field field)
    {
        var rows = Rows.SetItem(field.Row, Rows[field.Row].SetItem(field.Column, value));
        var columns = Columns.SetItem(field.Column, Columns[field.Column].SetItem(field.Row, value));

        var diagonals = GetDiagonalIndexes(field).Aggregate(Diagonals,
            (current, diagonal) => current.SetItem(diagonal, Diagonals[diagonal].SetItem(field.Column, value)));
        return new Board(rows, columns, diagonals, Locked);
    }

    public int GetField(Field field)
    {
        var value = Rows[field.Row][field.Column];
        
        if (!value.HasValue) throw new ArgumentException("Field has no value.", nameof(field));
        
        return value.Value;
    }
    
    private static IEnumerable<int> GetDiagonalIndexes(Field field)
    {
        if (field.Row == field.Column)
            yield return 0;

        if (field.Row == Size - 1 - field.Column)
            yield return 1;
    }

    public IEnumerable<ImmutableArray<int?>> GetDiagonals(Field field)
    {
        var indexes = GetDiagonalIndexes(field);
        return Diagonals.Where((_, i) => indexes.Contains(i));
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
            Rows.Select(x =>
                string.Join(" | ", x.Select(y => (y?.ToString(CultureInfo.InvariantCulture) ?? "").PadLeft(2)))));
    }
}