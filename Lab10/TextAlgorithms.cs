public static class TextAlgorithms
{
    public static List<string> Words(string text)
    {
        var words = new List<string>();
        int i = 0;
        while (i < text.Length)
        {
            if (!char.IsLetter(text[i]))
            {
                i++;
                continue;
            }
            int start = i++;
            while (i < text.Length && (char.IsLetter(text[i]) ||
                    ((text[i] is '\'' or '’' or 'ʼ') && i + 1 < text.Length && char.IsLetter(text[i + 1]))))
                i++;
            words.Add(text[start..i]);
        }
        return words;
    }

    public static string NormalizeNaive(string text)
    {
        char[] chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
            chars[i] = char.IsWhiteSpace(chars[i]) ? ' ' : char.ToUpperInvariant(chars[i]);

        string input = new string(chars).Trim(' ');
        while (input.IndexOf("  ", StringComparison.Ordinal) >= 0)
            input = input.Replace("  ", " ", StringComparison.Ordinal);

        var result = new System.Text.StringBuilder(input.Length);
        int start = 0;
        while (start < input.Length)
        {
            int end = input.IndexOf(' ', start);
            if (end < 0) end = input.Length;
            if (result.Length > 0) result.Append(' ');
            result.Append(input.Substring(start, end-start));
            start = end+1;
        }
        return result.ToString();
    }

    public static string NormalizeFast(string text)
    {
        var result = new System.Text.StringBuilder(text.Length);
        int read = 0;
        while (read < text.Length)
        {
            while (read < text.Length && char.IsWhiteSpace(text[read]))
                read++;
            int start = read;
            while (read < text.Length && !char.IsWhiteSpace(text[read]))
                read++;
            if (start == read) break;
            if (result.Length > 0) result.Append(' ');
            for (int write = start; write < read; write++)
                result.Append(char.ToUpperInvariant(text[write]));
        }
        return result.ToString();
    }

    public static int IndexOf(string text, string pattern, out int comparisons)
    {
        comparisons = 0;
        for (int start = 0; start <= text.Length - pattern.Length; start++)
        {
            int i = 0;
            while (i < pattern.Length)
            {
                comparisons++;
                if (text[start + i] != pattern[i])
                    break;
                i++;
            }
            if (i == pattern.Length)
                return start;
        }
        return -1;
    }
}
