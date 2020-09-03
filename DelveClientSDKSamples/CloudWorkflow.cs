using System;
using System.Collections.Generic;
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

        public CloudWorkflow(string computeName = "csharpcompute9", string profile = "default", int maxAttempts = 20, int sleepTime = 60000)
        {
            IniData ini = Config.LoadDotRaiConfig();
            this.maxAttempts = maxAttempts;
            this.sleepTime = sleepTime;

            this.cloudConn = new CloudConnection(
                dbname: computeName,
                creds: RAICredentials.FromFile(profile: profile),
                scheme: "https",
                host: Config.RaiGetHost(ini, profile),
                port: 443,
                verifySSL: false,
                computeName: computeName
            );

            this.delveClient = new DelveClient(conn: this.cloudConn);
            this.mngtClient = new DelveCloudClient(conn: this.cloudConn);
            this.delveClient.debugLevel = 1;
        }
        public void runCloudWorkflow()
        {
            // list computes
            var computes = this.mngtClient.ListComputes();
            Console.WriteLine("=> Computes: " + JObject.FromObject(computes));

            // list databases
            var databases = this.mngtClient.ListDatabases();
            Console.WriteLine("=> Databases: " + JObject.FromObject(databases).ToString());

            // list users
            var users = this.mngtClient.ListUsers();
            Console.WriteLine("=> Users: " + JObject.FromObject(users).ToString());

            // create compute
            var createComputeResponse = this.mngtClient.CreateCompute(conn: cloudConn, size: "XS");
            Console.WriteLine("=> Create compute response: " + JObject.FromObject(createComputeResponse).ToString());

            // wait for compute to be provisioned
            if(!WaitForCompute(this.cloudConn.ComputeName))
                return;

            // create database
            this.delveClient.CreateDatabase(overwrite: true);

            this.delveClient.LoadCSV(
                rel: "edge_csv",
                schema: new CSVFileSchema("Int64", "Int64"),
                syntax: new CSVFileSyntax(header: new List<string>() { "src", "dest" }, delim: "|"),
                data: @"
                        30|31
                        33|30
                        32|31
                        34|35
                        35|32
                    "
            );

            this.delveClient.Query(
                srcStr: @"
                    def vertex(id) = exists(pos: edge_csv(pos, :src, id) or edge_csv(pos, :dest, id))
                    def edge(a, b) = exists(pos: edge_csv(pos, :src, a) and edge_csv(pos, :dest, b))
                ",
                persist: new List<string>() { "vertex", "edge" },
                output: "edge"
            );

            string queryString = @"
                def uedge(a, b) = edge(a, b) or edge(b, a)
                def tmp(a, b, x) = uedge(x,a) and uedge(x,b) and a > b
                def jaccard_similarity(a,b,v) = (count[x : tmp(a,b,x)] / count[x: (uedge(a, x) or uedge(b, x)) and tmp(a,b,_)])(v)

                def result = jaccard_similarity
            ";

            var queryResult = this.delveClient.Query(
                srcStr: queryString,
                output: "result"
            );

            Console.WriteLine("=> Jaccard Similarity query result: " + queryResult);

            // remove default compute
            this.mngtClient.RemoveDefaultCompute(conn: this.cloudConn);

            // delete compute
            var deleteComputeResponse = this.mngtClient.DeleteCompute(conn: this.cloudConn);
            Console.WriteLine("=> DeleteComputeResponse: " + JObject.FromObject(deleteComputeResponse).ToString());
        }

        private bool WaitForCompute(string computeName)
        {
            for (int i=0; i<this.maxAttempts; i++)
            {
                var compute = GetByComputeName(computeName);
                string computeState = (string)compute["computeState"];
                if ("PROVISIONED".Equals(computeState))
                    return true;
                Console.WriteLine(String.Format("Current state: {0}. Waiting for {1} to be provisioned. (Attempt {2}).", computeState ,computeName, i+1));
                Thread.Sleep(this.sleepTime);
            }
            return false;
        }

        private JToken GetByComputeId(string computeId)
        {
            return GetByField("computeId", computeId);
        }

        private JToken GetByComputeName(string computeName)
        {
            return GetByField("computeName", computeName);
        }

        private JToken GetByField(string field, string value)
        {
            var computes = this.mngtClient.ListComputes();

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
