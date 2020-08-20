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
        string dbname;
        IniData ini;
        CloudConnection cloudConn;

        public CloudWorkflow(string dbname = "testcsharpclient")
        {
            this.dbname = dbname;
            ini = Config.loadDotRaiConfig();

            cloudConn = new CloudConnection(
                dbname: dbname,
                creds: RAICredentials.fromFile(),
                host: Config.raiGetHost(ini)
            );
        }
        public void runCloudWorkflow()
        {
            DelveCloudClient cloudClient = new DelveCloudClient(this.cloudConn);
            var databases = cloudClient.listDatabases();
            var computes = cloudClient.listComputes();
            var users = cloudClient.listUsers();

            Console.WriteLine("Databases: " + JObject.FromObject(databases).ToString());
            Console.WriteLine("Computes: " + JObject.FromObject(computes).ToString());
            Console.WriteLine("Users: " + JObject.FromObject(users).ToString());
        }
    }
}
