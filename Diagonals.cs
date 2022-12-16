namespace JuleSudoku;

[Flags]
internal enum Diagonals
{
    None = 0,
    TopLeftToBottomRight = 1,
    BottomLeftToTopRight = 2,
}