namespace JuleSudoku;

class Solver : ISolver
{
    readonly Board _board;
    readonly bool _stopAtFirstSolution;
    readonly List<Field[]> _lineCache = new(Board.Size * 2 + 1);

    public int Updates { get; private set; }

    public List<int[][]> Solutions { get; }

    public Solver(Board board, bool stopAtFirstSolution = false)
    {
        _board = board;
        _stopAtFirstSolution = stopAtFirstSolution;
        Updates = 0;
        Solutions = new List<int[][]>();
    }

    public bool Solve()
    {
        var predeterminedValues = _board.Locked.Select(x => x.Value!.Value);
        var availableValues = Enumerable.Range(1, 25).Where(x => !predeterminedValues.Contains(x)).ToList();

        SolveNextLine(availableValues);
        return Solutions.Any();
    }

    bool SolveNextLine(List<int> availableValues, int linesSolved = 0)
    {
        var line = FindNextLine(linesSolved);
        if (line == null)
        {
            Solutions.Add(_board.Rows.Select(x => x.Select(y => y.Value!.Value).ToArray()).ToArray());
            return _stopAtFirstSolution; // We want to find all the solutions
        }
        return TrySolveLine(line, availableValues, linesSolved);
    }

    bool TrySolveLine(Field[] line, List<int> availableValues, int linesSolved)
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

        return subsetWithSpecifiedSum
            .Select(x => TrySolveRestOfLine(new Stack<Field>(undeterminedFields), x, availableValues, linesSolved))
            .FirstOrDefault(x => x, false);
    }

    bool TrySolveRestOfLine(Stack<Field> restOfline, List<int> lineAvailableValues, List<int> allAvailableValues,
        int linesSolved)
    {
        if (!restOfline.Any()) return SolveNextLine(allAvailableValues, linesSolved + 1);
        if (!lineAvailableValues.Any()) return false;

        var field = restOfline.Pop();

        for (var i = 0; i < lineAvailableValues.Count; i++)
        {
            var value = lineAvailableValues[i];
            field.Set(value);
            Updates++;

            if (!Validator.ValidateField(_board, field.Point, allAvailableValues.Take(5).ToList()))
                continue;

            // We need the index to insert it back in the correct place.
            var allListIndex = allAvailableValues.FindIndex(i, x => x == value);
            if (allListIndex < 0)
                throw new InvalidOperationException($"Value not found in `{nameof(allAvailableValues)}`");

            lineAvailableValues.RemoveAt(i);
            allAvailableValues.RemoveAt(allListIndex);

            if (TrySolveRestOfLine(restOfline, lineAvailableValues, allAvailableValues, linesSolved))
                return true;

            lineAvailableValues.Insert(i, value);
            allAvailableValues.Insert(allListIndex, value);
        }

        field.Reset();
        restOfline.Push(field);
        return false;
    }

    static IEnumerable<List<int>> FindSubsetWithSpecifiedSum(Stack<int> values, int amount, int sum)
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
    
    Field[]? FindNextLine(int n)
    {
        if (_lineCache.Count >= n + 1)
            return _lineCache[n];
        
        var lineWithManyNumbers = _board.Rows.Concat(_board.Columns).Concat(_board.Diagonals)
            .Where(x => x.Any(y => !y.HasValue)).MinBy(x => x.Count(y => !y.HasValue));
        // TODO: Diff to mid: (Abs(65/2-SUM)) ?

        if (lineWithManyNumbers == null)
            return null;

        _lineCache.Add(lineWithManyNumbers);
        return lineWithManyNumbers;
    }
}
