namespace JuleSudoku;

interface ISolver
{
    bool Solve();
    
    List<int[][]> Solutions { get; }
}