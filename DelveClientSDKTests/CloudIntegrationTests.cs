using NUnit.Framework;

namespace Com.RelationalAI
{
    public class IntegrationTests
    {

        [SetUp]
        public void Setup()
        {
        }

        public static void createCloudConnection(out DelveClient api) {
            string dbname = IntegrationTestsCommons.genDbname("testcsharpclient");

            createCloudConnection(dbname, out api);
        }

        public static void createCloudConnection(string dbname, out DelveClient api) {
            string profile = "default";
            string computeName = dbname;
            var conn = new CloudConnection(
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
            IntegrationTestsCommons.Test1(createCloudConnection);
        }
    }
}
