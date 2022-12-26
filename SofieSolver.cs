namespace JuleSudoku;

class SofieSolver : ISolver
{
    readonly Board _board;
    readonly bool _stopAtFirstSolution;
    readonly List<Field[]> _lineCache = new(Board.Size * 2 + 1);

    public int Updates { get; private set; }

    public List<int[][]> Solutions { get; }

    public SofieSolver(Board board, bool stopAtFirstSolution = false)
    {
        _board = board;
        _stopAtFirstSolution = stopAtFirstSolution;
        Updates = 0;
        Solutions = new List<int[][]>();
    }

    public bool Solve()
    {
        var predeterminedValues = _board.Locked.Select(x => x.Value!.Value);
        var availableValues = InitializeNumbers(predeterminedValues);

        SolveNextLine(availableValues);
        return Solutions.Any();
    }
    static Numbers InitializeNumbers(IEnumerable<int> predeterminedValues)
    {
        var divisionGroups = new Number[Board.Size];
        var moduloGroups = new Number[Board.Size];
        var allNumbers = new Number[Board.Size*Board.Size];
        var availableValues = new List<Number>(Board.Size * Board.Size);
        foreach (var number in Enumerable.Range(1, Board.Size * Board.Size)
                     .Select(x => new Number(x, predeterminedValues.Contains(x))))
        {
            allNumbers[number.Value - 1] = number;
            divisionGroups[number.Division] = number;
            moduloGroups[number.Modulus] = number;
            if (number.Available)
                availableValues.Add(number);
        }

        return new Numbers(divisionGroups, moduloGroups, allNumbers, availableValues);
    }

    bool SolveNextLine(Numbers numbers, int linesSolved = 0)
    {
        var line = FindNextLine(linesSolved);
        if (line == null)
        {
            Solutions.Add(_board.Rows.Select(x => x.Select(y => y.Value!.Value).ToArray()).ToArray());
            return _stopAtFirstSolution; // We want to find all the solutions
        }

        return TrySolveLine(line, numbers, linesSolved);
    }

    bool TrySolveLine(Field[] line, Numbers numbers, int linesSolved)
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
            FindSubsetWithSpecifiedSum(new Stack<int>(numbers.AvailableValues.Select(x => x.Value)), undeterminedFields.Count,
                    Validator.ExpectedSum - existingSum)
                .ToList();

        return subsetWithSpecifiedSum
            .Select(x => TrySolveRestOfLine(new Stack<Field>(undeterminedFields), x, numbers, linesSolved))
            .FirstOrDefault(x => x, false);
    }

    bool TrySolveRestOfLine(Stack<Field> restOfline, List<int> lineAvailableValues, Numbers allNumbers,
        int linesSolved)
    {
        if (!restOfline.Any()) return SolveNextLine(allNumbers, linesSolved + 1);
        if (!lineAvailableValues.Any()) return false;

        var field = restOfline.Pop();

        for (var i = 0; i < lineAvailableValues.Count; i++)
        {
            var value = lineAvailableValues[i];
            field.Set(value);
            Updates++;

            if (!Validator.ValidateField(_board, field.Point, allNumbers.AvailableValues.Select(x => x.Value).Take(5).ToList()))
                continue;

            // We need the index to insert it back in the correct place.
            var numbersIndex = allNumbers.Use(value);
            
            lineAvailableValues.RemoveAt(i);

            if (TrySolveRestOfLine(restOfline, lineAvailableValues, allNumbers, linesSolved))
                return true;

            lineAvailableValues.Insert(i, value);
            allNumbers.Unused(numbersIndex, value);
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

    class Number
    {
        private readonly bool _predetermined;
        private bool _used;

        public Number(int value, bool predetermined = false)
        {
            Value = value;
            _predetermined = predetermined;
        }

        public int Value { get; }
        public int Division => (Value - 1) / Board.Size;
        public int Modulus => (Value - 1) % Board.Size;

        public bool Used
        {
            private get { return _used; }
            set
            {
                if (_predetermined)
                    throw new InvalidOperationException($"Number ({Value}) is predetermined and cannot be set.");
                if (_used == value)
                    throw new InvalidOperationException(
                        $"Number ({Value}) is already set and cannot be set, or not set and cannot be unset ({_used}).");
                _used = value;
            }
        }

        public bool Available => !_predetermined && !Used;
    }

    record struct Numbers(Number[] DivisionGroups, Number[] ModuloGroups, Number[] AllNumbers,
        List<Number> AvailableValues)
    {
        Number GetNumber(int value) => AllNumbers[value - 1];

        public int Use(int value)
        {
            var number = GetNumber(value);
            number.Used = true;
            var index = AvailableValues.FindIndex(x => x.Value == value);
            AvailableValues.RemoveAt(index);
            return index;
        }

        public void Unused(int index, int value)
        {
            var number = GetNumber(value);
            number.Used = false;
            AvailableValues.Insert(index, number);
        }
    }
}