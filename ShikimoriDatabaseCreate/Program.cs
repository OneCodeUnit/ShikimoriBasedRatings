using System.Text.Json;

namespace ShikimoriDatabaseCreate
{
    internal class Program
    {
        static void Main()
        {
            // Поиск файла с данными пользователя в текущей директории. Он начинается c имени пользователя
            string[] allFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*_animes.json", SearchOption.TopDirectoryOnly);
            if (allFiles == null || allFiles.Length == 0)
            {
                Console.WriteLine("Файл с данными пользователя не найден");
                return;
            }
            else
            {
                Console.WriteLine($"Загружен файл с данными пользователя: {allFiles[0]}");
            }
            JsonParse.CreateUserData(allFiles[0]);

            // Значения, сгруппированные: ид аниме - твоя оценка
            Dictionary<int, int> UserScopeDictionary = new(JsonParse.UserScopeDictionary);
            // Значения, сгруппированные: ид аниме - его название
            Dictionary<int, string> UserTitleDictionary = new(JsonParse.UserTitleDictionary);
            // Значения, сгруппированные: ид аниме - его название для адреса страницы
            Dictionary<int, string> UserTitleUrlDictionary = new(JsonParse.UserTitleUrlDictionary);
            // Значения, сгруппированные: ид аниме - разница оценок (абсолютная)
            Dictionary<int, float> UserDeltaDictionary = new();
            // Значения, сгруппированные: ид аниме - разница оценок
            Dictionary<int, float> UserDeltaDictionaryAlt = new();
            // Значения, сгруппированные: ид аниме - оценки пользователей
            Dictionary<int, int[]> UserCommunityScoreDictionary = new();

            // 3 параметра для вынесения настройки
            int delayMin = 1200;
            int delayMax = 1800;
            byte maxRetry = 1;
            int iter = 0;
            //int breakIter = 100;
            int errorIter = 0;
            int maxIter = UserTitleDictionary.Count;
            double estimatedTime = Math.Round((double)maxIter * (delayMin + delayMax / 2) / 60000);
            List<AnimeTitle> titles = new();
            Console.WriteLine($"Наберитесь терпения, создание данных займёт примерно {estimatedTime} мин.");
            foreach (var anime in UserTitleDictionary)
            {
                //if (iter == breakIter)
                //{
                //    break;
                //}
                int id = anime.Key;
                string name = anime.Value;
                string page = ShikimoriHttpClient.GetShikimoriRating(id, UserTitleUrlDictionary[id], maxRetry);
                // Если так и не получилось получить html
                if (page.Equals("error"))
                {
                    Console.WriteLine($"{name} - Error (get). {iter}/{maxIter}");
                    iter++;
                    errorIter++;
                    continue;
                }
                float onlineRating = JsonParse.ShikimoriRating(page);
                // Если так и не получилось найти оценку
                if (onlineRating == -1)
                {
                    Console.WriteLine($"{name} - Error (rating). {iter}/{maxIter}");
                    iter++;
                    errorIter++;
                    continue;
                }
                // Если так и не получилось найти оценки пользователей
                int[] onlineCommunityRating = JsonParse.ShikimoriCommunityRating(page);
                if (onlineCommunityRating[0] == -1)
                {
                    Console.WriteLine($"{name} - Error (community rating). {iter}/{maxIter}");
                    iter++;
                    errorIter++;
                    continue;
                }
                UserCommunityScoreDictionary.Add(id, onlineCommunityRating);
                int youRating = UserScopeDictionary[id];
                float delta = Math.Abs(onlineRating - youRating);
                float deltaAlt = youRating - onlineRating;
                UserDeltaDictionary.Add(id, delta);
                UserDeltaDictionaryAlt.Add(id, deltaAlt);
                AnimeTitle title = new(id, UserScopeDictionary[id], UserTitleDictionary[id], UserDeltaDictionary[id], UserDeltaDictionaryAlt[id], UserCommunityScoreDictionary[id]);
                titles.Add(title);
                iter++;
                Console.WriteLine($"{name} - OK. {iter}/{maxIter}");
                // Поскольку сайт очень болезненно относится к подобным запросам, после каждого запроса берётся случайная пауза
                Random rand = new();
                int delay = rand.Next(delayMin, delayMax);
                Thread.Sleep(delay);
            }

            Console.WriteLine($"Данные созданы. Ошибок: {errorIter}");
            string dataFile = allFiles[0][..^5] + "_database.json";
            StreamWriter sw = new(dataFile, false);
            sw.Write(JsonSerializer.Serialize(titles));
            sw.Close();
            Console.WriteLine($"Созданы данные по адресу: {dataFile}");
            Console.WriteLine("Нажмите любую клаивишу для закрытия окна");
            Console.ReadKey();
        }
    }
}
