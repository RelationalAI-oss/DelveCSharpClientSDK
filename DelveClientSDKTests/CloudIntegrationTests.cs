using NUnit.Framework;

namespace Com.RelationalAI
{
    public class CloudIntegrationTests
    {

        [SetUp]
        public void Setup()
        {
        }

        public static CloudConnection CreateCloudConnection() {
            string dbname = "testclientdb-0aa5c968-9ca1-4d14-9e76-06fc67bce463";

            return CreateCloudConnection(dbname);
        }

        public static CloudConnection CreateCloudConnection(string dbname) {
            string computeName = dbname;
            return new CloudConnection(
                dbname,
                scheme: "https",
                host: "azure-ssh.relationalai.com",
                port: 443,
                verifySSL: false,
                computeName: computeName
            );

        }

        [Test]
        public void Test1()
        {
            IntegrationTestsCommons.RunAllTests(CreateCloudConnection);
        }
    }
}
