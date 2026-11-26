using System;

public delegate void SortAlgorithm<T>(T[] a, Func<T, T, int> cmp, out long comparisons, out long moves);

public static class Sorts
{
    public static void Bubble<T>(T[] a, Func<T, T, int> cmp, out long comparisons, out long moves)
    {
        comparisons = 0; moves = 0;
        for (int end = a.Length - 1; end > 0; end--)
        {
            bool swapped = false;
            for (int i = 0; i < end; i++)
            {
                comparisons++;
                if (cmp(a[i], a[i + 1]) > 0)
                {
                    T temp = a[i]; a[i] = a[i + 1]; a[i + 1] = temp;
                    moves += 2; // Лише два записи в масив; локальна змінна не є записом у масив.
                    swapped = true;
                }
            }
            if (!swapped) break;
        }
    }
    public static void Shell<T>(T[] data, Func<T,T,int> compare, out long comparisons, out long moves)
    {
        comparisons = moves = 0;
        for (int gap = data.Length / 2; gap > 0; gap /= 2)
        {
            for (int i = gap; i < data.Length; i++)
            {
                T value = data[i];
                int j = i;
                while (j >= gap)
                {
                    comparisons++;
                    if (compare(data[j-gap],value) <= 0)
                        break;
                    data[j] = data[j-gap];
                    moves++;
                    j -= gap;
                }
                if (j != i)
                {
                    data[j] = value;
                    moves++;
                }
            }
        }
    }

    public static void Insertion<T>(T[] data, Func<T, T, int> compare, out long comparisons, out long moves)
    {
        comparisons = moves = 0;
        for (int i = 1; i < data.Length; i++)
        {
            T value = data[i];
            int j = i;
            while (j > 0)
            {
                comparisons++;
                if (compare(data[j - 1], value) <= 0)
                    break;
                data[j] = data[j - 1];
                moves++;
                j--;
            }
            if (j != i)
            {
                data[j] = value;
                moves++;
            }
        }
    }

    public static void QuickMedian<T>(T[] data, Func<T, T, int> compare, out long comparisons, out long moves)
    {
        long checks = 0, writes = 0;
        Sort(0, data.Length - 1);
        comparisons = checks;
        moves = writes;

        int Compare(T x, T y)
        {
            checks++;
            return compare(x, y);
        }

        void Swap(int i, int j)
        {
            if (i == j) return;
            (data[i], data[j]) = (data[j], data[i]);
            writes += 2;
        }

        T Median(T a, T b, T c)
        {
            if (Compare(a, b) > 0) (a, b) = (b, a);
            if (Compare(b, c) > 0) b = c;
            return Compare(a, b) > 0 ? a : b;
        }

        void Sort(int lo, int hi)
        {
            while (lo < hi)
            {
                T pivot = Median(data[lo], data[lo + (hi - lo) / 2], data[hi]);
                int lt = lo, i = lo, gt = hi;
                while (i <= gt)
                {
                    int order = Compare(data[i], pivot);
                    if (order < 0) Swap(lt++, i++);
                    else if (order > 0) Swap(i, gt--);
                    else i++;
                }

                if (lt - lo < hi - gt)
                {
                    Sort(lo, lt - 1);
                    lo = gt + 1;
                }
                else
                {
                    Sort(gt + 1, hi);
                    hi = lt - 1;
                }
            }
        }
    }

}
