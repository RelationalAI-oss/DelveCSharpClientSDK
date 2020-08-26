using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Com.RelationalAI;
using IniParser.Model;
using Newtonsoft.Json.Linq;

namespace DelveClientSDKSamples
{
    class CloudWorkflow
    {
        CloudConnection cloudConn;

        public CloudWorkflow(string dbname = "csharpdbtest", string computeName = "csharpcompute", string profile = "default")
        {
            IniData ini = Config.loadDotRaiConfig();

            this.cloudConn = new CloudConnection(
                dbname,
                creds: RAICredentials.fromFile(profile: profile),
                scheme: "https",
                host: Config.raiGetHost(ini, profile),
                port: 443,
                verifySSL: true,
                computeName: computeName
            );
        }
        public void runCloudWorkflow()
        {
            DelveCloudClient mngtClient = new DelveCloudClient(conn: this.cloudConn);
            DelveClient client = new DelveClient(conn: this.cloudConn);

            var computes = mngtClient.listComputes();
            var databases = mngtClient.listDatabases();
            var users = mngtClient.listUsers();

            Console.WriteLine(JObject.FromObject(computes).ToString());
            Console.WriteLine(JObject.FromObject(databases).ToString());
            Console.WriteLine(JObject.FromObject(users).ToString());

            var createComputeResponse = mngtClient.createCompute(conn: cloudConn, size: "XS");

            client.createDatabase(conn: this.cloudConn, overwrite: true);

            mngtClient.removeDefaultCompute(conn: this.cloudConn);
            var deleteComputeResponse = mngtClient.deleteCompute(conn: this.cloudConn);

            Console.WriteLine(JObject.FromObject(createComputeResponse).ToString());
            Console.WriteLine(JObject.FromObject(deleteComputeResponse).ToString());
        }
    }
}
