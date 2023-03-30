namespace ShikimoriBasedRatings
{
    public class AnimeData
    {
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        public int Id { get; set; }
        public int Score { get; set; }
        public string Title { get; set; }
        public float DeltaAbs { get; set; }
        public float Delta { get; set; }
        public int[] CommunityScore { get; set; }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    }
}
