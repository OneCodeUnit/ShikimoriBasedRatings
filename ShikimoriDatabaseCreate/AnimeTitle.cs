namespace ShikimoriDatabaseCreate
{
    public class AnimeTitle
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string Title { get; set; }
        public float DeltaAbs { get; set; }
        public float Delta { get; set; }
        public int[] CommunityScore { get; set; }

        public AnimeTitle()
        {
            Id = 0;
            Score = 0;
            Title = string.Empty;
            DeltaAbs = 0;
            Delta = 0;
            CommunityScore = new int[11];
        }

        public AnimeTitle(int id, int score, string title, float deltaAbs, float delta, int[] communityScore)
        {
            Id = id;
            Score = score;
            Title = title;
            DeltaAbs = deltaAbs;
            Delta = delta;
            CommunityScore = communityScore;
        }
    }
}
