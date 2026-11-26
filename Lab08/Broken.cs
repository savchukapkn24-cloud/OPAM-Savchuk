using System;

// Навмисні помилки лише для експерименту завдання 2.
public static class Broken
{
    public static int StrictLoop<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
    {
        int lo = 0, hi = a.Length - 1; comparisons = 0;
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2; comparisons++; int c = cmp(a[mid], target);
            if (c == 0) return mid;
            if (c < 0) lo = mid + 1; else hi = mid - 1;
        }
        return -1;
    }
    public static int Stuck<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons, out int lastLo, out int lastHi)
    {
        int lo = 0, hi = a.Length - 1; comparisons = 0;
        while (lo <= hi && comparisons < 100)
        {
            int mid = lo + (hi - lo) / 2; comparisons++; int c = cmp(a[mid], target);
            if (c == 0) { lastLo = lo; lastHi = hi; return mid; }
            if (c < 0) lo = mid; else hi = mid - 1;
        }
        lastLo = lo; lastHi = hi; return -1;
    }
    public static int UnsafeMidpoint<T>(T[] a, T target, Func<T, T, int> cmp, out int comparisons)
    {
        int lo = 0, hi = a.Length - 1; comparisons = 0;
        while (lo <= hi)
        {
            int mid = unchecked((lo + hi) / 2); comparisons++; int c = cmp(a[mid], target);
            if (c == 0) return mid;
            if (c < 0) lo = mid + 1; else hi = mid - 1;
        }
        return -1;
    }
}
