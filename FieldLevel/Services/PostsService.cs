using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FieldLevel.Services
{
    public class PostsService
    {
        private readonly ILogger<PostsService> _logger;
        private readonly HttpClient _client;
        private IMemoryCache _cache;

        private const string DATA_SOURCE_URL = "https://jsonplaceholder.typicode.com/posts";
        private const string RESULTS_CACHE_KEY = "result_set";

        public PostsService(ILogger<PostsService> logger, IHttpClientFactory clientFactory, IMemoryCache cache)
        {
            _logger = logger;
            _client = clientFactory.CreateClient();
            _cache = cache;
        }

        public async Task<List<UserPost>> GetLatestPostPerUserAsync()
        {
            if (_cache.TryGetValue(RESULTS_CACHE_KEY, out List<UserPost> cachedUserPosts))
            {
                return cachedUserPosts;
            }
            var userPosts = await getUserPostsAsync();
            var latestUserPosts = getLatestUserPostForEachUser(userPosts);
            _cache.Set(RESULTS_CACHE_KEY, latestUserPosts, TimeSpan.FromMinutes(1));

            return latestUserPosts;
        }

        private async Task<List<UserPost>> getUserPostsAsync()
        {
            List<UserPost> userPosts;
            try
            {
                var response = await _client.GetAsync(DATA_SOURCE_URL);
                var json = await response.Content.ReadAsStringAsync();
                userPosts = JsonConvert.DeserializeObject<List<UserPost>>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetAsync failed when calling data provider.", ex);                
                throw new Exception("An error occurred while retrieving data from remote resource.");
            }
            return userPosts;
        }


        public List<UserPost> getLatestUserPostForEachUser(List<UserPost> userPosts)
        {
            var userIds = userPosts.Select(t => t.UserId).Distinct();
            var latestPosts = new List<UserPost>();
            foreach (var userId in userIds)
            {
                var latestPost = userPosts.Where(t => t.UserId == userId).OrderByDescending(t => t.Id).FirstOrDefault();
                latestPosts.Add(latestPost);
            }
            return latestPosts;
        }

        public List<UserPost> getLatestUserPostForEachUserDictionary(List<UserPost> userPosts)
        {
            var userPostDictionary = new Dictionary<int, UserPost>();
            foreach (var userPost in userPosts)
            {
                if (!userPostDictionary.ContainsKey(userPost.UserId))
                {
                    userPostDictionary.Add(userPost.UserId, userPost);
                    continue;
                }

                if (userPost.Id > userPostDictionary[userPost.UserId].Id)
                {
                    userPostDictionary[userPost.UserId] = userPost;
                }
            }
            return userPostDictionary.Values.ToList<UserPost>();     
        }
    }
}
