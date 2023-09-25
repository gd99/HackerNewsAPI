using HackerNewsAPI.Models;
using HackerNewsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HackerNewsAPI.Controllers
{
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly IHackerNewsManager _hackerNewsManager;
        private readonly ILogger _logger;

        public HackerNewsController(ILogger<HackerNewsController> logger, IHackerNewsManager hackerNewsManager)
        {
            _logger = logger;
            _hackerNewsManager = hackerNewsManager;
        }

        [Route("HackerNews/BestStories/v0")]
        [HttpGet]
        public async Task<List<HackerNewsResult>> GetBestStoryV0(int storyCount = 1, bool liveCommentCount = false)
        {
            _logger.LogInformation("HackerNews best stories call inititated for {0} stories, version 0, liveCommentCount {1}", storyCount, liveCommentCount);

            try
            {
                return await _hackerNewsManager.RetrieveStoryDataAsync(storyCount, 0, liveCommentCount);
            }
            catch (Exception ex)
            {
                _logger.LogError("Best stories call failed", ex.ToString());
                throw;
            }
        }
    }
}
