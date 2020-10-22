using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using NUnit.Framework;

namespace Com.RelationalAI
{
    public class LocalIntegrationTests
    {

        [SetUp]
        public void Setup()
        {
        }

        public static LocalConnection CreateLocalConnection() {
            string dbname = IntegrationTestsCommons.genDbname("testcsharpclient");

            return CreateLocalConnection(dbname);
        }

        public static LocalConnection CreateLocalConnection(string dbname) {
            return new LocalConnection(dbname);
        }

        [Test]
        public void Test1()
        {
            IntegrationTestsCommons.RunAllTests(CreateLocalConnection);
        }

        [Test]
        public void Test2()
        {
            string dbname = IntegrationTestsCommons.genDbname("testcsharpclient");

            CloudConnection cloudConn = new CloudConnection(
                new LocalConnection(dbname),
                creds: new RAICredentials(
                    "e3536f8d-cbc6-4ed8-9de6-74cf4cb724a1",
                    "484aiIGKitw91qppUTR0m8ge4grU+hUp65/MZ4bO0MY="
                ),
                verifySSL: false
            );

            HttpRequestMessage httpReq = new HttpRequestMessage();
            httpReq.Method = HttpMethod.Get;
            httpReq.RequestUri = new Uri("https://127.0.0.1:8443/database");
            httpReq.Content = new StringContent("{}");
            httpReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpReq.Headers.Host = "127.0.0.1";
            RAIRequest req = new RAIRequest(httpReq, cloudConn, service: "database+list");

            req.Sign(DateTime.Parse("2020-05-04T10:36:00"), debugLevel: cloudConn.DebugLevel);

            string output = string.Join(
                "\n",
                from header in req.InnerReq.Headers.Union(req.InnerReq.Content.Headers)
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
