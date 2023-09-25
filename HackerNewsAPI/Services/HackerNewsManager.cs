using HackerNewsAPI.Models;
using System.Net.Http;

namespace HackerNewsAPI.Services
{
    public class HackerNewsManager : IHackerNewsManager
    {
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
            var http = _httpClientFactory.CreateClient("HackerNews");
            var bestStoriesURL = string.Format(_hackerNewsBestStoriesUrl, version, storyCount);

            var ids = (await http.GetFromJsonAsync<List<int>>(bestStoriesURL)).Take(storyCount).ToList();

            var resultDict = new Dictionary<int, HackerNewsResult>();
            ids.ForEach(id => resultDict.Add(id, null));

            _logger.LogInformation("Retrieved {0} best story ids from url {1}", ids.Count, bestStoriesURL);

            await Parallel.ForEachAsync(ids, _parallelOptions, async (id, token) =>
            {
                var itemUrl = string.Format(_hackerNewsItemsUrl, version, id);

                var item = await http.GetFromJsonAsync<HackerNewsItem>(itemUrl);

                if (liveCommentCount)
                {
                    _logger.LogInformation("Getting live comment count");
                    var calculatedCommentCount = await GetLiveCommentCount(item, version, http);
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

        private async Task<int> GetLiveCommentCount(HackerNewsItem responseParent, int version, HttpClient http)
        {
            int commentCount = 0;
            object objLock = new object();

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

                    int belowCount = await GetLiveCommentCount(response, version, http);

                    lock (objLock)
                        commentCount += belowCount;
                });
            }

            return commentCount;
        }
    }
}
