using System;

public static class Search
{
    public static int LinearSearch<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
    {
        comparisons = 0;
        for (int i = 0; i < a.Length; i++) { comparisons++; if (cmp(a[i], target) == 0) return i; }
        return -1;
    }
    public static int BinarySearch<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
    {
        int lo = 0, hi = a.Length - 1; comparisons = 0;
        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2; comparisons++; int c = cmp(a[mid], target);
            if (c == 0) return mid;
            if (c < 0) lo = mid + 1; else hi = mid - 1;
        }
        return -1;
    }
    // Умова повинна мати вигляд false...false,true...true.
    public static int Boundary<T>(T[] a, Func<T, bool> condition, out int comparisons)
    {
        int lo = 0, hi = a.Length; comparisons = 0;
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2; comparisons++;
            if (condition(a[mid])) hi = mid; else lo = mid + 1;
        }
        return lo;
    }
    public static int RecursiveSearch<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
        => RecursiveSearch(a, target, cmp, out comparisons, out _);
    public static int RecursiveSearch<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons, out int depth)
    { comparisons = 0; depth = 0; return Recurse(a, target, cmp, 0, a.Length - 1, 1, ref comparisons, ref depth); }
    private static int Recurse<T>(T[] a, T target, Func<T, T, int> cmp, int lo, int hi, int level, ref int comparisons, ref int depth)
    {
        depth = Math.Max(depth, level);
        if (lo > hi) return -1;
        int mid = lo + (hi - lo) / 2; comparisons++; int c = cmp(a[mid], target);
        if (c == 0) return mid;
        return c < 0 ? Recurse(a, target, cmp, mid + 1, hi, level + 1, ref comparisons, ref depth)
                     : Recurse(a, target, cmp, lo, mid - 1, level + 1, ref comparisons, ref depth);
    }
    public static int Exponential<T>(T[] data, T target, Func<T,T,int> compare, out int comparisons)
    {
        comparisons = 0;
        if (data.Length == 0) return -1;
        comparisons++;
        int first = compare(data[0],target);
        if (first >= 0) return first == 0 ? 0 : -1;
        int lower = 1, step = 1;
        while (step < data.Length)
        {
            comparisons++;
            int order = compare(data[step],target);
            if (order == 0) return step;
            if (order > 0) break;
            lower = step+1;
            step = step <= (data.Length-1)/2 ? step*2 : data.Length;
        }
        int upper = Math.Min(step-1,data.Length-1);
        while (lower <= upper)
        {
            int mid = lower+(upper-lower)/2;
            comparisons++;
            int order = compare(data[mid],target);
            if (order == 0) return mid;
            if (order < 0) lower = mid+1;
            else upper = mid-1;
        }
        return -1;
    }
}
