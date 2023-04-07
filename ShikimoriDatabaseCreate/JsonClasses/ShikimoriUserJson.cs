namespace ShikimoriDatabaseCreate.JsonClasses.User
{
#pragma warning disable IDE1006, CS8618
    public class Activity
    {
        public List<int> name { get; set; }
        public int value { get; set; }
    }

    public class Anime
    {
        public int id { get; set; }
        public string grouped_id { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public int value { get; set; }
    }

    public class FullStatuses
    {
        public List<Anime> anime { get; set; }
        public List<Manga> manga { get; set; }
    }

    public class Image
    {
        public string x160 { get; set; }
        public string x148 { get; set; }
        public string x80 { get; set; }
        public string x64 { get; set; }
        public string x48 { get; set; }
        public string x32 { get; set; }
        public string x16 { get; set; }
    }

    public class Manga
    {
        public int id { get; set; }
        public string grouped_id { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public int value { get; set; }
    }

    public class Ratings
    {
        public List<Anime> anime { get; set; }
    }

    public class ShikimoriUserJson
    {
        public int id { get; set; }
        public string nickname { get; set; }
        public string avatar { get; set; }
        public Image image { get; set; }
        public DateTime? last_online_at { get; set; }
        public string url { get; set; }
        public object name { get; set; }
        public string sex { get; set; }
        public int full_years { get; set; }
        public string last_online { get; set; }
        public string website { get; set; }
        public object location { get; set; }
        public bool banned { get; set; }
        public string about { get; set; }
        public string about_html { get; set; }
        public List<string> common_info { get; set; }
        public bool show_comments { get; set; }
        public object in_friends { get; set; }
        public bool is_ignored { get; set; }
        public Stats stats { get; set; }
        public int style_id { get; set; }
    }

    public class Scores
    {
        public List<Anime> anime { get; set; }
        public List<Manga> manga { get; set; }
    }

    public class Stats
    {
        public Statuses statuses { get; set; }
        public FullStatuses full_statuses { get; set; }
        public Scores scores { get; set; }
        public Types types { get; set; }
        public Ratings ratings { get; set; }
        public bool has_anime { get; set; }
        public bool has_manga { get; set; }
        public List<object> genres { get; set; }
        public List<object> studios { get; set; }
        public List<object> publishers { get; set; }
        public List<Activity> activity { get; set; }
    }

    public class Statuses
    {
        public List<Anime> anime { get; set; }
        public List<Manga> manga { get; set; }
    }

    public class Types
    {
        public List<Anime> anime { get; set; }
        public List<Manga> manga { get; set; }
    }
#pragma warning restore IDE1006, CS8618
}
