using System.Globalization;
using System.Text.Json;

namespace ShikimoriDatabaseCreate
{
    internal class JsonParse
    {
        // Значения, сгруппированные: ид аниме - твоя оценка
        public static readonly Dictionary<int, int> UserScopeDictionary = new();
        // Значения, сгруппированные: ид аниме - его название
        public static readonly Dictionary<int, string> UserTitleDictionary = new();
        // Значения, сгруппированные: ид аниме - его название для адреса страницы
        public static readonly Dictionary<int, string> UserTitleUrlDictionary = new();

        // Вычленение из файла только нужных данных - ид аниме, твоей оценки, названия
        public static void CreateUserData(string path)
        {
            string json = File.ReadAllText(path);
            //var userData = JsonConvert.DeserializeObject<List<UserRaitingsJson>>(json);
            var userData = JsonSerializer.Deserialize<List<UserRaitingsJson>>(json);
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
            foreach (var anime in userData)
            {
                // У аниме без оценок стоит оценка 0, которую нельзя поставить вручную
                if (anime.score > 0)
                {
                    UserScopeDictionary.Add(anime.target_id, anime.score);
                    UserTitleDictionary.Add(anime.target_id, anime.target_title);
                }
            }
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
            CreateTitleUrl();
        }

        private static void CreateTitleUrl()
        {
            foreach (var anime in UserTitleDictionary)
            {
                string title = anime.Value;
                // Найденные эмпирически правила преобразования названия аниме в адресную строку.
                title = title.ToLower();
                title = title.Replace(" - ", "-");
                title = title.Replace(": ", "-");
                title = title.Replace(' ', '-');
                title = title.Replace('.', '-');
                title = title.Replace('+', '-');
                title = title.Replace('/', '-');
                title = title.Replace('—', '-');
                title = title.Replace('\'', '-');
                title = title.Replace(" & ", "-");
                title = title.Replace(",", string.Empty);
                title = title.Replace("!", string.Empty);
                title = title.Replace("?", string.Empty);
                title = title.Replace("(", string.Empty);
                title = title.Replace(")", string.Empty);
                title = title.Replace("[", string.Empty);
                title = title.Replace("]", string.Empty);
                title = title.Replace("---", "-");
                title = title.Replace("--", "-");

                UserTitleUrlDictionary.Add(anime.Key, title);
            }
        }

        public static float ShikimoriRating(string json)
        {
            // Вычленение из http-файла оценки. Обойтись без нормального парсера - действительно хорошая идея?
            int index = json.IndexOf("ratingValue") - 12;
            float result;
            // Оценка указывается с точностью до двух знаков после запятой. Если на конце 0, то размер оценки уменьшается
            try
            {
                string raiting = json.Substring(index - 4, 4);
                result = Convert.ToSingle(raiting, CultureInfo.InvariantCulture);
            }
            catch
            {
                try
                {
                    string raiting = json.Substring(index - 3, 3);
                    result = Convert.ToSingle(raiting, CultureInfo.InvariantCulture);
                }
                catch
                {
                    try
                    {
                        string raiting = json.Substring(index - 1, 1);
                        result = Convert.ToSingle(raiting, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return -1;
                    }
                }
            }
            return result;
        }

        public static int[] ShikimoriCommunityRating(string json)
        {
            int[] communityRating = new int[11];
            // Вычленение из http-файла оценок пользователей
            int indexStart = json.IndexOf("data-stats") + 13;
            int indexEnd = json.IndexOf("rates_scores_stats") - 7;
            string raiting = string.Empty;
            try
            {
                raiting = json[indexStart..indexEnd];
            }
            catch
            {
                communityRating[0] = -1;
            }
            // Удаление лишних символов
            raiting = raiting.Replace("&quot;", string.Empty);
            raiting = raiting.Replace("[", string.Empty);
            raiting = raiting.Replace("]", string.Empty);
            string[] raitings = raiting.Split(',');
            // Для удобства индекс массива используется в качестве номера оценки, значение яцейки - количество этих оценок
            for (int i = 0; i < raitings.Length; i++)
            {
                communityRating[Convert.ToInt32(raitings[i])] = Convert.ToInt32(raitings[i + 1]);
                i++;
            }
            return communityRating;
        }
    }
}
