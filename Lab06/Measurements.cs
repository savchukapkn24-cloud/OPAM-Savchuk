using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class Measurements
{
    private static int sink;
    public static void Run(Report r)
    {
        r.Line("## 5. Час");
        int scale = 1;
        // Пробний повний прогін; якщо будь-яка спискова операція >60 с,
        // усі кількості зменшуються вдесятеро, й заміри повторюються.
        while (true)
        {
            var times = Measure(scale);
            bool tooSlow = false; foreach (var row in times) if (row.list > 60000) tooSlow = true;
            if (tooSlow) { scale *= 10; continue; }
            r.Line($"Масштаб: усі кількості поділено на {scale}. Побудова початкових даних поза таймером; побудова результату входить у час алгоритму.");
            r.Line("| Операція | Список, мс | Хеш-структура, мс | Виграш |"); r.Line("|---|---:|---:|---:|");
            foreach (var row in times) r.Line($"| {row.name} | {Bench.F(row.list)} | {Bench.F(row.hash)} | {Bench.F(row.list / row.hash)} |");
            r.Line($"Контрольна сума: {sink}."); break;
        }
    }
    private static List<(string name, double list, double hash)> Measure(int scale)
    {
        int n = 100000 / scale, q = 10000 / scale;
        var values = new List<int>(); for (int i = 0; i < n; i++) values.Add(i);
        var set = new HashSet<int>(values); var queries = Data.Numbers(q, 5, 0, n * 2);
        Action linear = () => { int found = 0; foreach (int x in queries) if (Algorithms.Position(values, x) >= 0) found++; sink = found; };
        Action hash = () => { int found = 0; foreach (int x in queries) if (set.Contains(x)) found++; sink = found; };
        var result = new List<(string, double, double)> { ($"{q} перевірок / {n} елементів", Bench.Min(linear), Bench.Min(hash)) };
        var words = new List<string>(); var random = new Random(5);
        for (int i = 0; i < 50000 / scale; i++) words.Add("слово" + random.Next(5000 / scale));
        double l = Bench.Min(() => sink = Algorithms.FrequenciesList(words).keys.Count);
        double h = Bench.Min(() => sink = Algorithms.Frequencies(words).Count);
        result.Add(($"Частоти {words.Count} слів", l, h));
        var numbers = new List<int>(Data.Numbers(20000 / scale, 5, 0, 10000 / scale));
        l = Bench.Min(() => sink = Algorithms.UniqueList(numbers).Count); h = Bench.Min(() => sink = Algorithms.UniqueOrdered(numbers).Count);
        result.Add(($"Дублікати {numbers.Count} чисел", l, h));
        var a = new List<int>(Data.Numbers(5000 / scale, 5, 0, 10000 / scale)); var b = new List<int>(Data.Numbers(5000 / scale, 105, 0, 10000 / scale));
        l = Bench.Min(() => sink = Algorithms.IntersectionList(a, b).Count); h = Bench.Min(() => sink = Algorithms.Intersection(a, b).Count);
        result.Add(($"Перетин {a.Count} + {b.Count}", l, h)); return result;
    }
}
