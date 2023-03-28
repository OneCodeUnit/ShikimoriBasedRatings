namespace ShikimoriBasedRatings
{
    internal class Program
    {
        static void Main()
        {
            // Поиск файла с данными пользователя в текущей директории. Он начинается и имени пользователя
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
            JsonParse.UserData(allFiles[0]);

            // Значения, сгруппированные: ид аниме - разница оценок (абсолютная)
            Dictionary<string, float> UserDeltaDictionary = new();
            // Значения, сгруппированные: ид аниме - разница оценок
            Dictionary<string, float> UserDeltaDictionaryAlt = new();

            int iter = 0;
            int maxIter = JsonParse.UserTitleDictionary.Count;
            foreach (var anime in JsonParse.UserTitleDictionary)
            {
                string id = anime.Key;
                string name = anime.Value;
                string page = ShikimoriHttpClient.GetShikimoriRating(id, name);
                // Если так и не получилось получить html
                if (page.Equals("error"))
                {
                    Console.WriteLine($"{name} - Error (get). {iter}/{maxIter}");
                    continue;
                }
                float onlineRating = JsonParse.ShikimoriRating(page);
                // Если так и не получилось найти оценку
                if (onlineRating < 1)
                {
                    Console.WriteLine($"{name} - Error (rating). {iter}/{maxIter}");
                    continue;
                }
                int youRating = JsonParse.UserScopeDictionary[id];
                float delta = Math.Abs(onlineRating - youRating);
                float deltaAlt = youRating - onlineRating;
                UserDeltaDictionary.Add(id, delta);
                UserDeltaDictionaryAlt.Add(id, deltaAlt);
                iter++;
                Console.WriteLine($"{name} - OK. {iter}/{maxIter}");
                // Поскольку сайт очень болезненно относится к подобным запросам, после каждого запроса берётся случайная пауза
                Random rand = new();
                int delay = rand.Next(1000, 2000);
                Thread.Sleep(delay);
            }

            // Сортировка по убыванию значений
            var sortedList = UserDeltaDictionary.ToList();
            sortedList.Sort((x, y) => x.Value.CompareTo(y.Value));
            string basedId = sortedList[0].Key;
            string unbasedId = sortedList[^1].Key;

            // Поиск среднего арифметического
            float sum = 0;
            foreach (var anime in UserDeltaDictionary)
            {
                sum += anime.Value;
            }
            float basedIndexAbs = sum / UserDeltaDictionary.Count;

            Console.WriteLine($"Больше всего ваше мнение совпадает с сообществом на этом аниме - {JsonParse.UserTitleDictionary[basedId]} (разница оценок - {UserDeltaDictionary[basedId]})");
            Console.WriteLine($"Больше всего вы не согласны с оценкой сообщества на этом аниме - {JsonParse.UserTitleDictionary[unbasedId]} (разница оценок - {UserDeltaDictionary[unbasedId]})");
            Console.WriteLine($"Индекс базированности (абсолютный) - {basedIndexAbs}");

            // Сортировка по убыванию значений
            var sortedListAlt = UserDeltaDictionaryAlt.ToList();
            sortedListAlt.Sort((x, y) => x.Value.CompareTo(y.Value));
            string mostHigh = sortedListAlt[0].Key;
            string mostLower = sortedListAlt[^1].Key;

            // Поиск среднего арифметического
            float sumAlt = 0;
            foreach (var anime in UserDeltaDictionaryAlt)
            {
                sumAlt += anime.Value;
            }
            float basedIndex = sumAlt / UserDeltaDictionaryAlt.Count;

            Console.WriteLine($"Аниме, которое больше всего не поняли вы - {JsonParse.UserTitleDictionary[mostHigh]} (разница оценок - {UserDeltaDictionaryAlt[mostHigh]})");
            Console.WriteLine($"Аниме, которое больше всего не поняло сообщество - {JsonParse.UserTitleDictionary[mostLower]} (разница оценок - {UserDeltaDictionaryAlt[mostLower]})");
            Console.WriteLine($"Индекс базированности (относительный) - {basedIndex}");

            // Запись результатов в файл. Некрасиво. Стоит реализовать иначе.
            StreamWriter resultFile = new("anime.txt", false);
            resultFile.WriteLine($"Больше всего ваше мнение совпадает с сообществом на этом аниме - {JsonParse.UserTitleDictionary[basedId]} (разница оценок - {Math.Abs(UserDeltaDictionary[basedId])})");
            resultFile.WriteLine($"Больше всего вы не согласны с оценкой сообщества на этом аниме - {JsonParse.UserTitleDictionary[unbasedId]} (разница оценок - {Math.Abs(UserDeltaDictionary[unbasedId])})");
            resultFile.WriteLine($"Индекс базированности (абсолютный) - {basedIndexAbs}");
            resultFile.WriteLine($"Аниме, которое больше всего не поняли вы - {JsonParse.UserTitleDictionary[mostHigh]} (разница оценок - {UserDeltaDictionaryAlt[mostHigh]})");
            resultFile.WriteLine($"Аниме, которое больше всего не поняло сообщество - {JsonParse.UserTitleDictionary[mostLower]} (разница оценок - {UserDeltaDictionaryAlt[mostLower]})");
            resultFile.WriteLine($"Индекс базированности (относительный) - {basedIndex}");
            resultFile.Close();
            Console.WriteLine($"Результаты записаны в файл {Directory.GetCurrentDirectory()}\\anime.txt");
            Console.ReadKey();
        }
    }
}