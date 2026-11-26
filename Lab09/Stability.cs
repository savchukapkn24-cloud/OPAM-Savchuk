using System;

public static class Stability
{
    // Очікування зафіксовані до запуску експерименту.
    private static readonly string[] Expected = { "стабільне", "не гарантує", "не гарантує", "не гарантує", "стабільне" };
    public static void Run(Report r)
    {
        r.Line("## 5. Стабільність");
        r.Line("Початкові 12 студентів мають різні бали, тому самі не виявляють нестабільність. Додатково створено 96 записів із повторюваними прізвищами та балами 60, 70, 80, 90; перед кожним дослідом — упорядкування прізвищ за StringComparer.Ordinal.");
        var original = Data.Students().ToArray();
        Array.Sort(original, (x, y) => StringComparer.Ordinal.Compare(x.Name, y.Name));
        r.Line("| Алгоритм | Очікувано до запуску | Початкові 12 | 96 із рівними балами | Перший контрприклад |"); r.Line("|---|---|---|---|---|");
        var source = Data.Students(); var extra = new Student[96];
        for (int i = 0; i < extra.Length; i++) { var s = source[i % source.Count]; extra[i] = new Student(s.Name + "_" + i.ToString("D2"), s.Group, 60 + (i % 4) * 10); }
        Array.Sort(extra, (x, y) => StringComparer.Ordinal.Compare(x.Name, y.Name));
        string[] names = { "Бульбашка", "Швидке", "Шелл", "Array.Sort", "OrderBy (лише еталон)" };
        for (int algorithm = 0; algorithm < names.Length; algorithm++)
        {
            var basic = Sort(original, algorithm); var sorted = Sort(extra, algorithm);
            Checks.Verify(original, basic, (x, y) => x.Score.CompareTo(y.Score)); Checks.Verify(extra, sorted, (x, y) => x.Score.CompareTo(y.Score));
            string counter = Counterexample(sorted);
            r.Line($"| {names[algorithm]} | {Expected[algorithm]} | {Counterexample(basic)} | {(counter == "немає" ? "порядок збережено" : "порядок порушено")} | {counter} |");
            if (algorithm is 0 or 4) Bench.Check(counter == "немає", "Стабільність");
        }
        r.Line("Відсутність контрприкладу в окремому досліді не доводить стабільність Array.Sort. OrderBy викликано лише в еталоні цієї таблиці: це прямо вимагає завдання 5; у власних алгоритмах LINQ немає."); r.Line();
    }
    private static Student[] Sort(Student[] source, int algorithm)
    {
        var a = (Student[])source.Clone(); Func<Student, Student, int> cmp = (x, y) => x.Score.CompareTo(y.Score);
        switch (algorithm)
        {
            case 0: Sorts.Bubble(a, cmp, out _, out _); break;
            case 1: Sorts.QuickMedian(a, cmp, out _, out _); break;
            case 2: Sorts.Shell(a, cmp, out _, out _); break;
            case 3: Array.Sort(a, (x, y) => cmp(x, y)); break;
            case 4:
                // Виняток з правила заради прямо зазначеного еталона OrderBy.
                a = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.OrderBy(a, s => s.Score)); break;
        }
        return a;
    }
    private static string Counterexample(Student[] a)
    {
        for (int i = 1; i < a.Length; i++)
            if (a[i - 1].Score == a[i].Score && StringComparer.Ordinal.Compare(a[i - 1].Name, a[i].Name) > 0)
                return $"{a[i - 1].Name} перед {a[i].Name}, бал {a[i].Score}";
        return "немає";
    }
}
