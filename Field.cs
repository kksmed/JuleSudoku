namespace JuleSudoku;

internal class Field
{
    public Field(Point point, int? value = null)
    {
        Point = point;
        Value = value;
        IsLocked = value.HasValue;
    }

    public Point Point { get; }
    public int? Value { get; private set; }
    
    public bool HasValue => Value.HasValue;

    public bool IsLocked { get; }

    public void Set(int newValue)
    {
        if (IsLocked)
            throw new InvalidOperationException($"Field ({Point}) is predetermined and cannot be set.");
        Value = newValue;
    }

    public void Reset()
    {
        if (IsLocked)
            throw new ArgumentException($"Field ({Point})is predetermined and cannot be reset.");
        Value = null;
    }
}