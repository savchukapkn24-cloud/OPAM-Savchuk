using System;
using System.Collections.Generic;

public static class Checks
{
    public static bool IsSorted<T>(T[] a, Func<T, T, int> cmp)
    { for (int i = 1; i < a.Length; i++) if (cmp(a[i - 1], a[i]) > 0) return false; return true; }
    public static bool IsPermutationOf<T>(T[] original, T[] sorted) where T : notnull
    {
        if (original.Length != sorted.Length) return false;
        var counts = new Dictionary<T, int>();
        foreach (T x in original) { counts.TryGetValue(x, out int n); counts[x] = n + 1; }
        foreach (T x in sorted)
        {
            if (!counts.TryGetValue(x, out int n) || n == 0) return false;
            counts[x] = n - 1;
        }
        foreach (var p in counts) if (p.Value != 0) return false;
        return true;
    }
    public static void Verify<T>(T[] original, T[] sorted, Func<T, T, int> cmp) where T : notnull
    { Bench.Check(IsSorted(sorted, cmp) && IsPermutationOf(original, sorted), "Порушено порядок або перестановку"); }
}
