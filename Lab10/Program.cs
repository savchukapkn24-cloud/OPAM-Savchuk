using System.Globalization;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
const int n = 23, k = 1;
int rod = 30 + k;
string prose = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "prose.txt"));
string reportPath = Path.GetFullPath(args.Length > 0 ? args[0] : "Results.md");
if (prose.Length < 50_000)
    throw new InvalidOperationException("Потрібно 50 000 символів прози.");
using var report = new StreamWriter(reportPath, false, new UTF8Encoding(false));
void Log(string line)
{
    Console.WriteLine(line);
    report.WriteLine(line);
}

Log("# Фактичні результати лабораторної №10");
Log("Release, Stopwatch, один прогрів і мінімум із трьох вимірювань.\n");
Measurements.Pitfalls(Log, TextAlgorithms.Words(prose[..50_000]));
Log("## 2. Нормалізація регістру й пробілів");
Log("| Символів | Replace/Substring, мс | Один прохід, мс | Виграш |");
Log("| ---: | ---: | ---: | ---: |");
double shortNaive = 0, shortFast = 0, fullNaive = 0, fullFast = 0;
int normalizedLength = 0;
foreach (int length in new[] {1000,10000,50000})
{
    string source = prose[..length];
    string slowResult = TextAlgorithms.NormalizeNaive(source);
    string fastResult = TextAlgorithms.NormalizeFast(source);
    Check(slowResult.Equals(fastResult,StringComparison.Ordinal),"Нормалізація");
    double slow = Measurements.Time(() => GC.KeepAlive(TextAlgorithms.NormalizeNaive(source)));
    double fast = Measurements.Time(() => GC.KeepAlive(TextAlgorithms.NormalizeFast(source)));
    if (length == 1000) (shortNaive,shortFast) = (slow,fast);
    if (length == 50000) (fullNaive,fullFast,normalizedLength) = (slow,fast,fastResult.Length);
    Log($"| {length} | {slow:F4} | {fast:F4} | {slow/fast:F2} |");
}
Log("\n## 3. Пошук підрядка");
Log("| Вхід | Шаблон | Індекс | Порівняння | Власний, мс | String.IndexOf, мс |");
Log("| --- | --- | ---: | ---: | ---: | ---: |");
string sample = prose[..50_000];
const string missing = "qzxvнеіснуючийкодовиймаркер";
string present = TextAlgorithms.Words(sample[25_000..])[1];
SearchRow("Проза, є збіг", sample, present, present);
SearchRow("Проза, без збігу", sample, missing, missing);
SearchRow("Повтори", new string('А', 10_000), new string('А', 909) + 'Б', "909 × А та Б");
Log("\nНа повторах майже кожен кандидат збігається на 909 позиціях. Вбудований пошук може перевіряти блоки символів і відкидати кандидатів швидше.\n");

Log($"## 4. Стрижень {rod} см");
var random = new Random(n);
int optimal = 0, minimalSize = int.MaxValue;
double worstGap = 0;
int[] counterexample = [];
int counterGreedy = 0, counterBest = 0;
for (int test = 0; test < 100; test++)
{
    int[] parts = Cutting.Parts(random, random.Next(1, 16));
    int greedy = Cutting.Greedy(rod, parts);
    int best = Cutting.Exhaustive(rod, parts, false).Best;
    Check(best == Cutting.Exhaustive(rod, parts, true).Best, "Відсікання");
    if (greedy == best) optimal++;
    else
    {
        worstGap = Math.Max(worstGap, 100.0 * (best - greedy) / best);
        if (parts.Length < minimalSize)
        {
            minimalSize = parts.Length;
            counterexample = parts;
            counterGreedy = greedy;
            counterBest = best;
        }
    }
}
Log($"- Оптимум жадібного алгоритму: {optimal}/100.");
Log($"- Найгірше відставання від максимальної використаної довжини: {worstGap:F2}%.");
Log($"- Найменший знайдений контрприклад за числом деталей: [{string.Join(", ", counterexample)}].");
Log($"- Використана довжина для нього: жадібно {counterGreedy} см, оптимально {counterBest} см.");
foreach (int count in new[] { 15, 20 })
{
    var parts = Cutting.Parts(random, count);
    var exhaustive = Cutting.Exhaustive(rod, parts, false);
    var pruned = Cutting.Exhaustive(rod, parts, true);
    Check(exhaustive.Best == pruned.Best, "Оптимум відсікання");
    double time = Measurements.Time(() => GC.KeepAlive(Cutting.Exhaustive(rod, parts, false)));
    double prunedTime = Measurements.Time(() => GC.KeepAlive(Cutting.Exhaustive(rod, parts, true)));
    Log($"- {count} деталей: без відсікання {exhaustive.Nodes} вузлів, {time:F4} мс. З відсіканням {pruned.Nodes} вузлів, {prunedTime:F4} мс.");
}

