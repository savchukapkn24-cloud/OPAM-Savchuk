using System;
using System.Collections.Generic;

public static class Program
{
    public const string Text = "Алгоритм описує послідовність дій. Алгоритм можна записати словами, а можна записати кодом. Кодом алгоритм стає тоді, коли його виконує машина, а не людина. Одна задача — багато алгоритмів.";
    public static void Main()
    {
        var r = new Report(); r.Line("# Результати лабораторної 6"); r.EnvironmentInfo();
        Frequencies(r); Groups(r); Sets(r); Edges(r); Measurements.Run(r);
        r.Save("Results.md");
    }
    private static void Frequencies(Report r)
    {
        r.Line("## 1. Частоти довжин слів"); r.Line(Text); r.Line();
        var letters = WordLengths(Text);
        var dictionary = Algorithms.Frequencies(letters);
        var list = Algorithms.FrequenciesList(letters);
        Bench.Check(dictionary.Count == list.keys.Count, "Кількість довжин");
        foreach (var entry in dictionary)
        {
            int i = Algorithms.Position(list.keys, entry.Key);
            Bench.Check(i >= 0 && list.counts[i] == entry.Value, "Частоти різні");
            if (entry.Value >= 3)
                r.Line($"{entry.Key}: {entry.Value}");
        }
        r.Line($"Різних довжин: {dictionary.Count}. Обидві реалізації збігаються.");
    }

    private static List<int> WordLengths(string text)
    {
        var result = new List<int>();
        foreach (var word in Algorithms.Words(text))
            result.Add(word.Length);
        return result;
    }

    private static void Groups(Report r)
    {
        r.Line("## 2. Групування"); var students = Data.Students();
        var d = Algorithms.Group(students); var l = Algorithms.GroupList(students);
        Bench.Check(d.Count == l.keys.Count, "Кількість груп");
        foreach (var item in d)
        {
            int i = Algorithms.Position(l.keys, item.Key);
            Bench.Check(i >= 0 && item.Value.Count == l.groups[i].Count, "Розміри груп");
            for (int j = 0; j < item.Value.Count; j++) Bench.Check(ReferenceEquals(item.Value[j], l.groups[i][j]), "Склад груп");
            Bench.Check(Algorithms.Average(item.Value) == Algorithms.Average(l.groups[i]), "Середній бал");
            r.Line($"{item.Key}: {item.Value.Count} студентів, середній бал {Bench.F(Algorithms.Average(item.Value))}");
        }
        r.Line("Спискова й словникова реалізації збігаються."); r.Line();
    }
    private static void Sets(Report r)
    {
        r.Line("## 3. Дублікати"); var values = new List<int>(Data.Numbers(25, 23, 1, 7));
        var unordered = Algorithms.UniqueUnordered(values); var ordered = Algorithms.UniqueOrdered(values); var list = Algorithms.UniqueList(values);
        Bench.Check(Algorithms.Same(new List<int>(unordered), ordered) && Algorithms.Same(list, ordered), "Унікальні елементи");
        for (int i = 0; i < list.Count; i++) Bench.Check(list[i] == ordered[i], "Порядок");
        r.Line("Вхід: " + string.Join(", ", values)); r.Line("HashSet: " + string.Join(", ", unordered));
        r.Line("Із порядком: " + string.Join(", ", ordered)); r.Line();
        r.Line("## 4. Підмножина"); var a = new List<int>(Data.Numbers(30, 23, 1, 21)); var b = new List<int>(Data.Numbers(30, 123, 1, 21));
        var hs = Algorithms.Extras(a, b); var ls = Algorithms.ExtrasList(a, b);
        Bench.Check(Algorithms.Same(new List<int>(hs), ls), "Різні зайві елементи");
        r.Line("A: " + string.Join(", ", a)); r.Line("B: " + string.Join(", ", b));
        r.Line("Зайві в A: " + string.Join(", ", hs)); r.Line($"Зайвих: {hs.Count}; A є підмножиною B: {hs.Count == 0}."); r.Line();
    }
    private static void Edges(Report r)
    {
        r.Line("## Крайові випадки"); r.Line("| Вхід | Очікувано | Отримано |"); r.Line("|---|---|---|");
        Bench.Check(Algorithms.Frequencies(WordLengths("")).Count == 0, "Порожній текст");
        r.Line("| Порожній текст | 0 довжин | 0 |");
        var letters = Algorithms.Frequencies(WordLengths("а ми три"));
        Bench.Check(letters.Count == 3, "Усі довжини різні");
        r.Line("| а ми три | 3 різні довжини | 3 |");
        var s = new List<Student> { new Student("А", "КН-3/1", 60), new Student("Б", "КН-3/2", 80) };
        Bench.Check(Algorithms.Group(s).Count == 1 && Algorithms.GroupList(s).groups.Count == 1 && Algorithms.Average(s) == 70, "Одна група за довжиною");
        r.Line("| Прізвища з однієї літери | 1 група, середній 70 | 1; 70 |");
        var unique = new List<int> { 1, 2, 3 }; Bench.Check(Algorithms.UniqueUnordered(unique).Count == 3 && Algorithms.Same(Algorithms.UniqueOrdered(unique), unique) && Algorithms.Same(Algorithms.UniqueList(unique), unique), "Без дублікатів");
        r.Line("| Без дублікатів | 1, 2, 3 | усі способи: 1, 2, 3, OK |");
        var other = new List<int> { 4, 5 }; Bench.Check(Algorithms.Extras(unique, other).Count == 3 && Algorithms.ExtrasList(unique, other).Count == 3, "Немає перетину");
        r.Line("| Набори не перетинаються | 3 зайві елементи | обидва способи: 3, OK |");
        Bench.Check(Algorithms.Extras(unique, unique).Count == 0 && Algorithms.ExtrasList(unique, unique).Count == 0, "Однакові набори");
        r.Line("| Однакові набори | 0 зайвих | обидва способи: 0, OK |"); r.Line();
    }
}
