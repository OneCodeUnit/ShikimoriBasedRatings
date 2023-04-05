using System.Text.Json;

namespace ShikimoriBasedRatings
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Граница для исключения аниме со слишком низким количеством оценок
            int border = 100;
            if (args.Length > 0)
            {
                border = Convert.ToInt32(args[0]);
            }

            // Поиск файла с данными в текущей директории. Он начинается c имени пользователя
            string[] allFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*_animes_database.json", SearchOption.TopDirectoryOnly);
            if (allFiles == null || allFiles.Length == 0)
            {
                Console.WriteLine("Файл с данными не найден");
                return;
            }
            else
            {
                Console.WriteLine($"Загружен файл с данными: {allFiles[0]}");
            }
            string json = File.ReadAllText(allFiles[0]);
            var userTitles = JsonSerializer.Deserialize<List<AnimeData>>(json);

            var sortedList = new List<AnimeData>();
            int countMarks = 0;
            // Отбор значений
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
            foreach (var anime in userTitles)
            {
                countMarks = 0;
                for (int i = 1; i < anime.CommunityScore.Length; i++)
                {
                    countMarks += anime.CommunityScore[i];
                }
                if (countMarks > border)
                {
                    sortedList.Add(anime);
                }
            }

            sortedList.Sort((x, y) => x.DeltaAbs.CompareTo(y.DeltaAbs));
            string stringDeltaAbsMin = $"Больше всего ваше мнение совпадает с сообществом на этом аниме - {sortedList[0].Title} (разница оценок - {Math.Round(sortedList[0].DeltaAbs, 2)})";
            string stringDeltaAbsMax = $"Больше всего вы не согласны с оценкой сообщества на этом аниме - {sortedList[^1].Title} (разница оценок - {Math.Round(sortedList[^1].DeltaAbs, 2)})";

            // Поиск среднего арифметического
            float sum = 0;
            foreach (var anime in sortedList)
            {
                sum += anime.DeltaAbs;
            }
            string stringDeltaAbsBaseIndex = $"Индекс базированности (абсолютный) - {Math.Round(sum / sortedList.Count, 4)}";


            sortedList.Sort((x, y) => x.Delta.CompareTo(y.Delta));
            string stringDeltaMin = $"Аниме, которое больше всего не поняли вы - {sortedList[0].Title} (разница оценок - {Math.Round(sortedList[0].Delta, 2)})";
            string stringDeltaMax = $"Аниме, которое больше всего не поняло сообщество - {sortedList[^1].Title} (разница оценок - {Math.Round(sortedList[^1].Delta, 2)})";

            // Поиск среднего арифметического
            sum = 0;
            foreach (var anime in sortedList)
            {
                sum += anime.Delta;
            }
            string stringDeltaBaseIndex = $"Индекс базированности (относительный) - {Math.Round(sum / sortedList.Count, 4)}";

            // Вычисление процента совпадения оценки с сообществом
            List<(int, float)> userScopePercentList = new();
            int count = 0;
            foreach (var anime in sortedList)
            {
                sum = 0;
                for (int i = 1; i < anime.CommunityScore.Length; i++)
                {
                    sum += anime.CommunityScore[i];
                }
                float percent = anime.CommunityScore[anime.Score];
                if (percent > 0)
                {
                    percent /= sum;
                }
                userScopePercentList.Add((count, percent));
                count++;
            }

            userScopePercentList.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            string stringPercentMin = $"Ваша оценка меньше всего соответствует сообществу у этого аниме  - {sortedList[userScopePercentList[0].Item1].Title} ({Math.Round(userScopePercentList[0].Item2, 4)}%)";
            string stringPercentMax = $"Ваша оценка больше всего соответствует сообществу у этого аниме - {sortedList[userScopePercentList[^1].Item1].Title} ({Math.Round(userScopePercentList[^1].Item2, 4)}%)";

#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
            Console.WriteLine(stringDeltaAbsMin);
            Console.WriteLine(stringDeltaAbsMax);
            Console.WriteLine(stringDeltaAbsBaseIndex);
            Console.WriteLine(stringDeltaMin);
            Console.WriteLine(stringDeltaMax);
            Console.WriteLine(stringDeltaBaseIndex);
            Console.WriteLine(stringPercentMin);
            Console.WriteLine(stringPercentMax);

            StreamWriter resultFile = new("anime.txt", false);
            resultFile.WriteLine(stringDeltaAbsMin);
            resultFile.WriteLine(stringDeltaAbsMax);
            resultFile.WriteLine(stringDeltaAbsBaseIndex);
            resultFile.WriteLine(stringDeltaMin);
            resultFile.WriteLine(stringDeltaMax);
            resultFile.WriteLine(stringDeltaBaseIndex);
            resultFile.WriteLine(stringPercentMin);
            resultFile.WriteLine(stringPercentMax);
            resultFile.Close();
            Console.WriteLine($"Результаты записаны в файл {Directory.GetCurrentDirectory()}\\anime.txt");
            Console.ReadKey();
        }
    }
}