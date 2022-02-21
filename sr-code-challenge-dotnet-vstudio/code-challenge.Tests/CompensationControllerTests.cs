using challenge.Controllers;
using challenge.Data;
using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void Add_And_Get_Lennon_Compensation()
        {
            string empId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            float salary = 160000;
            DateTime effectiveDate = Convert.ToDateTime("2022-02-21T17:21:42+0000");

            // Arrange
            var compensation = new Compensation()
            {
                EmployeeId = empId,
                Salary =    salary,
                EffectiveDate = effectiveDate
                
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.EmployeeId);
            Assert.AreEqual(newCompensation.Salary, salary);
            Assert.AreEqual(newCompensation.EffectiveDate, effectiveDate);

            // Arrange
            

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/reportingstructure/{empId}");
            var response2 = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode);
            var comp2 = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(comp2.EmployeeId);
            Assert.AreEqual(comp2.EmployeeId, empId);
            Assert.AreEqual(comp2.Salary, salary);
            Assert.AreEqual(comp2.EffectiveDate, effectiveDate);
        }

    }
}
