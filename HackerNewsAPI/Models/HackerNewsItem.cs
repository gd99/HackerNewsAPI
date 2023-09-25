namespace HackerNewsAPI.Models
{
    public class HackerNewsItem
    {
        public string By { get; init; }
        public int Descendants { get; init; }
        public int Id { get; init; }
        public List<int> Kids { get; init; }
        public int Score { get; init; }
        public int Time { get; init; }
        public string Title { get; init; }
        public string Type { get; init; }
        public string Url { get; init; }
        public bool Dead { get; init; }
    }
}
