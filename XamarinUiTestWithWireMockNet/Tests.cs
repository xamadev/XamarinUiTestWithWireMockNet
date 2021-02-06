using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace XamarinUiTestWithWireMockNet
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        private WireMockServer _server;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            var serverSettings = new WireMockServerSettings();
            //serverSettings.Port = 5005;
            serverSettings.AllowPartialMapping = true; //by default WireMock.NET returns 404 if request doesn't match exact parameter

            _server = WireMockServer.Start(serverSettings);

            _server
                .Given(Request.Create().WithPath("/test").UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBody(jsonResponse)
                );

            Console.WriteLine("Press any key to stop the server");
            //Console.ReadKey();
            //app = AppInitializer.StartApp(platform);
        }

        [Test]
        public async Task TestWireMock()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri(_server.Urls[0]);
            httpClient.Timeout = new TimeSpan(0, 0, 10);

            var response = await httpClient.GetAsync("/test");

            Assert.AreEqual(await response.Content.ReadAsStringAsync(), jsonResponse);
        }

        private string jsonResponse = @"{""$success"": ""1""}";
    }
}