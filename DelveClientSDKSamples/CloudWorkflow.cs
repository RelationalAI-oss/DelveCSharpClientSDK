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
        CloudConnection CloudConn;
        DelveClient DelveClient;

        int MaxAttempts;
        int SleepTime;

        public CloudWorkflow(string computeName = "csharpcompute", string profile = "default", int maxAttempts = 20, int sleepTime = 60000)
        {
            // Loads data from ~/.rai/config (rai cloud configuration)
            IniData ini = Config.LoadDotRaiConfig();
            this.MaxAttempts = maxAttempts;
            this.SleepTime = sleepTime;

            this.CloudConn = new CloudConnection(
                dbname: computeName,
                creds: RAICredentials.FromFile(profile: profile),
                scheme: "https",
                host: Config.RaiGetHost(ini, profile),
                port: 443,
                verifySSL: false,
                computeName: computeName
            );

            // creates a database client for data load.
            this.DelveClient = new DelveClient(conn: this.CloudConn);
            // we are working on merging both clients into single one

            this.DelveClient.DebugLevel = 1;
        }

        /*
         * Cloud workflow using RAICloud
         *
        */
        public void runCloudWorkflow()
        {
            // list computes for the current account
            /* Expected output: {
            "compute_requests_list": [
            {
                "id": "5170d1ac-e5f4-4547-a4f4-37dbca58048a",
                "accountName": "relationalai-db",
                "createdBy": "924c1f35-5062-4e78-9ce9-10157c29dc61",
                "computeName": "csharpcompute",
                "computeSize": "XS",
                "computeRegion": "us-east",
                "requestedOn": "2020-08-27T15:50:18.488Z",
                "createdOn": "2020-08-27T15:52:10.040Z",
                "deletedOn": "2020-08-27T23:38:34.062Z",
                "computeState": "DELETED",
                "computeId": "b4d958cf-8772-48f4-8b2c-7a37bdd415b5",
                "_etag": "\"0a002297-0000-0100-0000-5f4d07c30000\""
            }, ... ]}
              */
            var computes = this.DelveClient.ListComputes();
            Console.WriteLine("=> Computes: " + JObject.FromObject(computes));

            // list databases for the current account
            /* Expected output: {
            "databases": [
                {
                  "account_name": "relationalai-db",
                  "name": "csharpdbtest",
                  "region": "us-east",
                  "database_id": "a659009a-dae8-4a90-8438-41f5ac6bbb2d",
                  "status": "CREATED"
                },
                {
                  "account_name": "relationalai-db",
                  "name": "csharpcompute2",
                  "region": "us-east",
                  "database_id": "05e90e99-5601-4630-8696-5656eb0b31d2",
                  "status": "CREATED"
                }, ... ]}
            */
            var databases = this.DelveClient.ListDatabases();
            Console.WriteLine("=> Databases: " + JObject.FromObject(databases).ToString());

            // list users for the current account
            /* Expected output: {
              "users": [
                  {
                    "account_name": "account_name",
                    "username": "username",
                    "first_name": "firstname",
                    "last_name": "lastname",
                    "email": "user@relational.ai",
                    "status": "ACTIVE",
                    "access_key1": "xxxxxxxxxxxxxxxxxxxxxx"
                  }, ...
                ]}
            */
            var users = this.DelveClient.ListUsers();
            Console.WriteLine("=> Users: " + JObject.FromObject(users).ToString());

            // create compute
            var createComputeResponse = this.DelveClient.CreateCompute(computeName: this.CloudConn.ComputeName, size: "XS");
            Console.WriteLine("=> Create compute response: " + JObject.FromObject(createComputeResponse).ToString());

            // wait for compute to be provisioned
            // a compute is a single tenant VM used for the current account (provisioning time ~ 5 mins)
            if(!WaitForCompute(this.CloudConn.ComputeName))
                return;

            // create database with the name as specificied in the CloudConnection
            this.DelveClient.CreateDatabase(overwrite: true);

            this.DelveClient.LoadCSV(
                // import data into edge_csv relation
                rel: "edge_csv",
                // data type mapping
                schema: new CSVFileSchema("Int64", "Int64"),
                syntax: new CSVFileSyntax(header: new List<string>() { "src", "dest" }, delim: "|"),
                // data imported over the wire
                // alternative options are to specify a datasource that is a path for an azure blob storage file
                data: @"
                        30|31
                        33|30
                        32|31
                        34|35
                        35|32
                    "
            );

            // persisting vertex and edges for future computations
            var edges = this.DelveClient.Query(
                srcStr: @"
                    def vertex(id) = exists(pos: edge_csv(pos, :src, id) or edge_csv(pos, :dest, id))
                    def edge(a, b) = exists(pos: edge_csv(pos, :src, a) and edge_csv(pos, :dest, b))
                ",
                persist: new List<string>() { "vertex", "edge" },
                // this the result of the query
                output: "edge"
            );

            Console.WriteLine("==> Query output: " + JObject.FromObject(edges).ToString());

            // Jaccard Similarity query
            string queryString = @"
                def uedge(a, b) = edge(a, b) or edge(b, a)
                def tmp(a, b, x) = uedge(x,a) and uedge(x,b) and a > b
                def jaccard_similarity(a,b,v) = (count[x : tmp(a,b,x)] / count[x: (uedge(a, x) or uedge(b, x)) and tmp(a,b,_)])(v)

                def result = jaccard_similarity
            ";

            var queryResult = this.DelveClient.Query(
                srcStr: queryString,
                // query output
                output: "result"
            );

            Console.WriteLine("=> Jaccard Similarity query result: " + JObject.FromObject(queryResult).ToString());

            // remove default compute (disassociate database from compute)
            this.DelveClient.RemoveDefaultCompute(dbname: this.CloudConn.DbName);

            // delete compute => stop charging for the compute
            var deleteComputeResponse = this.DelveClient.DeleteCompute(computeName: this.CloudConn.ComputeName);
            Console.WriteLine("=> DeleteComputeResponse: " + JObject.FromObject(deleteComputeResponse).ToString());
        }

        /*
         * Helpers
         *
         */

        private bool WaitForCompute(string computeName)
        {
            for (int i=0; i<this.MaxAttempts; i++)
            {
                var compute = GetByComputeName(computeName);
                string computeState = (string)compute["computeState"];
                if ("PROVISIONED".Equals(computeState))
                    return true;
                Console.WriteLine(String.Format("Current state: {0}. Waiting for {1} to be provisioned. (Attempt {2}).", computeState ,computeName, i+1));
                Thread.Sleep(this.SleepTime);
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
            var computes = this.DelveClient.ListComputes();

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
