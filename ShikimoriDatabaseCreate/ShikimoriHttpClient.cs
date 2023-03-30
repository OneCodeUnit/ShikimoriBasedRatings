namespace ShikimoriDatabaseCreate
{
    public class ShikimoriHttpClient
    {
        private static readonly HttpClient Client = new();

        static ShikimoriHttpClient()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:111.0) Gecko/20100101 Firefox/111.0");
        }

        public static string GetShikimoriRating(int animeId, string animeTitle, byte maxRetry)
        {
            // Попытка получить html-файл страницы аниме
            string url = $"https://shikimori.one/animes/{animeId}-{animeTitle}";
            //string url = $"https://shikimori.one/animes/34134-one-punch-man-2nd-season";
            HttpResponseMessage response = Client.GetAsync(url).Result;
            // У некоторых выдаётся ошибка 404 и стоит перенаправление на страницу с другим ид (который имеет букву в начале, но я не уверен, что так всегда)
            string page;
            if (response.StatusCode.ToString().Equals("NotFound"))
            {
                page = response.Content.ReadAsStringAsync().Result;
                int startIndex = page.IndexOf("доступна") + 21;
                int endIndex = page.IndexOf("новой") - 2;
                string newUrl;
                try
                {
                    newUrl = page[startIndex..endIndex];
                }
                catch
                {
                    // Если это реальная 404
                    newUrl = url;
                }
                response = Client.GetAsync(newUrl).Result;
            }
            // Повторные попытки, если запрос не удался
            byte retry = 0;
            while (!response.IsSuccessStatusCode)
            {
                if (retry == maxRetry)
                {
                    return "error";
                }
                Random rand = new();
                int delay = rand.Next(10000, 15000);
                Console.WriteLine($"Ошибка {response.StatusCode}. Повторная попытка через {delay / 1000} сек. Проблема с адресом {url}");
                Thread.Sleep(delay);
                response = Client.GetAsync(url).Result;
                retry++;
            }
            page = response.Content.ReadAsStringAsync().Result;
            return page;
        }
    }
}
