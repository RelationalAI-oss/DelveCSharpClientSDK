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

        public CloudWorkflow(string computeName = "csharpcompute", string profile = "default")
        {
            IniData ini = Config.loadDotRaiConfig();

            this.cloudConn = new CloudConnection(
                dbname: computeName,
                creds: RAICredentials.fromFile(profile: profile),
                scheme: "https",
                host: Config.raiGetHost(ini, profile),
                port: 443,
                verifySSL: false,
                computeName: computeName
            );
        }
        public void runCloudWorkflow()
        {
            DelveCloudClient mngtClient = new DelveCloudClient(conn: this.cloudConn);
            DelveClient client = new DelveClient(conn: this.cloudConn);

            // list computes
            var computes = mngtClient.listComputes();
            Console.WriteLine(JObject.FromObject(computes).ToString());

            // list databases
            var databases = mngtClient.listDatabases();
            Console.WriteLine(JObject.FromObject(databases).ToString());

            // list users
            var users = mngtClient.listUsers();
            Console.WriteLine(JObject.FromObject(users).ToString());

            // create compute
            var createComputeResponse = mngtClient.createCompute(conn: cloudConn, size: "XS");
            Console.WriteLine(JObject.FromObject(createComputeResponse).ToString());

            // create database
            client.createDatabase(conn: this.cloudConn, overwrite: true);

            // install source
            InstallActionResult sourceInstall = client.installSource(conn: this.cloudConn, "name", "name", "def foo = 1");
            Console.WriteLine("InstallActionResult: " + JObject.FromObject(sourceInstall).ToString());

            // query
            QueryActionResult queryRes = client.query(conn: this.cloudConn, srcStr: "def bar = 2 + foo", output: "bar");
            Console.WriteLine("QueryActionResult: " + JObject.FromObject(queryRes).ToString());

            // remove default compute
            mngtClient.removeDefaultCompute(conn: this.cloudConn);

            // delete compute
            var deleteComputeResponse = mngtClient.deleteCompute(conn: this.cloudConn);
            Console.WriteLine(JObject.FromObject(deleteComputeResponse).ToString());
        }
    }
}
