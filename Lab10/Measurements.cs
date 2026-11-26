using System.Diagnostics;
using System.Globalization;
using System.Text;

public static class Measurements
{
    public static double Time(Action action)
    {
        action();
        double best = double.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            var timer = Stopwatch.StartNew();
            action();
            timer.Stop();
            best = Math.Min(best, timer.Elapsed.TotalMilliseconds);
        }
        return best;
    }

    public static void Pitfalls(Action<string> log, List<string> words)
    {
        log("## 1. Пастки");
        log("| Додавань | +=, мс | StringBuilder, мс |");
        log("| ---: | ---: | ---: |");
        foreach (int n in new[] { 10_000, 100_000 })
        {
            double slow = Time(() =>
            {
                string value = "";
                for (int i = 0; i < n; i++) value += 'В';
                GC.KeepAlive(value);
            });
            double fast = Time(() =>
            {
                var value = new StringBuilder();
                for (int i = 0; i < n; i++) value.Append('В');
                GC.KeepAlive(value.ToString());
            });
            log($"| {n} | {slow:F4} | {fast:F4} |");
        }

        int lowerMatches = 0, ordinalMatches = 0;
        double lower = Time(() =>
        {
            int same = 0;
            for (int i = 0; i < 100_000; i++)
                if (words[i % words.Count].ToLower() == words[i / 2 % words.Count].ToLower())
                    same++;
            lowerMatches = same;
        });
        double ordinal = Time(() =>
        {
            int same = 0;
            for (int i = 0; i < 100_000; i++)
                if (words[i % words.Count].Equals(words[i / 2 % words.Count], StringComparison.OrdinalIgnoreCase))
                    same++;
            ordinalMatches = same;
        });
        log($"\n100 000 пар: ToLower — {lower:F4} мс ({lowerMatches} збігів), OrdinalIgnoreCase — {ordinal:F4} мс ({ordinalMatches} збігів).");

        string[] sample = ["гай", "ґанок", "ґава", "ґазда", "ґатунок", "ґвалт", "ґедзь", "ґелґіт", "ґердан", "ґиндичка", "ґирлиґа", "ґлей", "ґонт", "ґонта", "ґраніт", "ґречний", "ґринджоли", "ґрунт", "ґудзик", "ґуля", "ґума", "дар"];
        var cultural = (string[])sample.Clone();
        Array.Sort(sample, StringComparer.Ordinal);
        Array.Sort(cultural, StringComparer.Create(CultureInfo.GetCultureInfo("uk-UA"), ignoreCase: false));
        log("Ординально: " + string.Join(", ", sample));
        log("За українською культурою: " + string.Join(", ", cultural));
        log("\nКожне += копіює вже накопичений незмінний рядок. OrdinalIgnoreCase порівнює без створення двох нижньорегістрових копій. Українське порівняння розміщує ґ між г і д.\n");
    }
    public static void CompareReading(Action<string> log)
    {
        string path = Path.Combine(Path.GetTempPath(),"opam-23-"+Guid.NewGuid()+".txt");
        try
        {
            using (var output = new StreamWriter(path))
                for (int i = 0; i < 100_000; i++)
                    output.WriteLine($"Рядок {i:000000} — навчальний приклад обробки тексту.");

            (double Time,long Peak,long Chars) full = Profile(() =>
            {
                string raw = File.ReadAllText(path);
                string[] lines = raw.Split('\n',StringSplitOptions.RemoveEmptyEntries);
                long chars = 0;
                foreach (string line in lines) chars += line.TrimEnd('\r').Length;
                return (chars,GC.GetTotalMemory(false));
            });
            (double Time,long Peak,long Chars) stream = Profile(() =>
            {
                using var reader = new StreamReader(path);
                long chars = 0, peak = 0;
                int lineNumber = 0;
                while (reader.ReadLine() is string line)
                {
                    chars += line.Length;
                    if (++lineNumber % 100 == 0)
                        peak = Math.Max(peak,GC.GetTotalMemory(false));
                }
                return (chars,peak);
            });
            if (full.Chars != stream.Chars) throw new InvalidOperationException("Різна кількість символів");
            log($"Split: {full.Time:F3} мс, піковий приріст керованої купи приблизно {full.Peak/1048576.0:F2} MiB.");
            log($"StreamReader: {stream.Time:F3} мс, піковий приріст керованої купи приблизно {stream.Peak/1048576.0:F2} MiB. Контрольна сума символів: {stream.Chars}.");
            log("Купу оцінено за GC.GetTotalMemory: для Split після обробки масиву, для StreamReader кожні 100 рядків. Це вибірковий замір, а не загальна пікова пам’ять процесу.");
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static (double Time,long Peak,long Chars) Profile(Func<(long Chars,long Peak)> execute)
    {
        execute();
        double best = double.MaxValue;
        long largest = 0, chars = 0;
        for (int trial = 0; trial < 3; trial++)
        {
            long baseline = GC.GetTotalMemory(true);
            var watch = Stopwatch.StartNew();
            (chars,long peak) = execute();
            watch.Stop();
            best = Math.Min(best,watch.Elapsed.TotalMilliseconds);
            largest = Math.Max(largest,peak-baseline);
        }
        return (best,largest,chars);
    }
}
