using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services
{
    public interface IHackerNewsManager
    {
        Task<List<HackerNewsResult>> RetrieveStoryDataAsync(int storyCount, int version, bool liveCommentCount);
    }
}