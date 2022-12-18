namespace JuleSudoku;

internal static class Solver
{
    public static int Updates { get; private set; }
    
    public static bool Solve(Board board)
    {
        Updates = 0;
        var predeterminedValues = board.Locked.Select(x => x.Value!.Value);
        var availableValues = Enumerable.Range(1, 25).Where(x => !predeterminedValues.Contains(x)).ToList();

        return TrySolveNextLine(board, availableValues);
    }

    private static bool TrySolveNextLine(Board board, List<int> availableValues, int linesSolved = 0)
    {
        var line = FindNextLine(board, linesSolved);
        if (line == null)
        {
            Console.WriteLine($"Solution after updates: {Updates}");
            Console.WriteLine(board.ToString());

            return false; // We want to find all the solutions
        }
        return TrySolveLine(board, line, availableValues, linesSolved);
    }

    private static bool TrySolveLine(Board board, Field[] line, List<int> availableValues, int linesSolved)
    {
        var undeterminedFields = new List<Field>(Board.Size);
        var existingSum = 0;
        foreach (var field in line)
        {
            if (field.HasValue)
                existingSum += field.Value!.Value;
            else
                undeterminedFields.Add(field);
        }

        var subsetWithSpecifiedSum =
            FindSubsetWithSpecifiedSum(new Stack<int>(availableValues), undeterminedFields.Count,
                    Validator.ExpectedSum - existingSum)
                .ToList();
        var suitableValuesForLine =
            subsetWithSpecifiedSum
                .SelectMany(x => x).Distinct().OrderBy(x => x).ToList(); // TODO: Maybe bad idea to fatten this out?

        return TrySolveRestOfLine(board, new Stack<Field>(undeterminedFields), suitableValuesForLine,
            availableValues, linesSolved);
    }

    private static bool TrySolveRestOfLine(Board board, Stack<Field> restOfline, List<int> lineAvailableValues,
        List<int> allAvailableValues, int linesSolved)
    {
        if (!restOfline.Any()) return TrySolveNextLine(board, allAvailableValues, linesSolved + 1);
        if (!lineAvailableValues.Any()) return false;

        var field = restOfline.Pop();

        for (var i = 0; i < lineAvailableValues.Count; i++)
        {
            var value = lineAvailableValues[i];
            field.Set(value);
            Updates++;
            
            if (!Validator.ValidateField(board, field.Point, allAvailableValues.Take(5).ToList())) 
                continue;

            // We need the index to insert it back in the correct place.
            var allListIndex = allAvailableValues.FindIndex(i, x => x == value);
            if (allListIndex < 0)
                throw new InvalidOperationException($"Value not found in `{nameof(allAvailableValues)}`");
            
            lineAvailableValues.RemoveAt(i);
            allAvailableValues.RemoveAt(allListIndex);
            
            if (TrySolveRestOfLine(board, restOfline, lineAvailableValues, allAvailableValues, linesSolved))
                return true;
            
            lineAvailableValues.Insert(i, value);
            allAvailableValues.Insert(allListIndex, value);
        }
        
        field.Reset();
        restOfline.Push(field);
        return false;
    }

    private static IEnumerable<List<int>> FindSubsetWithSpecifiedSum(Stack<int> values, int amount, int sum)
    {
        if (amount < 1)
            throw new ArgumentException("Amount is too low.", nameof(amount));
        
        if (amount == 1)
        {
            if (values.Contains(sum))
                yield return new List<int> { sum };
            yield break;
        }

        while (values.Any())
        {
            int? value = values.Pop();

            if (value >= sum)
                continue;
            
            var rest = FindSubsetWithSpecifiedSum(new Stack<int>(values), amount - 1, sum - value.Value);
            foreach (var otherValues in rest)
            {
                otherValues.Add(value.Value);
                yield return otherValues;
            }
        }
    }

    private static readonly List<Field[]> LineCache = new(11);

    private static Field[]? FindNextLine(Board board, int n)
    {
        if (LineCache.Count >= n + 1)
            return LineCache[n];
        
        var lineWithManyNumbers = board.Rows.Concat(board.Columns).Concat(board.Diagonals)
            .Where(x => x.Any(y => !y.HasValue)).MinBy(x => x.Count(y => !y.HasValue));
        // TODO: Diff to mid: (Abs(65/2-SUM)) ?

        if (lineWithManyNumbers == null)
            return null;

        LineCache.Add(lineWithManyNumbers);
        return lineWithManyNumbers;
    }
}