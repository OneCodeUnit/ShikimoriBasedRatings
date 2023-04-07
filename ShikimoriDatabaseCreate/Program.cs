using ShikimoriDatabaseCreate.JsonClasses.Anime;
using ShikimoriDatabaseCreate.JsonClasses.List;
using ShikimoriDatabaseCreate.JsonClasses.Program;
using System.Globalization;
using System.Text.Json;

namespace ShikimoriDatabaseCreate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Получение ид пользователя по вводимому нику
            string? nickname = string.Empty;
            int userId = 0;
            if (args.Length > 0)
            {
                if (args[0] is not null)
                {
                    nickname = args[0];
                    userId = ShikimoriHttpClient.CheckUser(nickname);
                }
            }
            if (userId == -1)
            {
                Console.WriteLine("Пользователь не найден");
            }
            if (userId < 1)
            {
                do
                {
                    Console.WriteLine("Введите имя пользователя:");
                    nickname = Console.ReadLine();
                    if (nickname is null)
                        userId = -1;
                    else
                        userId = ShikimoriHttpClient.CheckUser(nickname);
                }
                while (userId < 1);
            }

            // Получение данных пользователя
            string userData = ShikimoriHttpClient.GetUserRatings(userId);
            if (userData.StartsWith("Error"))
            {
                Console.WriteLine(userData);
                return;
            }
            Console.WriteLine($"Данные пользователя {nickname} получены");
            var json = JsonSerializer.Deserialize<List<ShikimoriListJson>>(userData);

#pragma warning disable CS8602, CS8600
            int iter = 0;
            int errorIter = 0;
            int maxIter = json.Count;
            int delayMin = 400;
            int delayMax = 600;
            List<AnimeTitle> titles = new();

            double estimatedTime = Math.Round((double)maxIter * delayMax / 60000);
            Console.WriteLine($"В статистику можно добавить оценки пользователей, но это займёт до {estimatedTime} минут времени. Добавить? [y/n]");
            string answer = Console.ReadLine();
            bool mode = false;
            if (answer.Equals("y") || answer.Equals("д") || answer.Equals("н"))
                mode = true;


            foreach (var anime in json)
            {
                if (anime.score == 0)
                {
                    iter++;
                    Console.WriteLine($"{anime.anime.name} - Skip. {iter}/{maxIter}");
                    continue;
                }
                float onlineRating = Convert.ToSingle(anime.anime.score, CultureInfo.InvariantCulture);
                float delta = Math.Abs(anime.score - onlineRating);
                float deltaAlt = anime.score - onlineRating;
                AnimeTitle title;
                if (mode)
                {
                    string communityData = ShikimoriHttpClient.GetCommunityRatings(anime.anime.id);
                    if (communityData.StartsWith("Error"))
                    {
                        Console.WriteLine(communityData);
                        iter++;
                        errorIter++;
                        continue;
                    }
                    var data = JsonSerializer.Deserialize<ShikimoriAnimeJson>(communityData);
                    List<RatesScoresStat> scopes = new(data.rates_scores_stats);
                    int[] communityScore = new int[11];
                    foreach (var scope in scopes)
                    {
                        communityScore[scope.name] = scope.value;
                    }
                    title = new(anime.anime.id, anime.score, anime.anime.name, delta, deltaAlt, communityScore);
                    iter++;
                    Console.WriteLine($"{anime.anime.name} - OK. {iter}/{maxIter}");
                    Random rand = new();
                    int delay = rand.Next(delayMin, delayMax);
                    Thread.Sleep(delay);
                }
                else
                {
                    title = new(anime.anime.id, anime.score, anime.anime.name, delta, deltaAlt, Array.Empty<int>());
                }
                titles.Add(title);
            }
#pragma warning restore CS8602, CS8600

            Console.WriteLine($"Данные созданы. Ошибок {errorIter}. Обработано {json.Count} аниме");
            string dataFile = nickname + "_database.json";
            StreamWriter sw = new(dataFile, false);
            sw.Write(JsonSerializer.Serialize(titles));
            sw.Close();
            Console.WriteLine($"Данные записаны в файл: {Directory.GetCurrentDirectory()}\\{dataFile}");
            Console.WriteLine("Нажмите любую клавишу для закрытия окна");
            Console.ReadKey();
        }
    }
}
