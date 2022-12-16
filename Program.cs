// See https://aka.ms/new-console-template for more information

using JuleSudoku;

var board = new Board();

var initialValidation = Validator.ValidateBoard(board);

Console.WriteLine(initialValidation ? "Passed initial validation." : "Impossible!");

Solver.Solve(board);

Console.ReadKey();