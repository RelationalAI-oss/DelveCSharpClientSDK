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
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);

            dbname = "testcsharpclient";
            conn = new Connection(
                dbname,
                creds: new RAICredentials(
                    "e3536f8d-cbc6-4ed8-9de6-74cf4cb724a1",
                    "484aiIGKitw91qppUTR0m8ge4grU+hUp65/MZ4bO0MY="
                ),
                scheme: "https",
                host: "127.0.0.1",
                port: 8443
            );

            api = new DelveClient(conn);
        }

        [Test]
        public void Test1()
        {
            Assert.IsTrue(api.create_database(conn, true));
            Assert.IsFalse(api.create_database(conn, false));


            InstallActionResult source_install = api.install_source(conn, "name", "name", "def foo = 1");
            Assert.IsNotNull(source_install);

            QueryActionResult query_res = api.query(conn, "name", "name", "def bar = 2", "bar");
            Assert.IsNotNull(query_res);
        }
    }
}
