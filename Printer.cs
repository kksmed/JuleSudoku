using System.Globalization;

namespace JuleSudoku;

static class Printer
{
    static string NumbersToString(IEnumerable<IEnumerable<int?>> numbers)
    {
        return string.Join(Environment.NewLine,
            numbers.Select(x =>
                string.Join(" | ", x.Select(y => (y?.ToString(CultureInfo.InvariantCulture) ?? "").PadLeft(2)))));
    }

    public static string NumbersToString(int[][] numbers)
        => NumbersToString(numbers.Select(x => x.Select(y => (int?)y)));

    public static string FieldsToString(Field[][] fields) =>
        NumbersToString(fields.Select(x => x.Select(y => y.Value)));
}