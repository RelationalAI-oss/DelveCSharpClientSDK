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

        public CloudWorkflow(string dbname = "csharpdbtest6", string computeName = "csharpcompute6", string profile = "default")
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
            mngtClient.createCompute(conn: cloudConn, size: "XS", dryRun: false);
            client.createDatabase(conn: this.cloudConn, overwrite: true);
            mngtClient.removeDefaultCompute(conn: this.cloudConn);
            mngtClient.deleteCompute(conn: this.cloudConn, dryRun: true);
        }
    }
}
