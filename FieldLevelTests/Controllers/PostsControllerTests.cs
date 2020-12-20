using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using System.Net.Http;
using Moq;
using Moq.Protected;
using System.Threading;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;

namespace FieldLevel.Controllers.Tests
{
    [TestClass()]
    public class PostsControllerTests
    {
        private const string SUCCESS_CONTENT = "[{\"body\":\"quo et expedita modi cum officia vel magni\ndoloribus qui repudiandae\nvero nisi sit\nquos veniam quod sed accusamus veritatis error\",\"title\":\"optio molestias id quia eum\",\"id\":10,\"userId\":1},{\"body\":\"qui consequuntur ducimus possimus quisquam amet similique\nsuscipit porro ipsam amet\neos veritatis officiis exercitationem vel fugit aut necessitatibus totam\nomnis rerum consequatur expedita quidem cumque explicabo\",\"title\":\"doloribus ad provident suscipit at\",\"id\":20,\"userId\":2},{\"body\":\"alias dolor cumque\nimpedit blanditiis non eveniet odio maxime\nblanditiis amet eius quis tempora quia autem rem\na provident perspiciatis quia\",\"title\":\"a quo magni similique perferendis\",\"id\":30,\"userId\":3},{\"body\":\"ut voluptatum aliquid illo tenetur nemo sequi quo facilis\nipsum rem optio mollitia quas\nvoluptatem eum voluptas qui\nunde omnis voluptatem iure quasi maxime voluptas nam\",\"title\":\"enim quo cumque\",\"id\":40,\"userId\":4},{\"body\":\"error suscipit maxime adipisci consequuntur recusandae\nvoluptas eligendi et est et voluptates\nquia distinctio ab amet quaerat molestiae et vitae\nadipisci impedit sequi nesciunt quis consectetur\",\"title\":\"repellendus qui recusandae incidunt voluptates tenetur qui omnis exercitationem\",\"id\":50,\"userId\":5},{\"body\":\"asperiores sunt ab assumenda cumque modi velit\nqui esse omnis\nvoluptate et fuga perferendis voluptas\nillo ratione amet aut et omnis\",\"title\":\"consequatur placeat omnis quisquam quia reprehenderit fugit veritatis facere\",\"id\":60,\"userId\":6},{\"body\":\"sunt repellendus quae\nest asperiores aut deleniti esse accusamus repellendus quia aut\nquia dolorem unde\neum tempora esse dolore\",\"title\":\"voluptatem laborum magni\",\"id\":70,\"userId\":7},{\"body\":\"ex quod dolorem ea eum iure qui provident amet\nquia qui facere excepturi et repudiandae\nasperiores molestias provident\nminus incidunt vero fugit rerum sint sunt excepturi provident\",\"title\":\"labore in ex et explicabo corporis aut quas\",\"id\":80,\"userId\":8},{\"body\":\"minus omnis soluta quia\nqui sed adipisci voluptates illum ipsam voluptatem\neligendi officia ut in\neos soluta similique molestias praesentium blanditiis\",\"title\":\"ad iusto omnis odit dolor voluptatibus\",\"id\":90,\"userId\":9},{\"body\":\"cupiditate quo est a modi nesciunt soluta\nipsa voluptas error itaque dicta in\nautem qui minus magnam et distinctio eum\naccusamus ratione error aut\",\"title\":\"at nam consequatur ea labore ea harum\",\"id\":100,\"userId\":10}]";

        [TestMethod()]
        public async Task GetTest_Success()
        {
            // Found how to mock IHttpClientFactory at https://www.thecodebuzz.com/mock-typed-httpclient-httpclientfactory-moq-net-core/
            var mockFactory = new Mock<IHttpClientFactory>();
            var handler = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(); 
            mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage 
                { 
                    StatusCode = HttpStatusCode.OK, Content = new StringContent(SUCCESS_CONTENT), }
                ); 
            var client = new HttpClient(mockHttpMessageHandler.Object); 
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // mock memory cache
            var memoryCache    = Mock.Of<IMemoryCache>();
            var cacheEntry = Mock.Of<ICacheEntry>();
            var mockMemoryCache = Mock.Get(memoryCache);
            mockMemoryCache
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntry);

            var testController = new PostsController(new NullLogger<PostsController>(), mockFactory.Object, mockMemoryCache.Object);
            var result = await testController.GetAsync() as JsonResult;
            Assert.IsNotNull(result);
            var userPosts = result.Value as List<UserPost>;
            Assert.IsInstanceOfType(userPosts[0], typeof(UserPost));
        }
    }
}