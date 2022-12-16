using System.Globalization;

namespace JuleSudoku;

internal class Board
{
    public const int Size = 5;
    
    public int?[][] Rows { get; }
    public int?[][] Columns { get; }
    public int?[][] Diagonals { get; }
    public Field[] Locked { get; }
        
    public Board(params (int value, Field field)[] predeterminedFields)
    {
        Rows = Enumerable.Range(0, Size).Select(_ => Enumerable.Repeat<int?>(null, Size).ToArray()).ToArray();
        Columns = Enumerable.Range(0, Size).Select(_ => Enumerable.Repeat<int?>(null, Size).ToArray()).ToArray();
        Diagonals = Enumerable.Range(0, 2).Select(_ => Enumerable.Repeat<int?>(null, Size).ToArray()).ToArray();
        
        foreach (var (value, field) in predeterminedFields) 
            InternalSetField(value, field);
        
        Locked = predeterminedFields.Select(x => x.field).ToArray();
    }

    public void SetField(int value, Field field)
    {
        if (Locked.Contains(field))
            throw new ArgumentException("Field is predetermined and cannot be set.", nameof(field));

        InternalSetField(value, field);
    }

    private void InternalSetField(int value, Field field)
    {
        Rows[field.Row][field.Column] = value;
        Columns[field.Column][field.Row] = value;

        foreach (var diagonal in GetDiagonals(field))
            diagonal[field.Column] = value;
    }

    public int GetField(Field field)
    {
        var value = Rows[field.Row][field.Column];
        
        if (!value.HasValue) throw new ArgumentException("Field has no value.", nameof(field));
        
        return value.Value;
    }

    public void ResetField(Field field)
    {
        if (Locked.Contains(field))
            throw new ArgumentException("Field is predetermined and cannot be reset.", nameof(field));

        Rows[field.Row][field.Column] = null;
        Columns[field.Column][field.Row] = null;

        foreach (var diagonal in GetDiagonals(field))
            diagonal[field.Column] = null;
    }
    
    public IEnumerable<int?[]> GetDiagonals(Field field)
    {
        if (field.Row == field.Column)
            yield return Diagonals[0];

        if (field.Row == Size - 1 - field.Column)
            yield return Diagonals[1];
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
            Rows.Select(x =>
                string.Join(" | ", x.Select(y => (y?.ToString(CultureInfo.InvariantCulture) ?? "").PadLeft(2)))));
    }
}