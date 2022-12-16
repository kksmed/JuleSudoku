// See https://aka.ms/new-console-template for more information

using JuleSudoku;

var board = new Board();

var initialValidation = Validator.ValidateBoard(board);

if (!initialValidation)
    throw new InvalidOperationException("Invalid initial board.");

Solver.Solve(board);

Console.ReadKey();