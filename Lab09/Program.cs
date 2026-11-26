using System;

public static class Program
{
    public static readonly Func<int, int, int> Cmp = (x, y) => x.CompareTo(y);
    public static readonly string[] Names = { "Бульбашка", "Швидке, медіана трьох", "Шелл" };
    public static readonly SortAlgorithm<int>[] Algorithms = { Sorts.Bubble, Sorts.QuickMedian, Sorts.Shell };
    public static void Main()
    {
        var r = new Report(); r.Line("# Результати лабораторної 9"); r.EnvironmentInfo();
        Trace(r); Edges(r); Counters(r); Stability.Run(r); Measurements.Run(r); r.Save("Results.md");
    }
    public static int[] Input(int n, int kind)
    {
        var a = Data.Numbers(n, 23, 0, 20);
        if (kind > 0) Array.Sort(a); // Підготовка входу поза власними сортуваннями.
        if (kind == 2)
            for (int i = 0; i < a.Length / 2; i++) { int t = a[i]; a[i] = a[a.Length - 1 - i]; a[a.Length - 1 - i] = t; }
        if (kind == 3 && a.Length > 0)
        {
            var rng = new Random(23);
            for (int i = 0; i < 1; i++) { int x = rng.Next(n), y = rng.Next(n); int t = a[x]; a[x] = a[y]; a[y] = t; }
        }
        return a;
    }
    private static void Trace(Report r)
    {
        r.Line("## 1. Трасування 7 елементів, перевірене проходами");
        var a = Data.Numbers(7, 23, 0, 20); r.Line("| Прохід | Стан | Порівняння за прохід | Записи за прохід |"); r.Line("|---|---|---:|---:|");
        r.Line($"| Вхід | {string.Join(", ", a)} | — | — |");
        for (int end = a.Length - 1, pass = 1; end > 0; end--, pass++)
        {
            bool swapped = false; int moves = 0;
            for (int i = 0; i < end; i++) if (a[i] > a[i + 1])
            { int t = a[i]; a[i] = a[i + 1]; a[i + 1] = t; moves += 2; swapped = true; }
            r.Line($"| {pass} | {string.Join(", ", a)} | {end} | {moves} |");
            if (!swapped) break;
        }
        r.Line();
    }
    private static void Counters(Report r)
    {
        r.Line("## 4. Порівняння / записи елементів у масиви");
        r.Line("Рахуються записи як в основний масив, так і в буфер T[]. Присвоєння локальній змінній та службові частоти не враховуються. Лічильники long, бо на n=100000 int переповнюється.");
        for (int alg = 0; alg < Algorithms.Length; alg++)
        {
            r.Line(); r.Line("### " + Names[alg]); r.Line("| n | Випадковий | Відсортований | Навпаки | Майже |"); r.Line("|---:|---|---|---|---|");
            foreach (int n in new[] { 100, 1000, 10000 })
            {
                string row = $"| {n}";
                for (int kind = 0; kind < 4; kind++)
                {
                    var original = Input(n, kind); var a = (int[])original.Clone(); Algorithms[alg](a, Cmp, out long c, out long m); Checks.Verify(original, a, Cmp);
                    if (alg == 0 && kind == 1) Bench.Check(c == n - 1 && m == 0, "Бульбашка на впорядкованому");
                    row += $" | {c} / {m}";
                }
                r.Line(row + " |");
            }
        }
        r.Line("## Шелл та звичайні вставки, n = 10 000");
        for (int kind = 0; kind < 4; kind++)
        {
            var data = Input(10000, kind);
            var copy = (int[])data.Clone();
            Sorts.Insertion(copy,Cmp,out long ci,out long mi);
            Checks.Verify(data,copy,Cmp);
            Sorts.Shell(copy = (int[])data.Clone(),Cmp,out long cs,out long ms);
            Checks.Verify(data,copy,Cmp);
            r.Line($"{kind+1}: вставки {ci}/{mi}, Шелл {cs}/{ms}.");
        }
        var equal = new int[10000];
        Sorts.QuickMedian(equal,Cmp,out long ce,out long me);
        Checks.Verify(new int[10000],equal,Cmp);
        r.Line($"Усі 10 000 однакові: швидке {ce} порівнянь, {me} записів.");
        var randomInput = Input(10000, 0); var b = (int[])randomInput.Clone(); var merge = (int[])randomInput.Clone();
        Sorts.Bubble(b, Cmp, out long bc, out _); Sorts.QuickMedian(merge, Cmp, out long mc, out _);
        r.Line(); r.Line($"Відношення порівнянь бульбашки до швидкого на випадкових n=10000: {Bench.F((double)bc / mc)} (лекційне 376 — інша модель і вхід)."); r.Line();
    }
    private static void Edges(Report r)
    {
        r.Line("## Крайові випадки"); r.Line("| Вхід | Очікувано | Отримано |"); r.Line("|---|---|---|");
        int[][] inputs = { Array.Empty<int>(), new[] { 5 }, new[] { 1, 2 }, new[] { 2, 1 }, new[] { 7, 7, 7, 7 }, new[] { 2, 2, 2, 1, 1, 1 } };
        foreach (var input in inputs)
        {
            foreach (var sort in Algorithms)
            { var a = (int[])input.Clone(); sort(a, Cmp, out _, out _); Checks.Verify(input, a, Cmp); }
            r.Line($"| [{string.Join(",", input)}] | IsSorted і IsPermutationOf = true | усі три алгоритми: true / true, OK |");
        }
        // Додаткові розміри перевіряють неповні хвости злиття.
        for (int n = 0; n <= 257; n++)
        {
            var original = Input(n, 0);
            foreach (var sort in Algorithms) { var a = (int[])original.Clone(); sort(a, Cmp, out _, out _); Checks.Verify(original, a, Cmp); }
        }
        r.Line("Додатково пройдено всі довжини 0–257, включно з короткими ділянками."); r.Line();
    }
}
