using NUnit.Framework;

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
            string profile = "default";
            dbname = "testclientdb";
            string computeName = dbname;
            conn = new CloudConnection(
                dbname,
                creds: RAICredentials.fromFile(profile: profile),
                scheme: "https",
                host: string.Format("azure-ssh.relationalai.com", profile),
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
            IntegrationTestsCommons.Test1(api, conn);
        }
    }
}
