using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using NUnit.Framework;

namespace Com.RelationalAI
{
    public class UnitTests
    {
        private DelveClient cloudApi;

        [SetUp]
        public void Setup()
        {
            createCloudConnection("testcsharpclient", out cloudApi);
        }

        public static void createCloudConnection(out DelveClient api) {
            string dbname = IntegrationTestsCommons.genDbname("testcsharpclient");

            createCloudConnection(dbname, out api);
        }

        public static void createCloudConnection(string dbname, out DelveClient api) {
            var conn = new CloudConnection(
                new Connection(dbname),
                creds: new RAICredentials(
                    "e3536f8d-cbc6-4ed8-9de6-74cf4cb724a1",
                    "484aiIGKitw91qppUTR0m8ge4grU+hUp65/MZ4bO0MY="
                ),
                verifySSL: false
            );
            api = new DelveClient(conn);
            api.debugLevel = 1;
        }

        [Test]
        public void Test1()
        {
            IntegrationTestsCommons.Test1(createCloudConnection);
        }

        [Test]
        public void Test2()
        {

            HttpRequestMessage httpReq = new HttpRequestMessage();
            httpReq.Method = HttpMethod.Get;
            httpReq.RequestUri = new Uri("https://127.0.0.1:8443/database");
            httpReq.Content = new StringContent("{}");
            httpReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpReq.Headers.Host = "127.0.0.1";
            RAIRequest req = new RAIRequest(httpReq, cloudApi.conn, service: "database+list");

            req.sign(DateTime.Parse("2020-05-04T10:36:00"), debugLevel: cloudApi.debugLevel);

            string output = string.Join(
                "\n",
                from header in req.innerReq.Headers.Union(req.innerReq.Content.Headers)
                orderby header.Key.ToLower()
                select string.Format(
                    "{0}: {1}",
                    header.Key.ToLower(),
                    string.Join(",", header.Value).Trim()
                )
            )+"\n";
            string expected =
                "authorization: RAI01-ED25519-SHA256 " +
                "Credential=e3536f8d-cbc6-4ed8-9de6-74cf4cb724a1/20200504/us-east/database+list/rai01_request, " +
                "SignedHeaders=content-type;host, " +
                "Signature=cbf601dfb7d2973b7e00484dc0819a9cd3cdfae61466d37287890da766af8e32ac2365bdab4b52e6ddd91a5926154115abec6142d080cd964ebb819a92fbb40c\n" +
                "content-type: application/json\n" +
                "host: 127.0.0.1\n";

            Assert.AreEqual(output, expected);
        }
    }
}
