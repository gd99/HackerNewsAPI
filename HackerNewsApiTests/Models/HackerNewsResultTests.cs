using HackerNewsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsApiTests.Models
{
    public class HackerNewsResultTests
    {
        [Test]
        public void converts_from_hacker_news_item_to_result()
        {
            var item = new HackerNewsItem() { Title = "title", Url = "url", Time = 100, Score = 200, Descendants = 50 };

            var result = HackerNewsResult.FromHackerNewsItem(item);

            Assert.AreEqual(item.Title, result.title);
            Assert.AreEqual(item.Url, result.uri);
            Assert.AreEqual(DateTimeOffset.FromUnixTimeSeconds(item.Time).DateTime.ToString("yyyy-MM-ddTHH:mm:sszzz"), result.time);
            Assert.AreEqual(item.Score, result.score);
            Assert.AreEqual(item.Descendants, result.commentCount);
        }

        [Test]
        public void converts_from_hacker_news_item_to_result_with_comment_count()
        {
            var item = new HackerNewsItem() { Title = "title", Url = "url", Time = 100, Score = 200, Descendants = 50 };

            var result = HackerNewsResult.FromHackerNewsItem(item, 88);

            Assert.AreEqual(item.Title, result.title);
            Assert.AreEqual(item.Url, result.uri);
            Assert.AreEqual(DateTimeOffset.FromUnixTimeSeconds(item.Time).DateTime.ToString("yyyy-MM-ddTHH:mm:sszzz"), result.time);
            Assert.AreEqual(item.Score, result.score);
            Assert.AreEqual(88, result.commentCount);
        }
    }
}
