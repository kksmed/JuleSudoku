// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using JuleSudoku;

var board = new Board(
    (1, new Field(0, 2)),
    (7, new Field(0, 4)),
    (16, new Field(1, 0)),
    (3, new Field(1, 4)),
    (5, new Field(2, 1)),
    (18, new Field(2, 2)),
    (21, new Field(3, 1)),
    (11, new Field(4, 4)));

Console.Write(board.ToString());
Console.WriteLine($"{Environment.NewLine}<Press any key to continue>");

Console.WriteLine(" ");
Console.ReadKey();

Console.WriteLine(" ");

var initialValidation = Validator.ValidateBoard(board);

if (!initialValidation)
    throw new InvalidOperationException("Invalid initial board.");

Console.WriteLine($"Start solving ({DateTime.Now})...");
Console.WriteLine(" ");

var stopWatch = Stopwatch.StartNew();

Solver.Solve(board);

stopWatch.Stop();

Console.WriteLine($"Solved in: {stopWatch.Elapsed}");

Console.Write(board.ToString());
Console.ReadKey();