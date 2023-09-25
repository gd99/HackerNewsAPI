using AutoFixture;
using HackerNewsAPI.Models;
using HackerNewsAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace HackerNewsApiTests.Services
{
    public class HackerNewsManagerTests
    {
        private readonly Mock<ILogger<HackerNewsManager>> _loggerMock = new Mock<ILogger<HackerNewsManager>>();
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        private HackerNewsManager testSubject;

        private HackerNewsItem item1, item2, item3, comment1, comment2;

        List<int> kidIds = new List<int>() { 1, 2, 3 };

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();

            item1 = new HackerNewsItem() { Url = "url", Kids = new List<int>() { 5, 10 } };
            item2 = fixture.Create<HackerNewsItem>();
            item3 = fixture.Create<HackerNewsItem>();
            comment1 = new HackerNewsItem() { Type = "comment", Kids = null };
            comment2 = new HackerNewsItem() { Type = "comment", Kids = null };

            var appSettings = @"{ 
    ""ConnectionStrings"": {
    ""FirebaseBestStories"": ""https://hacker-news.firebaseio.com/v{0}/beststories.json"",
    ""FirebaseItems"": ""https://hacker-news.firebaseio.com/v{0}/item/{1}.json""
    },
  ""HttpClient"": { ""MaxSocketPoolSize"": 10 },
  ""FirebaseManager"": { ""MaxParallelism"": 10 }
            }";

            var builder = new ConfigurationBuilder();

            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            var configuration = builder.Build();

            MockHttpMessageHandler _handlerMock = new MockHttpMessageHandler();

            _httpClientFactoryMock.Setup(x => x.CreateClient("HackerNews"))
                    .Returns(new HttpClient(_handlerMock));

            _handlerMock.When($"https://hacker-news.firebaseio.com/v0/beststories.json")
                .Respond(HttpStatusCode.OK, JsonContent.Create(kidIds));

            _handlerMock.When($"https://hacker-news.firebaseio.com/v0/item/1.json")
                .Respond(HttpStatusCode.OK, JsonContent.Create(item1));

            _handlerMock.When($"https://hacker-news.firebaseio.com/v0/item/2.json")
                .Respond(HttpStatusCode.OK, JsonContent.Create(item2));

            _handlerMock.When($"https://hacker-news.firebaseio.com/v0/item/3.json")
                .Respond(HttpStatusCode.OK, JsonContent.Create(item3));

            _handlerMock.When($"https://hacker-news.firebaseio.com/v0/item/5.json")
                .Respond(HttpStatusCode.OK, JsonContent.Create(comment1));

            _handlerMock.When($"https://hacker-news.firebaseio.com/v0/item/10.json")
                .Respond(HttpStatusCode.OK, JsonContent.Create(comment2));

            testSubject = new HackerNewsManager(_loggerMock.Object, _httpClientFactoryMock.Object, configuration);
        }

        [Test]
        public async Task gives_expected_results()
        {
            var result = await testSubject.RetrieveStoryDataAsync(3, 0, false);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(item1.Url, result[0].uri);
            Assert.AreEqual(item2.Url, result[1].uri);
            Assert.AreEqual(item3.Url, result[2].uri);

        }

        [Test]
        public async Task limits_story_count_to_2()
        {
            var result = await testSubject.RetrieveStoryDataAsync(2, 0, false);

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task limits_story_count_to_1()
        {
            var result = await testSubject.RetrieveStoryDataAsync(1, 0, false);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task calls_live_comment_counter_correctly()
        {
            var result = await testSubject.RetrieveStoryDataAsync(1, 0, true);

            Assert.AreEqual(2, result[0].commentCount);
        }
    }
}
