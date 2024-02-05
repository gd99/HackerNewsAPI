namespace HackerNewsAPI.Models
{
    public class HackerNewsResult
    {
        public HackerNewsResult(string title, string uri, string postedBy, string time, int score, int commentCount)
        {
            this.title = title;
            this.uri = uri;
            this.postedBy = postedBy;
            this.time = time;
            this.score = score;
            this.commentCount = commentCount;
        }

        public string title { get; init; }
        public string uri { get; init; }
        public string postedBy { get; init; }
        public string time { get; init; }
        public int score { get; init; }
        public int commentCount { get; init; }


        public static HackerNewsResult FromHackerNewsItem(HackerNewsItem response)
        {
            return new HackerNewsResult(response.Title, response.Url, response.By, DateTimeOffset.FromUnixTimeSeconds(response.Time).DateTime.ToString("yyyy-MM-ddTHH:mm:sszzz"), response.Score, response.Descendants);
        }

        public static HackerNewsResult FromHackerNewsItem(HackerNewsItem response, int liveCommentCount)
        {
            return new HackerNewsResult(response.Title, response.Url, response.By, DateTimeOffset.FromUnixTimeSeconds(response.Time).DateTime.ToString("yyyy-MM-ddTHH:mm:sszzz"), response.Score, liveCommentCount);
        }
    } 
}
