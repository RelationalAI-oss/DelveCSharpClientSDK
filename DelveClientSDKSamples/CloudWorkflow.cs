using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Com.RelationalAI;
using IniParser.Model;
using Newtonsoft.Json.Linq;

namespace DelveClientSDKSamples
{
    class CloudWorkflow
    {
        CloudConnection cloudConn;
        DelveClient delveClient;
        DelveCloudClient mngtClient;

        int maxAttempts;
        int sleepTime;

        public CloudWorkflow(string computeName = "csharpcompute8", string profile = "default", int maxAttempts = 20, int sleepTime = 60000)
        {
            IniData ini = Config.loadDotRaiConfig();
            this.maxAttempts = maxAttempts;
            this.sleepTime = sleepTime;

            this.cloudConn = new CloudConnection(
                dbname: computeName,
                creds: RAICredentials.fromFile(profile: profile),
                scheme: "https",
                host: Config.raiGetHost(ini, profile),
                port: 443,
                verifySSL: false,
                computeName: computeName
            );

            this.delveClient = new DelveClient(conn: this.cloudConn);
            this.mngtClient = new DelveCloudClient(conn: this.cloudConn);
        }
        public void runCloudWorkflow()
        {
            // list computes
            var computes = this.mngtClient.listComputes();
            Console.WriteLine("=> Computes: " + JObject.FromObject(computes));

            // list databases
            var databases = this.mngtClient.listDatabases();
            Console.WriteLine("=> Databases: " + JObject.FromObject(databases).ToString());

            // list users
            var users = this.mngtClient.listUsers();
            Console.WriteLine("=> Users: " + JObject.FromObject(users).ToString());

            // create compute
            var createComputeResponse = this.mngtClient.createCompute(conn: cloudConn, size: "XS");
            Console.WriteLine("=> Create compute response: " + JObject.FromObject(createComputeResponse).ToString());

            // wait for compute to be provisioned
            if(!waitForCompute(this.cloudConn.computeName))
                return;

            // create database
            this.delveClient.createDatabase(conn: this.cloudConn, overwrite: true);

            // install source
            InstallActionResult sourceInstall = this.delveClient.installSource(conn: this.cloudConn, "name", "name", "def foo = 1");
            Console.WriteLine("=> InstallActionResult: " + JObject.FromObject(sourceInstall).ToString());

            // query
            QueryActionResult queryRes = this.delveClient.query(conn: this.cloudConn, srcStr: "def bar = 2 + foo", output: "bar");
            Console.WriteLine("=> QueryActionResult: " + JObject.FromObject(queryRes).ToString());

            // remove default compute
            this.mngtClient.removeDefaultCompute(conn: this.cloudConn);

            // delete compute
            var deleteComputeResponse = this.mngtClient.deleteCompute(conn: this.cloudConn);
            Console.WriteLine("=> DeleteComputeResponse: " + JObject.FromObject(deleteComputeResponse).ToString());
        }

        private bool waitForCompute(string computeName)
        {
            for (int i=0; i<this.maxAttempts; i++)
            {
                var compute = getByComputeName(computeName);
                string computeState = (string)compute["computeState"];
                if ("PROVISIONED".Equals(computeState))
                    return true;
                Console.WriteLine(String.Format("Current state: {0}. Waiting for {1} to be provisioned. (Attempt {2}).", computeState ,computeName, i+1));
                Thread.Sleep(this.sleepTime);
            }
            return false;
        }

        private JToken getByComputeId(string computeId)
        {
            return getByField("computeId", computeId);
        }

        private JToken getByComputeName(string computeName)
        {
            return getByField("computeName", computeName);
        }

        private JToken getByField(string field, string value)
        {
            var computes = this.mngtClient.listComputes();

            foreach (var compute in JObject.FromObject(computes)["compute_requests_list"])
            {
                string currentComputeField = (string)compute[field];
                if (value.Equals(currentComputeField))
                    return compute;
            }
            return null;
        }
    }
}
