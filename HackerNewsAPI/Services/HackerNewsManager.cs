using HackerNewsAPI.Models;
using System.Collections.Concurrent;

namespace HackerNewsAPI.Services
{
    public class HackerNewsManager : IHackerNewsManager
    {
        private const string HttpClientName = "HackerNews";
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private string _hackerNewsBestStoriesUrl;
        private string _hackerNewsItemsUrl;
        private ParallelOptions _parallelOptions;
        

        public HackerNewsManager(ILogger<HackerNewsManager> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _hackerNewsBestStoriesUrl = configuration.GetConnectionString("HackerNewsBestStories")!;
            _hackerNewsItemsUrl = configuration.GetConnectionString("HackerNewsItems")!;
            _parallelOptions = new() { MaxDegreeOfParallelism = configuration.GetValue<int>("HackerNewsManager:MaxParallelism") };
        }

        public async Task<List<HackerNewsResult>> RetrieveStoryDataAsync(int storyCount, int version, bool liveCommentCount)
        {
            var http = _httpClientFactory.CreateClient(HttpClientName);
            var bestStoriesURL = string.Format(_hackerNewsBestStoriesUrl, version, storyCount);

            var ids = (await http.GetFromJsonAsync<List<int>>(bestStoriesURL)).Take(storyCount);

            var resultDict = new ConcurrentDictionary<int, HackerNewsResult>();
                        
            _logger.LogInformation("Retrieved best story ids from url {1}", bestStoriesURL);

            await Parallel.ForEachAsync(ids, _parallelOptions, async (id, token) =>
            {
                var itemUrl = string.Format(_hackerNewsItemsUrl, version, id);

                var item = await http.GetFromJsonAsync<HackerNewsItem>(itemUrl);

                if (liveCommentCount)
                {
                    _logger.LogInformation("Getting live comment count");
                    var calculatedCommentCount = await GetLiveCommentCountAsync(item, version, http);
                    resultDict[id] = HackerNewsResult.FromHackerNewsItem(item, calculatedCommentCount);
                }
                else
                {
                    resultDict[id] = HackerNewsResult.FromHackerNewsItem(item);
                }
            });

            _logger.LogInformation("Successfully retrieved {0} results", resultDict.Count);

            return resultDict.Values.ToList();
        }

        private async Task<int> GetLiveCommentCountAsync(HackerNewsItem responseParent, int version, HttpClient http)
        {
            int commentCount = 0;
            
            if (responseParent.Type == "comment" && !responseParent.Dead)
            {
                commentCount++;
            }

            if (responseParent.Kids != null && responseParent.Kids.Count > 0)
            {
                await Parallel.ForEachAsync(responseParent.Kids, _parallelOptions, async (kidId, token) =>
                {
                    var itemUrl = string.Format(_hackerNewsItemsUrl, version, kidId);

                    var response = await http.GetFromJsonAsync<HackerNewsItem>(itemUrl, token);

                    int belowCount = await GetLiveCommentCountAsync(response, version, http);

                    Interlocked.Add(ref commentCount, belowCount);
                });
            }

            return commentCount;
        }
    }
}
