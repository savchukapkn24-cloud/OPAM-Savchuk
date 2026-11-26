public static class Cutting
{
    public static int Greedy(int length, int[] parts)
    {
        var sorted = (int[])parts.Clone();
        Array.Sort(sorted);
        int chosen = 0;
        for (int i = sorted.Length - 1; i >= 0; i--)
            if (sorted[i] <= length - chosen)
                chosen += sorted[i];
        return chosen;
    }

    public static (int Best, long Nodes) Exhaustive(int length, int[] parts, bool prune)
    {
        int best = 0;
        long visited = 0;
        var suffix = new int[parts.Length + 1];
        for (int i = parts.Length - 1; i >= 0; i--)
            suffix[i] = suffix[i + 1] + parts[i];
        Walk(0, 0);
        return (best, visited);

        void Walk(int i, int total)
        {
            visited++;
            if (prune && (total > length || best == length || total + suffix[i] <= best))
                return;
            if (i == parts.Length)
            {
                if (total <= length && total > best)
                    best = total;
                return;
            }
            Walk(i + 1, total + parts[i]);
            Walk(i + 1, total);
        }
    }

    public static int[] Parts(Random random, int length)
    {
        var parts = new int[length];
        for (int i = 0; i < parts.Length; i++)
            parts[i] = random.Next(1, 17);
        return parts;
    }
}
