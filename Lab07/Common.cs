using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

public sealed class Student
{
    public string Name;
    public string Group;
    public int Score;
    public Student(string name, string group, int score) { Name = name; Group = group; Score = score; }
    public override string ToString() => $"{Name} ({Group}, {Score})";
}

public static class Data
{
    public static List<Student> Students() => new List<Student>
    {
        new Student("Коваленко", "КН-3/1", 88), new Student("Мельник", "КН-3/2", 64),
        new Student("Іщенко", "КН-3/1", 93), new Student("Бондар", "КН-3/1", 71),
        new Student("Савченко", "КН-3/2", 55), new Student("Ткачук", "КН-3/2", 90),
        new Student("Кравець", "КН-3/1", 76), new Student("Іванів", "КН-3/1", 48),
        new Student("Мороз", "КН-3/2", 82), new Student("Шевчук", "КН-3/1", 67),
        new Student("Бойко", "КН-3/2", 95), new Student("Кушнір", "КН-3/2", 73)
    };
    public static int[] Numbers(int n, int seed, int min, int max)
    {
        var r = new Random(seed); var a = new int[n];
        for (int i = 0; i < n; i++) a[i] = r.Next(min, max + 1);
        return a;
    }
    public static Student[] Records(int n)
    {
        var r = new Random(5); var source = Students(); var a = new Student[n];
        for (int i = 0; i < n; i++)
        { var s = source[i % source.Count]; a[i] = new Student(s.Name, s.Group, r.Next(101)); }
        return a;
    }
}

public static class Bench
{
    public static double Min(Action action)
    {
        action(); // Прогрів перед трьома вимірюваннями.
        double best = double.MaxValue;
        for (int i = 0; i < 3; i++)
        { var sw = Stopwatch.StartNew(); action(); sw.Stop(); best = Math.Min(best, sw.Elapsed.TotalMilliseconds); }
        return best;
    }
    public static string F(double x) => x.ToString("F4", CultureInfo.InvariantCulture);
    public static void Check(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }
}

public sealed class Report
{
    private readonly StringBuilder text = new StringBuilder();
    public void Line(string s = "") { text.AppendLine(s); Console.WriteLine(s); }
    public void Save(string file)
    {
        File.WriteAllText(file, text.ToString(), new UTF8Encoding(false));
    }
    public void EnvironmentInfo()
    {
        Line($"Фактичний запуск: {DateTimeOffset.Now:O}; {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}; {System.Runtime.InteropServices.RuntimeInformation.OSDescription}; {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}.");
#if DEBUG
        throw new InvalidOperationException("Для вимірювань потрібна збірка Release.");
#else
        Line("Конфігурація Release; Stopwatch; прогрів; мінімум із трьох запусків. Час залежить від комп'ютера й навантаження.");
#endif
        Line();
    }
}
