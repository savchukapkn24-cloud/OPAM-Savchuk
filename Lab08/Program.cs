using System;

public static class Program
{
    public static readonly Func<Student, Student, int> Cmp = (x, y) => x.Score.CompareTo(y.Score);
    public static void Main()
    {
        var r = new Report(); r.Line("# Результати лабораторної 8"); r.EnvironmentInfo();
        var unsorted = Data.Records(100000); var a = (Student[])unsorted.Clone(); Array.Sort(a, (x, y) => Cmp(x, y));
        Queries(r, a); Failures(r, a, unsorted); Boundaries(r, a); Edges(r); Measurements.Run(r); r.Save("Results.md");
    }
    private static void Queries(Report r, Student[] a)
    {
        r.Line("## 1, 2, 4. Пошуки");
        r.Line("| Запит | Лінійний індекс / порівняння | Бінарний | Експоненційний |"); r.Line("|---|---|---|---|");
        foreach (int score in new[] { 51, a[0].Score, 101 })
        {
            var x = new Student("", "", score);
            int l = Search.LinearSearch(a, x, Cmp, out int lc), b = Search.BinarySearch(a, x, Cmp, out int bc);
            int rec = Search.Exponential(a, x, Cmp, out int rc);
            Bench.Check((l < 0) == (b < 0) && (b < 0) == (rec < 0), "Пошуки не узгоджені");
            r.Line($"| Бал {score} | {l} / {lc} | {b} / {bc} | {rec} / {rc} |");
        }
        r.Line("Лінійний пошук відсутнього бала переглядає весь масив. Експоненційний знаходить вузьке вікно поряд із початком, а для останніх позицій сплачує за подвоєння межі.");
    }
    private static void Failures(Report r, Student[] a, Student[] unsorted)
    {
        r.Line("## Чотири поломки");
        int bad = Broken.StrictLoop(new[] { 7 }, 7, (x, y) => x.CompareTo(y), out int comparisons);
        Bench.Check(bad == -1, "Очікувана поломка <="); r.Line($"1. [7], шукаємо 7: {bad}, порівнянь {comparisons}. Останній кандидат не перевірений.");
        bad = Broken.Stuck(new[] { 1, 2 }, 2, (x, y) => x.CompareTo(y), out comparisons, out int lo, out int hi);
        Bench.Check(comparisons == 100 && lo == 0 && hi == 1, "Очікуване зависання");
        r.Line($"2. [1,2], шукаємо 2: зупинено після {comparisons} кроків; lo={lo}, hi={hi}, результат {bad}. Без обмежувача — нескінченний цикл.");
        int index = Broken.UnsafeMidpoint(a, new Student("", "", 51), Cmp, out comparisons);
        int largeLo = 1073741824, largeHi = 1073741824;
        r.Line($"3. На 100000 записах індекс={index}, порівнянь={comparisons}. Без великого масиву: lo=hi={largeLo}, unchecked((lo+hi)/2)={unchecked((largeLo + largeHi) / 2)}.");
        Bench.Check(index >= 0 && a[index].Score == 51, "Мала unsafe версія");
        int misses = 0; var random = new Random(123);
        for (int i = 0; i < 100; i++)
        {
            Student target = unsorted[random.Next(unsorted.Length)];
            if (Search.BinarySearch(unsorted, target, Cmp, out _) < 0) misses++;
        }
        r.Line($"4. Невідсортовані записи: з 100 запитів гарантовано наявних балів не знайдено {misses}."); r.Line();
    }
    private static void Boundaries(Report r, Student[] a)
    {
        r.Line("## 3. Межі"); var target = new Student("", "", 51);
        int lower = Search.Boundary(a, x => Cmp(x, target) >= 0, out int lc);
        int upper = Search.Boundary(a, x => Cmp(x, target) > 0, out int uc);
        var lastValue = new Student("", "", 61);
        int end = Search.Boundary(a, x => Cmp(x,lastValue) > 0, out int rangeChecks);
        int equal = 0, within = 0;
        foreach (var item in a)
        {
            if (item.Score == target.Score) equal++;
            if (item.Score >= target.Score && item.Score <= lastValue.Score) within++;
        }
        Bench.Check(upper-lower == equal && end-lower == within, "Пошук меж");
        r.Line($"X: [{lower}, {upper}); повторів {equal}; порівнянь {lc+uc}.");
        r.Line($"[51;61]: {end-lower} елементів, {lc+rangeChecks} порівнянь. Лінійно перевірили {a.Length} записів.");
        r.Line("Кількість порівнянь залежить від довжини вікна, яке ділиться навпіл, а довжина блоку повторів не впливає на число кроків.");
        int boundary = Search.Boundary(a, s => s.Score % 7 == 0, out int c);
        int actual = 0; while (actual < a.Length && a[actual].Score % 7 != 0) actual++;
        r.Line($"Немонотонна умова Score % 7 == 0: межа={boundary}, лінійна відповідь={actual}, збіг={boundary == actual}, перевірок={c}. Це не коректний критерій для пошуку межі."); r.Line();
    }
    private static void Edges(Report r)
    {
        r.Line("## Крайові випадки"); r.Line("| Вхід / запит | Очікувано: Binary, Lower, Upper | Отримано |"); r.Line("|---|---|---|");
        Edge(r, Array.Empty<int>(), 1, -1, 0, 0);
        Edge(r, new[] { 5 }, 5, 0, 0, 1);
        Edge(r, new[] { 1, 2 }, 2, 1, 1, 2);
        Edge(r, new[] { 3, 4 }, 1, -1, 0, 0);
        Edge(r, new[] { 3, 4 }, 5, -1, 2, 2);
        Edge(r, new[] { 7, 7, 7 }, 7, 1, 0, 3); r.Line();
    }
    private static void Edge(Report r, int[] a, int x, int eb, int el, int eu)
    {
        int b = Search.BinarySearch(a, x, (p, q) => p.CompareTo(q), out _);
        int l = Search.Boundary(a, v => v >= x, out _), u = Search.Boundary(a, v => v > x, out _);
        int rec = Search.RecursiveSearch(a, x, (p, q) => p.CompareTo(q), out _);
        Bench.Check(b == eb && l == el && u == eu && rec == b, "Крайовий пошук");
        r.Line($"| [{string.Join(",", a)}], X={x} | {eb}, {el}, {eu} | {b}, {l}, {u}; рекурсивний {rec}, OK |");
    }
}
