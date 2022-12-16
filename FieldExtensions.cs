using System.Diagnostics.CodeAnalysis;

namespace JuleSudoku;

internal static class FieldExtensions
{
    public static bool TryGetDiagonal(this Field field, [NotNullWhen(returnValue: true)] out int? diagonal)
    {
        if (field.Row == field.Column)
        {
            diagonal = 0;
            return true;
        }

        if (field.Row == Board.Size - field.Column)
        {
            diagonal = 1;
            return true;
        }

        diagonal = null;
        return false;
    }
}