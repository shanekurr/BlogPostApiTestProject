using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FieldLevel.Services;
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
        private PostsService _postsService;

        // Should move this url to appsettings when in production
        private const string DATA_SOURCE_URL = "https://jsonplaceholder.typicode.com/posts";
        private const string RESULTS_CACHE_KEY = "result_set";

        public PostsController(PostsService postsService)
        {
            
            _postsService = postsService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            
            List<UserPost> userPosts;
            try
            {
                userPosts = await _postsService.GetLatestPostPerUserAsync();
            } catch(Exception ex)
            {
                _logger.LogError("GetAsync failed when calling data provider.", ex);
                return StatusCode(500,"An error occurred while processing your request.");
            }

            
            return new JsonResult(userPosts);
        }

    }
}
