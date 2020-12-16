using Microsoft.VisualStudio.TestTools.UnitTesting;
using FieldLevel.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;

namespace FieldLevel.Controllers.Tests
{
    [TestClass()]
    public class HomeControllerTests
    {
        [TestMethod()]
        public async Task GetTest()
        {

            var testController = new HomeController(new NullLogger<HomeController>(), new System.Net.Http.HttpClient());
            var response = await testController.Get();
            var result = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(result);
        }
    }
}