using System;
using Newtonsoft.Json.Linq;
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
            ManagementConnection mgmtConn = new ManagementConnection(
                scheme: "https",
                host: "azure-ssh.relationalai.com",
                port: 443,
                verifySSL: false
            );

            var computes = mgmtConn.ListComputes();

            ComputeData provisionedCompute = null;

            foreach(var comp in computes) {
                if( "PROVISIONED".Equals(comp.AdditionalProperties["computeState"]) )
                {
                    var databases = mgmtConn.ListDatabases();
                    foreach(var db in databases) {
                        if( comp.ComputeName.Equals(db.Default_compute_name) )
                        {
                            return CreateCloudConnection(db.Name, comp.ComputeName, mgmtConn);
                        }
                    }

                    provisionedCompute = comp;
                    break;
                }
            }

            string dbname = IntegrationTestsCommons.genDbname("testcsharpclient");
            if( provisionedCompute == null )
            {
                var createComputeRes = mgmtConn.CreateCompute(dbname);
                if( createComputeRes == null ) return null;
                provisionedCompute = createComputeRes.Compute_data;
            }
            return CreateCloudConnection(dbname, provisionedCompute.ComputeName, mgmtConn);
        }

        public static CloudConnection CreateCloudConnection(string dbname, string computeName, ManagementConnection mgmtConn) {
            return new CloudConnection(
                dbname,
                mgmtConn,
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
