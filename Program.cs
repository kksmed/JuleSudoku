// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using JuleSudoku;

var initialBoard = Board.Create(
    (1, new Field(0, 2)),
    (7, new Field(0, 4)),
    (16, new Field(1, 0)),
    (3, new Field(1, 4)),
    (5, new Field(2, 1)),
    (18, new Field(2, 2)),
    (21, new Field(3, 1)),
    (11, new Field(4, 4)));

Console.Write(initialBoard.ToString());
Console.WriteLine($"{Environment.NewLine}<Press any key to continue>");

Console.WriteLine(" ");
Console.ReadKey();

Console.WriteLine(" ");

var initialValidation = Validator.ValidateBoard(initialBoard);

if (!initialValidation)
    throw new InvalidOperationException("Invalid initial board.");

Console.WriteLine($"Start solving ({DateTime.Now})...");
Console.WriteLine(" ");

var stopWatch = Stopwatch.StartNew();

var result = Solver.Solve(initialBoard);

stopWatch.Stop();

Console.WriteLine(result == null ? "Failure!" : "Solved!");

Console.WriteLine($"Time: {stopWatch.Elapsed}");

Console.Write(result?.ToString());
