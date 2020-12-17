using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FieldLevel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
        private const string DATA_SOURCE_URL = "https://jsonplaceholder.typicode.com/posts";

        public HomeController(ILogger<HomeController> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {
            // TODO: Check cache for latest results from within the last minute.
           
            var response = await _client.GetAsync(DATA_SOURCE_URL);
            var userPosts = JsonConvert.DeserializeObject<List<UserPost>>(await response.Content.ReadAsStringAsync());
            var latestUserPosts = getLatestUserPosts(userPosts);
            
            // TODO: cache latest results.
            
            return new JsonResult(userPosts);
        }

        private List<UserPost> getLatestUserPosts(List<UserPost> userPosts)
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
