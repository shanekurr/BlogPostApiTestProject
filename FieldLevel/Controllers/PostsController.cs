using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FieldLevel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        
        private readonly ILogger<PostsController> _logger;
        private readonly HttpClient _client;
        private IMemoryCache _cache;

        // Should move this url to appsettings when in production
        private const string DATA_SOURCE_URL = "https://jsonplaceholder.typicode.com/posts";
        private const string RESULTS_CACHE_KEY = "result_set";

        public PostsController(ILogger<PostsController> logger, IHttpClientFactory clientFactory, IMemoryCache cache)
        {
            _logger = logger;
            _client = clientFactory.CreateClient();
            _cache = cache;            
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            if (_cache.TryGetValue(RESULTS_CACHE_KEY, out List<UserPost> cachedUserPosts))
            {
                return new JsonResult(cachedUserPosts);
            }

            List<UserPost> userPosts;
            try
            {
                var response = await _client.GetAsync(DATA_SOURCE_URL);
                userPosts = JsonConvert.DeserializeObject<List<UserPost>>(await response.Content.ReadAsStringAsync());
                
            } catch(Exception ex)
            {
                _logger.LogError("GetAsync failed when calling data provider.", ex);
                return StatusCode(500,"An error occurred while processing your request.");
            }

            var latestUserPosts = getLatestUserPostForEachUser(userPosts);
            _cache.Set(RESULTS_CACHE_KEY, latestUserPosts, TimeSpan.FromMinutes(1));

            return new JsonResult(latestUserPosts);
        }

        private List<UserPost> getLatestUserPostForEachUser(List<UserPost> userPosts)
        {
            var userIds = userPosts.Select(t => t.UserId).Distinct();
            var latestPosts = new List<UserPost>();
            foreach(var userId in userIds)
            {
                var latestPost = userPosts.Where(t => t.UserId == userId).OrderByDescending(t => t.Id).FirstOrDefault();
                latestPosts.Add(latestPost);
            }
            return latestPosts;
        }
    }
}
