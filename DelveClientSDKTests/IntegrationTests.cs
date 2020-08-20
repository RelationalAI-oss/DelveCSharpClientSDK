using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;
using NSec.Cryptography;
using NUnit.Framework;
using System.IO;
using System.Runtime.InteropServices;

namespace Com.RelationalAI
{
    public class IntegrationTests
    {
        string dbname;
        Connection conn;
        private DelveClient api;

        [SetUp]
        public void Setup()
        {
            string profile = "azure-ssh";
            dbname = "testdb5";
            string computeName = dbname;
            conn = new CloudConnection(
                dbname,
                creds: RAICredentials.fromFile(profile: profile),
                scheme: "https",
                host: string.Format("{0}.relationalai.com", profile),
                port: 443,
                verifySSL: false,
                computeName: computeName
            );

            api = new DelveClient(conn);
            api.debugLevel = 1;
        }

        [Test]
        public void Test1()
        {
            Assert.IsTrue(api.createDatabase(conn, true));
            Assert.IsFalse(api.createDatabase(conn, false));

            InstallActionResult sourceInstall = api.installSource(conn, "name", "name", "def foo = 1");
            Assert.IsNotNull(sourceInstall);

            QueryActionResult queryRes = api.query(conn, "name", "name", "def bar = 2", "bar");
            Assert.IsNotNull(queryRes);
        }
    }
}
