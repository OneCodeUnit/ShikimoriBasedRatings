namespace ShikimoriDatabaseCreate.JsonClasses.List
{
#pragma warning disable IDE1006, CS8618
    public class Anime
    {
        public int id { get; set; }
        public string name { get; set; }
        public string russian { get; set; }
        public Image image { get; set; }
        public string url { get; set; }
        public string kind { get; set; }
        public string score { get; set; }
        public string status { get; set; }
        public int episodes { get; set; }
        public int episodes_aired { get; set; }
        public string aired_on { get; set; }
        public string released_on { get; set; }
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
        public string original { get; set; }
        public string preview { get; set; }
        public string x96 { get; set; }
    }

    public class ShikimoriListJson
    {
        public int id { get; set; }
        public int score { get; set; }
        public string status { get; set; }
        public string text { get; set; }
        public int episodes { get; set; }
        public int? chapters { get; set; }
        public int? volumes { get; set; }
        public string text_html { get; set; }
        public int rewatches { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public User user { get; set; }
        public Anime anime { get; set; }
        public object manga { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string nickname { get; set; }
        public string avatar { get; set; }
        public Image image { get; set; }
        public DateTime? last_online_at { get; set; }
        public string url { get; set; }
    }
#pragma warning restore IDE1006, CS8618
}
