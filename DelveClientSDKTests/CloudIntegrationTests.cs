using NUnit.Framework;

namespace Com.RelationalAI
{
    public class IntegrationTests
    {

        [SetUp]
        public void Setup()
        {
        }

        public static void createCloudConnection(out LocalConnection conn) {
            string dbname = IntegrationTestsCommons.genDbname("testcsharpclient");

            createCloudConnection(dbname, out conn);
        }

        public static void createCloudConnection(string dbname, out LocalConnection conn) {
            string computeName = dbname;
            conn = new CloudConnection(
                dbname,
                scheme: "https",
                host: "azure-ssh.relationalai.com",
                port: 443,
                verifySSL: false,
                computeName: computeName
            );

            new DelveClient(conn); //to register the connection with a client
        }

        [Test]
        public void Test1()
        {
            IntegrationTestsCommons.RunAllTests(createCloudConnection);
        }
    }
}
