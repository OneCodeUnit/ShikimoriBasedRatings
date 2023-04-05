using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

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
            string pattern1 = @"[,!?\(\)\[\]\p{M}]";
            string pattern2 = @" - |: |;| |\.|\+|/|—|\\| & |---|--";
            Regex regex1 = new (pattern1);
            Regex regex2 = new (pattern2);
            foreach (var anime in UserTitleDictionary)
            {
                string title = anime.Value.ToLower();
                // Найденные эмпирически правила преобразования названия аниме в адресную строку.
                title = regex1.Replace(title, string.Empty);
                title = regex2.Replace(title, "-");

                UserTitleUrlDictionary.Add(anime.Key, title);
            }
        }

        public static float ShikimoriRating(string json)
        {
            // Вычленение из http-файла оценки.
            string pattern = @"<.*meta content=(.*)itemprop=.*ratingValue.*/>";
            Match m = Regex.Match(json, pattern);
            string mark = m.Groups[1].Value[1..^2];
            float result = Convert.ToSingle(mark, CultureInfo.InvariantCulture);
            return result;
        }

        public static int[] ShikimoriCommunityRating(string json)
        {
            int[] communityRating = new int[11];
            // Вычленение из http-файла оценок пользователей
            string pattern = @".*data-stats=(.*)\s*id=.rates_scores_stats";
            Match m = Regex.Match(json, pattern);
            string raiting = m.Groups[1].Value[2..^2];
            // Удаление лишних символов
            pattern = @"&quot;|\[|\]";
            raiting = Regex.Replace(raiting, pattern, string.Empty);

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
