using ShikimoriDatabaseCreate.JsonClasses.User;
using System.Text.Json;

namespace ShikimoriDatabaseCreate
{
    public class ShikimoriHttpClient
    {
        private static readonly HttpClient Client = new();

        static ShikimoriHttpClient()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:111.0) Gecko/20100101 Firefox/111.0");
        }

        public static int CheckUser(string user)
        {
            string url = $"https://shikimori.me/api/users/{user}/?is_nickname=1";
            HttpResponseMessage response = Client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode) return -1;
            string data = response.Content.ReadAsStringAsync().Result;
            var json = JsonSerializer.Deserialize<ShikimoriUserJson>(data);
            if (json == null) return -1;
            return json.id;
        }

        public static string GetUserRatings(int id)
        {
            string url = $"https://shikimori.me/api/users/{id}/anime_rates?censored=false&status=completed&limit=2000";
            HttpResponseMessage response = Client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode) return $"Error {response.StatusCode} ({response.ReasonPhrase})";
            string data = response.Content.ReadAsStringAsync().Result;
            if (data is null) return $"Error 0 (Данные отсутствуют)";
            return data;
        }

        public static string GetCommunityRatings(int id)
        {
            string url = $"https://shikimori.me/api/animes/{id}";
            HttpResponseMessage response = Client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                Random rand = new();
                int delay = rand.Next(10000, 15000);
                Console.WriteLine($"Ошибка {response.StatusCode} ({response.ReasonPhrase}). Повторная попытка через {delay / 1000} сек. Проблема с адресом {url}");
                Thread.Sleep(delay);
                response = Client.GetAsync(url).Result;
                if (!response.IsSuccessStatusCode)
                    return $"Error {response.StatusCode} ({response.ReasonPhrase})";
            }
            string data = response.Content.ReadAsStringAsync().Result;
            if (data is null) return $"Error 0 (Данные отсутствуют)";
            return data;
        }
    }
}
