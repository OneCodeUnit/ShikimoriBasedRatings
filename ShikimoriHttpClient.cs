namespace ShikimoriBasedRatings
{
    public class ShikimoriHttpClient
    {
        private static readonly HttpClient Client = new();

        static ShikimoriHttpClient()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:111.0) Gecko/20100101 Firefox/111.0");
        }

        public static string GetShikimoriRating(string animeId, string animeTitle)
        {
            // Найденные эмпирически правила преобразования названия аниме в адресную строку. Сюда бы регулярку...
            animeTitle = animeTitle.ToLower();
            animeTitle = animeTitle.Replace(" - ", "-");
            animeTitle = animeTitle.Replace(": ", "-");
            animeTitle = animeTitle.Replace(' ', '-');
            animeTitle = animeTitle.Replace('.', '-');
            animeTitle = animeTitle.Replace('+', '-');
            animeTitle = animeTitle.Replace('/', '-');
            animeTitle = animeTitle.Replace('—', '-');
            animeTitle = animeTitle.Replace('\'', '-');
            animeTitle = animeTitle.Replace(" & ", "-");
            animeTitle = animeTitle.Replace(",", string.Empty);
            animeTitle = animeTitle.Replace("!", string.Empty);
            animeTitle = animeTitle.Replace("?", string.Empty);
            animeTitle = animeTitle.Replace("(", string.Empty);
            animeTitle = animeTitle.Replace(")", string.Empty);
            animeTitle = animeTitle.Replace("[", string.Empty);
            animeTitle = animeTitle.Replace("]", string.Empty);
            animeTitle = animeTitle.Replace("---", "-");
            animeTitle = animeTitle.Replace("--", "-");

            // Попытка получить html-файл страницы аниме
            HttpResponseMessage response;
            string url = $"https://shikimori.one/animes/{animeId}-{animeTitle}";
            response = Client.GetAsync(url).Result;
            // У некоторых выдаётся ошибка 404 и стоит перенаправление на страницу с другим ид (который имеет букву в начале, но я не уверен, что так всегда)
            if (response.StatusCode.ToString().Equals("NotFound"))
            {
                string newPage = response.Content.ReadAsStringAsync().Result;
                int startIndex = newPage.IndexOf("<a href=\"");
                int endIndex = newPage.IndexOf("\">новой ссылке</a>");
                string newUrl;
                try
                {
                    newUrl = newPage.Substring(startIndex + 9, endIndex - startIndex - 9);
                }
                catch
                {
                    // Если это реальная 404
                    newUrl = url;
                }
                response = Client.GetAsync(newUrl).Result;
            }
            // Повторные попытки, если запрос не удался
            int retry = 0;
            int maxRetry = 1; 
            while (!response.IsSuccessStatusCode)
            {
                if (retry == maxRetry)
                {
                    return "error";
                }
                else
                {
                    Random rand = new();
                    int delay = rand.Next(10000, 20000);
                    Console.WriteLine($"Ошибка {response.StatusCode}. Повторная попытка через {delay / 1000} сек. Проблема с адресом {url}");
                    Thread.Sleep(delay);
                    response = Client.GetAsync(url).Result;
                    retry++;
                }
            }
            string page = response.Content.ReadAsStringAsync().Result;
            return page;
        }
    }
}
