

class Program
{
    internal enum Direction
    {
        N, E, S, W, None
    }

    public static readonly Dictionary<char, Tuple<Direction, Direction>> Pipes = new Dictionary<char, Tuple<Direction, Direction>>
    {
        {'|', Tuple.Create(Direction.N, Direction.S)},
        {'-', Tuple.Create(Direction.E, Direction.W)},
        {'L', Tuple.Create(Direction.N, Direction.E)},
        {'J', Tuple.Create(Direction.N, Direction.W)},
        {'7', Tuple.Create(Direction.S, Direction.W)},
        {'F', Tuple.Create(Direction.S, Direction.E)},
        {'.', Tuple.Create(Direction.None, Direction.None)}
    };

    static List<List<int>> Visit(List<List<Tuple<Direction, Direction>>> pipeMap, int startY, int startX)
    {
        var visited = new List<List<int>>();
        visited.Capacity = pipeMap.Count;

        for (int y = 0; y < pipeMap.Count; y++)
        {
            visited.Add(Enumerable.Repeat(-1, pipeMap[y].Count).ToList());
        }

        var check = new HashSet<Tuple<int, int>> { Tuple.Create(startY, startX) };
        int depth = 0;

        while (check.Count > 0)
        {
            var next = new HashSet<Tuple<int, int>>();

            foreach (var tuple in check)
            {
                var y = tuple.Item1;
                var x = tuple.Item2;

                if (visited[y][x] != -1)
                {
                    continue;
                }

                visited[y][x] = depth;
                var dirs = pipeMap[y][x];

                if (dirs.Item1 == Direction.N)
                    next.Add(Tuple.Create(y - 1, x));
                else if (dirs.Item1 == Direction.S)
                    next.Add(Tuple.Create(y + 1, x));
                else if (dirs.Item1 == Direction.E)
                    next.Add(Tuple.Create(y, x + 1));
                else if (dirs.Item1 == Direction.W) next.Add(Tuple.Create(y, x - 1));

                switch (dirs.Item2)
                {
                    case Direction.N:
                        next.Add(Tuple.Create(y - 1, x));
                        break;
                    case Direction.S:
                        next.Add(Tuple.Create(y + 1, x));
                        break;
                    case Direction.E:
                        next.Add(Tuple.Create(y, x + 1));
                        break;
                    case Direction.W:
                        next.Add(Tuple.Create(y, x - 1));
                        break;
                    case Direction.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            check = next;
            depth++;
        }

        return visited;
    }

    static int Part1(List<List<int>> visited)
    {
        int deepest = 0;

        foreach (var row in visited)
        {
            foreach (var col in row)
            {
                if (col > deepest)
                {
                    deepest = col;
                }
            }
        }

        return deepest;
    }

    public static int Part2(List<List<Tuple<Direction, Direction>>> pipeMap, IReadOnlyList<List<int>> visited)
    {
        var area = new List<List<bool>>();
        area.Capacity = pipeMap.Count;

        for (int y = 0; y < pipeMap.Count; y++)
        {
            area.Add(Enumerable.Repeat(false, pipeMap[y].Count).ToList());
        }

        for (var row = 0; row < pipeMap.Count; row++)
        {
            var inside = false;
            var lastCorner = Tuple.Create(Direction.None, Direction.None);

            for (var col = 0; col < pipeMap[row].Count; col++)
            {
                if (visited[row][col] == -1)
                {
                    area[row][col] = inside;
                }
                else
                {
                    var tile = pipeMap[row][col];

                    if (tile.Equals(Pipes['|']))
                    {
                        inside = !inside;
                    }
                    else if (tile.Equals(Pipes['L']) || tile.Equals(Pipes['F']))
                    {
                        lastCorner = tile;
                    }
                    else if (tile.Equals(Pipes['J']) && lastCorner.Equals(Pipes['F']))
                    {
                        inside = !inside;
                    }
                    else if (tile.Equals(Pipes['7']) && lastCorner.Equals(Pipes['L']))
                    {
                        inside = !inside;
                    }
                }
            }
        }

        var count = 0;

        for (int row = 0; row < pipeMap.Count; row++)
        {
            for (int col = 0; col < pipeMap[row].Count; col++)
            {
                if (area[row][col])
                {
                    count++;
                }
            }
        }

        return count;
    }

    static void Main()
    {
        var input = new StreamReader(@"C:\testutvikling\PipeMaze\PipeMaze\input.txt");
        var pipeMap = new List<List<Tuple<Direction, Direction>>>();
        int y = 0, startX = 0, startY = 0;

        while (!input.EndOfStream)
        {
            var line = input.ReadLine();
            pipeMap.Add(new List<Tuple<Direction, Direction>>());

            if (line != null)
                for (var x = 0; x < line.Length; x++)
                {
                    if (line[x] == 'S')
                    {
                        startX = x;
                        startY = y;

                        pipeMap[^1].Add(Pipes['.']);
                    }
                    else
                    {
                        pipeMap[^1].Add(Pipes[line[x]]);
                    }
                }

            y++;
        }

        var startConnections = new List<Direction>();
        startConnections.Capacity = 2;

        if (startY - 1 >= 0 && (pipeMap[startY - 1][startX].Item1 == Direction.S || pipeMap[startY - 1][startX].Item2 == Direction.S))
        {
            startConnections.Add(Direction.N);
        }
        if (startY + 1 < pipeMap.Count && (pipeMap[startY + 1][startX].Item1 == Direction.N || pipeMap[startY + 1][startX].Item2 == Direction.N))
        {
            startConnections.Add(Direction.S);
        }
        if (startX + 1 < pipeMap[startY].Count && (pipeMap[startY][startX + 1].Item1 == Direction.W || pipeMap[startY][startX + 1].Item2 == Direction.W))
        {
            startConnections.Add(Direction.E);
        }
        if (startX - 1 >= 0 && (pipeMap[startY][startX - 1].Item1 == Direction.E || pipeMap[startY][startX - 1].Item2 == Direction.E))
        {
            startConnections.Add(Direction.W);
        }

        pipeMap[startY][startX] = Tuple.Create(startConnections[0], startConnections[1]);

        var visited = Visit(pipeMap, startY, startX);

        Console.WriteLine("Part 1: " + Part1(visited));
        Console.WriteLine("Part 2: " + Part2(pipeMap, visited));
    }
}
