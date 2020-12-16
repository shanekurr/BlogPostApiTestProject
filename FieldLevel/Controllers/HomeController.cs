using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public async Task<HttpResponseMessage> Get()
        {
            var result = await _client.GetAsync(DATA_SOURCE_URL);
            return result;
        }
    }
}
