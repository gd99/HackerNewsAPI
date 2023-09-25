using HackerNewsAPI.Controllers;
using HackerNewsAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNewsApiTests.Controllers
{
    public class HackerNewsControllerTests
    {
        private readonly Mock<IHackerNewsManager> _hackerNewsManagerMock = new Mock<IHackerNewsManager>();
        private readonly Mock<ILogger<HackerNewsController>> _loggerMock = new Mock<ILogger<HackerNewsController>>();

        [Test]
        public async Task run()
        {
            var subject = new HackerNewsController(_loggerMock.Object, _hackerNewsManagerMock.Object);

            await subject.GetBestStoryV0(5, false);

            _hackerNewsManagerMock.Verify(mock => mock.RetrieveStoryDataAsync(5, 0, false));
        }

        [Test]
        public async Task run_different()
        {
            var subject = new HackerNewsController(_loggerMock.Object, _hackerNewsManagerMock.Object);

            await subject.GetBestStoryV0(15, true);

            _hackerNewsManagerMock.Verify(mock => mock.RetrieveStoryDataAsync(15, 0, true));
        }
    }
}
