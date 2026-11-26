using System;
using System.Collections.Generic;
using System.Text;

public static class Algorithms
{
    public static List<string> Words(string text)
    {
        var result = new List<string>(); var word = new StringBuilder();
        foreach (char ch in text)
        {
            if (char.IsLetter(ch)) word.Append(char.ToLowerInvariant(ch));
            else if (word.Length > 0) { result.Add(word.ToString()); word.Clear(); }
        }
        if (word.Length > 0) result.Add(word.ToString());
        return result;
    }
    public static Dictionary<T, int> Frequencies<T>(List<T> values) where T : notnull
    {
        var result = new Dictionary<T, int>();
        foreach (T x in values) { result.TryGetValue(x, out int count); result[x] = count + 1; }
        return result;
    }
    // Два паралельні списки: словника й множини тут немає.
    public static (List<T> keys, List<int> counts) FrequenciesList<T>(List<T> values)
    {
        var keys = new List<T>(); var counts = new List<int>();
        foreach (T x in values)
        {
            int i = Position(keys, x);
            if (i < 0) { keys.Add(x); counts.Add(1); } else counts[i]++;
        }
        return (keys, counts);
    }
    public static int Position<T>(List<T> values, T target)
    {
        for (int i = 0; i < values.Count; i++)
            if (EqualityComparer<T>.Default.Equals(values[i], target)) return i;
        return -1;
    }
    public static Dictionary<int, List<Student>> Group(List<Student> students)
    {
        var result = new Dictionary<int, List<Student>>();
        foreach (var s in students)
        {
            if (!result.TryGetValue(s.Name.Length, out var group)) { group = new List<Student>(); result[s.Name.Length] = group; }
            group.Add(s);
        }
        return result;
    }
    public static (List<int> keys, List<List<Student>> groups) GroupList(List<Student> students)
    {
        var keys = new List<int>(); var groups = new List<List<Student>>();
        foreach (var s in students)
        {
            int i = Position(keys, s.Name.Length);
            if (i < 0) { i = keys.Count; keys.Add(s.Name.Length); groups.Add(new List<Student>()); }
            groups[i].Add(s);
        }
        return (keys, groups);
    }
    public static double Average(List<Student> students)
    { int sum = 0; foreach (var s in students) sum += s.Score; return students.Count == 0 ? 0 : (double)sum / students.Count; }
    public static HashSet<int> UniqueUnordered(List<int> values) => new HashSet<int>(values);
    public static List<int> UniqueOrdered(List<int> values)
    {
        var seen = new HashSet<int>(); var result = new List<int>();
        foreach (int x in values) if (seen.Add(x)) result.Add(x);
        return result;
    }
    public static List<int> UniqueList(List<int> values)
    {
        var result = new List<int>(); foreach (int x in values) if (Position(result, x) < 0) result.Add(x);
        return result;
    }
    public static HashSet<int> Intersection(List<int> a, List<int> b)
    { var result = new HashSet<int>(a); result.IntersectWith(b); return result; }
    public static List<int> IntersectionList(List<int> a, List<int> b)
    {
        var result = new List<int>();
        foreach (int x in a) if (Position(b, x) >= 0 && Position(result, x) < 0) result.Add(x);
        return result;
    }
    public static HashSet<int> Extras(List<int> first, List<int> second)
    {
        var known = new HashSet<int>(second);
        var result = new HashSet<int>();
        foreach (int item in first)
            if (!known.Contains(item))
                result.Add(item);
        return result;
    }

    public static List<int> ExtrasList(List<int> first, List<int> second)
    {
        var result = new List<int>();
        foreach (int item in first)
            if (Position(second, item) < 0 && Position(result, item) < 0)
                result.Add(item);
        return result;
    }

    public static bool Same(List<int> a, List<int> b)
    {
        if (a.Count != b.Count) return false;
        foreach (int x in a) if (Position(b, x) < 0) return false;
        return true;
    }
}

