using System.Collections;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

int MinWaitMs = 500;
int MaxWaitMs = 5000;
int ResultsToShow = 7;
List<long> results = new();

LoadSettings();

Console.WriteLine("Тест на реакцию. Нажмите Ctrl+C для выхода.\n");
while (true)
{
    Console.WriteLine("Нажмите любую клавишу, чтобы начать раунд...");
    Console.ReadKey(intercept: true);

    var round = await ReactionTest();

    if (!round.Success)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteColored($"Слишком рано! Раунд не засчитан. Попробуйте ещё раз.\n\n", ConsoleColor.Yellow);
        Console.ResetColor();
        continue;
    }

    Console.Write($"Ваше время реакции: ");
    WriteColored($"{round.Elapsed} мс\n", ConsoleColor.Green);
    Console.ResetColor();

    results.Add(round.Elapsed);

    if (results.Count >= ResultsToShow)
    {
        PrintStatistics();
        results.Clear();
        Console.WriteLine("Результаты очищены. Можно начать новый цикл.\n");
    }
}

async Task <(bool Success, long Elapsed)> ReactionTest()
{
    int wait = Random.Shared.Next(Math.Abs(MinWaitMs), Math.Abs(MaxWaitMs));
    Stopwatch sw = new Stopwatch();

    Console.Clear();
    Console.WriteLine("Ждите... Не нажимайте клавиши до сигнала.");

    Console.ForegroundColor = ConsoleColor.Red;
    int checkInterval = 20;
    int waited = 0;
    while (waited < wait)
    {
        if (Console.KeyAvailable)
        {
            Console.ReadKey(intercept: true);
            return (false, 0);
        }
        await Task.Delay(checkInterval);
        waited += checkInterval;
    }

    Console.WriteLine("СЕЙЧАС! Нажмите любую клавишу как можно быстрее...");
    sw.Start();
    Console.ReadKey(intercept: true);
    sw.Stop();
    Console.ResetColor();

    return (true, sw.ElapsedMilliseconds);
}

void PrintStatistics()
{
    int index = 0;

    Dictionary<string, double> stats = new()
    {
        { "Среднее время: ", results.Average() },
        { "Медианное время: ", GetMedian(results) },
        { "Лучшее время: ", results.Min() },
        { "Худшее время: ", results.Max() },
        { "Стандартное отклонение: ", GetStdDev(results) },
    };

    List<ConsoleColor> statNames = new()
    {
        ConsoleColor.Blue,
        ConsoleColor.DarkCyan,
        ConsoleColor.Green,
        ConsoleColor.DarkRed,
        ConsoleColor.Magenta
    };

    Console.Write("--- ");
    WriteColored($"Статистика", ConsoleColor.Cyan);
    Console.WriteLine(" ---");

    for (int i = 0; i < results.Count; i++)
    {
        if (i < 9)
            Console.Write(" ");
        Console.Write($"[{i + 1}] ");
        WriteColored($"{results[i]} мс\n", ConsoleColor.DarkYellow);
    }

    Console.WriteLine("------------------");
    foreach (var item in stats)
    {
        Console.Write(item.Key);
        WriteColored($"{item.Value:F1} мс\n", statNames[index]);
        index++;
    }
    Console.WriteLine("------------------\n");
    SaveStatistic(results, stats);
}

double GetMedian(List<long> values)
{
    var sorted = values.OrderBy(x => x).ToList();
    int n = sorted.Count;
    if (n % 2 == 1)
        return sorted[n / 2];
    return (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
}

double GetStdDev(List<long> values)
{
    double avg = values.Average();
    double sumSq = values.Select(v => (v - avg) * (v - avg)).Sum();
    return Math.Sqrt(sumSq / values.Count);
}

void WriteColored(string text, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.Write(text);
    Console.ResetColor();
}

void SaveStatistic(IEnumerable Results, IEnumerable Statistic)
{
    JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
    };

    string configPath = Path.Combine(AppContext.BaseDirectory, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json");

    var stats = new
    {
        Results,
        Statistic
    };

    string json = JsonSerializer.Serialize(stats, JsonOptions);

    File.WriteAllText(configPath, json);
}

void LoadSettings()
{
    try
    {
        string configPath = Path.Combine(AppContext.BaseDirectory, "config.json");

        if (!File.Exists(configPath))
        {
            var config = new Config(MinWaitMs, MaxWaitMs, ResultsToShow);
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
            string json = JsonSerializer.Serialize(config, jsonOptions);
            File.WriteAllText(configPath, json);
            return;
        }

        string jsonContent = File.ReadAllText(configPath);
        var loadedConfig = JsonSerializer.Deserialize<Config>(jsonContent);

        if (loadedConfig != null)
        {
            MinWaitMs = loadedConfig.MinWaitMs;
            MaxWaitMs = loadedConfig.MaxWaitMs;
            ResultsToShow = loadedConfig.ResultsToShow;
        }
        else
        {
            Console.WriteLine("Конфиг повреждён, использую дефолтные значения.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при загрузке настроек: {ex.Message}");
    }
}

class Config
{
    public int MinWaitMs { get; set; }
    public int MaxWaitMs { get; set; }
    public int ResultsToShow { get; set; }
    public Config(int MinWaitMs, int MaxWaitMs, int ResultsToShow)
    {
        this.MinWaitMs = MinWaitMs;
        this.MaxWaitMs = MaxWaitMs;
        this.ResultsToShow = ResultsToShow;
    }
}