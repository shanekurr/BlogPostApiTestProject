using Microsoft.VisualStudio.TestTools.UnitTesting;
using FieldLevel.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FieldLevel.Controllers.Tests
{
    [TestClass()]
    public class HomeControllerTests
    {
        [TestMethod()]
        public async Task GetTest()
        {
            var testController = new HomeController(new NullLogger<HomeController>(), new System.Net.Http.HttpClient());
            var result = await testController.GetAsync();
            Assert.IsNotNull(result);
            var userPosts = result.Value as List<UserPost>;
            Assert.IsInstanceOfType(userPosts[0], typeof(UserPost));
        }
    }
}