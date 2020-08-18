using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Com.RelationalAI;
using IniParser.Model;

namespace DelveClientSDKSamples
{
    class CloudWorkflow
    {
        string dbname;
        string host;
        string baseUrl;
        string accessKey;
        string privateKey;

        Connection localConn;
        CloudConnection cloudConn;
        DelveClient cloudClient;
        HttpClient httpClient;

        public CloudWorkflow(string dbname = "testcsharpclient")
        {
            this.dbname = dbname;

            IniData iniData = Config.load_dot_rai_config();
            this.host = Config.rai_get_host(iniData);
            this.baseUrl = "https://" + this.host;

            this.localConn = new Connection(this.dbname);
            this.cloudConn = new CloudConnection(
                this.localConn,
                creds: new RAICredentials(
                    "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                    "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
                ),
                verifySSL: false
            );
            this.cloudClient = new DelveClient(cloudConn);
            this.httpClient = new HttpClient();
            this.cloudClient.debugLevel = 1;
        }
        private void sendHttpRequest(HttpMethod method, string endpoint, string content, string service)
        {
            HttpRequestMessage httpReq = new HttpRequestMessage();
            httpReq.Method = HttpMethod.Get;
            httpReq.RequestUri = new Uri(baseUrl + endpoint);
            httpReq.Content = new StringContent(content);
            httpReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpReq.Headers.Host = this.host;
            RAIRequest req = new RAIRequest(httpReq, cloudConn, service: service);

            req.sign(debugLevel: cloudClient.debugLevel);
            HttpResponseMessage response = httpClient.SendAsync(req.innerReq).Result;
            Console.WriteLine("HttpResponseMessage: " + JObject.FromObject(response).ToString());
            Console.WriteLine("Content: " + response.Content.ReadAsStringAsync().Result);

        }
        public void listDatabases()
        {
            sendHttpRequest(HttpMethod.Get, "/database", "{}", "database+list");
        }

        public void listComputes()
        {
            sendHttpRequest(HttpMethod.Get, "/compute", "{}", "compute+list");
        }

        public void runCloudWorkflow()
        {
            listDatabases();
            listComputes();
        }
    }
}
