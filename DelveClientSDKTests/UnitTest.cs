using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;
using NSec.Cryptography;
using NUnit.Framework;

namespace Com.RelationalAI
{
    public class Tests
    {
        private DelveClient api;
        string dbname;
        Connection conn;

        [SetUp]
        public void Setup()
        {
            api = new DelveClient();

            dbname = "testcsharpclient";
            conn = new Connection(
                dbname,
                creds: new RAICredentials(
                    "e3536f8d-cbc6-4ed8-9de6-74cf4cb724a1",
                    "484aiIGKitw91qppUTR0m8ge4grU+hUp65/MZ4bO0MY="
                )
            );
        }

        [Test]
        public void Test1()
        {
            //TransactionResult res = client.TransactionAsync(new Transaction()).Result;
            //Console.WriteLine(res);
            Assert.IsTrue(api.create_database(conn, true));
            Assert.IsFalse(api.create_database(conn, false));


            InstallActionResult source_install = api.install_source(conn, "name", "name", "def foo = 1");
            Assert.IsNotNull(source_install);

            QueryActionResult query_res = api.query(conn, "name", "name", "def bar = 2", "bar");
            Assert.IsNotNull(query_res);
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
            RAIRequest req = new RAIRequest(httpReq, conn, service: "database+list");

            req.sign(DateTime.Parse("2020-05-04T10:36:00"));

            string output = string.Join(
                "\n",
                from header in req.inner_req.Headers.Union(req.inner_req.Content.Headers)
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
