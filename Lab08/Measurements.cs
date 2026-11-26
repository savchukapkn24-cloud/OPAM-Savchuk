using System;

public static class Measurements
{
    private static long sink;
    public static void Run(Report r)
    {
        r.Line("## 5. Середні порівняння: 100 запитів");
        r.Line("Запити — бали 0–100 із Random(123); масив відсортований. Лінійний пошук доходить приблизно до середини масиву, а бінарний часто потрапляє в широкий блок рівних балів раніше, ніж за log₂n кроків.");
        r.Line("| n | Лінійний | Бінарний | Відношення | n/2; ceil(log₂n) |"); r.Line("|---:|---:|---:|---:|---|");
        foreach (int n in new[] { 1000, 10000, 100000, 1000000 })
        {
            var a = Data.Records(n); Array.Sort(a, (x, y) => Program.Cmp(x, y)); var random = new Random(123);
            long linear = 0, binary = 0;
            for (int i = 0; i < 100; i++)
            {
                var target = new Student("", "", random.Next(101));
                int li = Search.LinearSearch(a, target, Program.Cmp, out int lc), bi = Search.BinarySearch(a, target, Program.Cmp, out int bc);
                Bench.Check((li < 0) == (bi < 0), "Збіг запитів"); linear += lc; binary += bc;
            }
            r.Line($"| {n} | {Bench.F(linear / 100.0)} | {Bench.F(binary / 100.0)} | {Bench.F((double)linear / binary)} | {n / 2}; {Math.Ceiling(Math.Log2(n))} |");
        }
        r.Line(); r.Line("## Точка окупності");
        r.Line("Лінійний пошук працює на початковому невідсортованому масиві; сортований шлях щоразу починається з його копії. Клонування перед таймером в обох шляхах; сортування всередині таймера. Обидва шляхи повертають наявність бала, а не однакові індекси.");
        var source = Data.Records(100000); var rng = new Random(123); var queries = new Student[100];
        for (int i = 0; i < queries.Length; i++) queries[i] = new Student("", "", rng.Next(101));
        r.Line("| q | Лінійно, мс | Сортування + бінарно, мс | Швидший |"); r.Line("|---:|---:|---:|---|");
        int breakEven = -1;
        foreach (int q in new[] { 1, 5, 10, 20, 30, 50, 100 })
        {
            double linear = Measure(source, queries, q, false), binary = Measure(source, queries, q, true);
            if (breakEven < 0 && binary < linear) breakEven = q;
            r.Line($"| {q} | {Bench.F(linear)} | {Bench.F(binary)} | {(linear < binary ? "лінійний" : "сортування + бінарний")} |");
        }
        r.Line(breakEven < 0 ? "У перевірених q ≤ 100 точку окупності не досягнуто." : $"Перше виграшне q серед перевірених: {breakEven}.");
        if (breakEven < 0) Extend(r, source);
        r.Line("У невідсортованому масиві бали рівномірні 0–100: перший збіг у середньому знаходиться приблизно за 101 перевірку. Тому очікування 34 запити (для інших припущень про дані) сюди не переноситься; повтори істотно змінюють картину.");
        r.Line($"Контрольна сума: {sink}.");
    }
    private static void Extend(Report r, Student[] source)
    {
        r.Line(); r.Line("### Додатковий пошук окупності за межами q=100");
        var rng = new Random(123); var queries = new Student[1000000];
        for (int i = 0; i < queries.Length; i++) queries[i] = new Student("", "", rng.Next(101));
        r.Line("| q | Лінійно, мс | Сортування + бінарно, мс |"); r.Line("|---:|---:|---:|");
        int low = 100, high = 1000;
        while (true)
        {
            double l = Measure(source, queries, high, false), b = Measure(source, queries, high, true);
            r.Line($"| {high} | {Bench.F(l)} | {Bench.F(b)} |");
            if (b < l) break;
            low = high;
            if (high == queries.Length) { r.Line("Точку окупності не досягнуто навіть за q=1000000."); return; }
            high *= 10;
        }
        // Уточнення на сітці з кроком 1000; секундомір має шум.
        while (high - low > 1000)
        {
            int q = ((low + high) / 2000) * 1000;
            if (q <= low) q = low + 1000;
            double l = Measure(source, queries, q, false), b = Measure(source, queries, q, true);
            r.Line($"| {q} | {Bench.F(l)} | {Bench.F(b)} |");
            if (b < l) high = q; else low = q;
        }
        r.Line($"Експериментальна точка окупності на сітці 1000: q≈{high}; останній програшний перевірений q={low}. Це значно більше лекційних 34 через ранній збіг повторюваних балів у невідсортованому вході; шум вимірювань впливає на межу.");
    }
    private static double Measure(Student[] source, Student[] queries, int q, bool sort)
    {
        double best = double.MaxValue;
        for (int run = -1; run < 3; run++)
        {
            var copy = (Student[])source.Clone(); var sw = System.Diagnostics.Stopwatch.StartNew(); long sum = 0;
            if (sort) Array.Sort(copy, (x, y) => Program.Cmp(x, y));
            for (int i = 0; i < q; i++)
            {
                int pos = sort ? Search.BinarySearch(copy, queries[i], Program.Cmp, out _) : Search.LinearSearch(copy, queries[i], Program.Cmp, out _);
                sum += pos >= 0 ? 1 : 0;
            }
            sw.Stop(); sink = sum;
            if (run >= 0) best = Math.Min(best, sw.Elapsed.TotalMilliseconds);
        }
        return best;
    }
}
