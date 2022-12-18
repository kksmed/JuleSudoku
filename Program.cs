// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using JuleSudoku;

var board = new Board(
    new Field(new Point(0, 2), 1),
    new Field(new Point(0, 4), 7),
    new Field(new Point(1, 0), 16),
    new Field(new Point(1, 4), 3),
    new Field(new Point(2, 1), 5),
    new Field(new Point(2, 2), 18),
    new Field(new Point(3, 1), 21),
    new Field(new Point(4, 4), 11));

Console.WriteLine(board.ToString());
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

var result = Solver.Solve(board);

stopWatch.Stop();

Console.WriteLine(result ? "Solved!" : "Failure!");

Console.WriteLine($"Time: {stopWatch.Elapsed}");
Console.WriteLine($"Updates in total: {Solver.Updates}");
