// See https://aka.ms/new-console-template for more information

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
Console.ReadKey();

var initialValidation = Validator.ValidateBoard(board);

if (!initialValidation)
    throw new InvalidOperationException("Invalid initial board.");

Solver.Solve(board);

Console.ReadKey();