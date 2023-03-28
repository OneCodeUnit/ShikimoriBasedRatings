using Newtonsoft.Json;
using System.Globalization;

namespace ShikimoriBasedRatings
{
    public class JsonParse
    {
        // Значения, сгруппированные: ид аниме - твоя оценка
        public static readonly Dictionary<string, int> UserScopeDictionary = new();
        // Значения, сгруппированные: ид аниме - его название
        public static readonly Dictionary<string, string> UserTitleDictionary = new();

        // Вычленение из файла только нужных данных - ид аниме, твоей оценки, названия
        public static void UserData(string path)
        {
            string json = File.ReadAllText(path);
            List<UserRaitingsJson>? userData = JsonConvert.DeserializeObject<List<UserRaitingsJson>>(json);
            foreach (var anime in userData)
            {
                // У аниме без оценок стоит оценка 0, которую нельзя поставить вручную
                if (anime.score > 0)
                {
                    UserScopeDictionary.Add(anime.target_id, anime.score);
                    UserTitleDictionary.Add(anime.target_id, anime.target_title);
                }
            }
        }

        public static float ShikimoriRating(string json)
        {
            // Вычленение из http - файла оценки. Обойтись без нормального парсера - действительно хорошая идея?
            int index = json.IndexOf("\" itemprop=\"ratingValue\"");
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
    }
}
