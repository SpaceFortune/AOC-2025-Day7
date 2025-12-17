using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;

class Program
{
    static void Main()
    {
        var path = "input.txt";
        if (!File.Exists(path))
        {
            Console.WriteLine("input.txt nicht gefunden.");
            return;
        }

        var grid = File.ReadAllLines(path);
        int rows = grid.Length;
        int cols = grid[0].Length;

        // Startpunkt finden
        int startRow = -1, startCol = -1;
        for (int r = 0; r < rows; r++)
        {
            int c = grid[r].IndexOf('S');
            if (c >= 0)
            {
                startRow = r;
                startCol = c;
                break;
            }
        }

        if (startRow == -1)
        {
            Console.WriteLine("Kein Startpunkt gefunden!");
            return;
        }

        Console.WriteLine($"Startpunkt: Zeile {startRow}, Spalte {startCol}");

        // Simulation
        int splits = 0;
        var queue = new Queue<(int r, int c)>();
        var visited = new HashSet<(int r, int c)>(); // neu: besuchte Positionen merken

        queue.Enqueue((startRow + 1, startCol));

        while (queue.Count > 0)
        {
            var (r, c) = queue.Dequeue();

            // schon besucht? -> überspringen
            if (!visited.Add((r, c))) continue;

            if (r >= rows || c < 0 || c >= cols) continue; // raus aus Grid

            char cell = grid[r][c];

            if (cell == '.')
            {
                queue.Enqueue((r + 1, c));
            }
            else if (cell == '^')
            {
                splits++;
                Console.WriteLine($"Split bei Zeile {r}, Spalte {c} (Gesamt: {splits})");

                if (c - 1 >= 0) queue.Enqueue((r + 1, c - 1));
                if (c + 1 < cols) queue.Enqueue((r + 1, c + 1));
            }
            else
            {
                queue.Enqueue((r + 1, c));
            }
        }

        Console.WriteLine($"Gesamtzahl Splits: {splits}");

        long timelines = CountTimelines(grid, startRow, startCol);
        Console.WriteLine($"Part 2 – Anzahl Zeitlinien: {timelines}");

    }
    // part 2 
    static long CountTimelines(string[] grid, int startRow, int startCol)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;

        var current = new Dictionary<int, long>();

        if (startRow + 1 < rows)
            current[startCol] = 1;
        else
            return 1;

        for (int r = startRow + 1; r < rows; r++)
        {
            var next = new Dictionary<int, long>();

            foreach (var kv in current)
            {
                int c = kv.Key;
                long count = kv.Value;
                if (c < 0 || c >= cols || count == 0) continue;

                char cell = grid[r][c];

                if (cell == '.')
                {
                    Add(next, c, count);
                }
                else if (cell == '^')
                {

                    if (c - 1 >= 0) Add(next, c - 1, count);
                    if (c + 1 < cols) Add(next, c + 1, count);
                }
                else
                {
                    Add(next, c, count);
                }
            }
            current = next;
        }

        long finished = 0;
        foreach (var kv in current)
            finished += kv.Value;

        return finished;
    }

    static void Add(Dictionary<int, long> dict, int key, long val)
    {
        if (dict.TryGetValue(key, out long old))
            dict[key] = old + val;
        else
            dict[key] = val;
    }
}