Log("\n## 5. Вибір алгоритму");
Log($"1. Фрагмент прози має 50 000 символів, результат після нормалізації — {normalizedLength} символів.");
Log("2. Запит один: привести регістр і розділювачі до єдиного формату.");
Log("3. Текст статичний протягом запиту. Наступний фрагмент потребує власної нормалізації.");
Log($"4. На 50 000 символів різниця часу становить {Math.Abs(fullNaive-fullFast):F4} мс. Під час пакетної обробки також мають значення проміжні рядки.");
Log("5. Вбудовані засоби: Replace, Substring, StringBuilder та Char.IsWhiteSpace.");
Log($"6. На 1 000 символів: {shortNaive:F4} і {shortFast:F4} мс. На 50 000: {fullNaive:F4} і {fullFast:F4} мс. Для довгих текстів обираю прохід зі StringBuilder, бо він не створює рядок для кожного слова.");
Log("\n## Читання 100 000 рядків");
Measurements.CompareReading(Log);

Log("\n## Крайові випадки");
Log("| Вхід | Очікувано | Отримано |");
Log("| --- | --- | --- |");
foreach (var test in new[] { ("",""),("а","А"),("ааа","ААА"),("  а  б\tу  ","А Б У") })
{
    string fast = TextAlgorithms.NormalizeFast(test.Item1);
    Check(fast.Equals(test.Item2,StringComparison.Ordinal) &&
          fast.Equals(TextAlgorithms.NormalizeNaive(test.Item1),StringComparison.Ordinal),"Крайовий текст");
    Log($"| «{test.Item1.Replace("\t","[TAB]",StringComparison.Ordinal)}» | «{test.Item2}» | «{fast}» |");
}
foreach (var test in new[] { ("", "В", -1), ("В", "ВВ", -1), ("В", "", 0), ("", "", 0) })
{
    int index = TextAlgorithms.IndexOf(test.Item1, test.Item2, out _);
    Check(index == test.Item3, "Крайовий пошук");
    Log($"| «{test.Item2}» у «{test.Item1}» | {test.Item3} | {index} |");
}
foreach (int[] parts in new int[][] { [], [5] })
{
    int goal = parts.Length == 0 ? 0 : 5;
    int result = Cutting.Exhaustive(rod, parts, true).Best;
    Check(result == goal && result == Cutting.Greedy(rod, parts), "Крайовий розкрій");
    Log($"| Деталей {parts.Length} | {goal} см | {result} см |");
}

void SearchRow(string label, string text, string pattern, string shown)
{
    int index = TextAlgorithms.IndexOf(text, pattern, out int comparisons);
    Check(index == text.IndexOf(pattern, StringComparison.Ordinal), "Пошук підрядка");
    double naive = Measurements.Time(() => GC.KeepAlive(TextAlgorithms.IndexOf(text, pattern, out _)));
    double standard = Measurements.Time(() => GC.KeepAlive(text.IndexOf(pattern, StringComparison.Ordinal)));
    Log($"| {label} | {shown} | {index} | {comparisons} | {naive:F4} | {standard:F4} |");
}

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
