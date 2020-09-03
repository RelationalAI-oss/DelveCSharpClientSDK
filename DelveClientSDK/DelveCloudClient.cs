using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Com.RelationalAI
{
    public partial class GeneratedDelveCloudClient
    {
        public Connection conn {get; set;}

        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            if (request.Content == null)
                request.Content = new StringContent("");
            // populate headers
            request.Headers.Clear();
            request.Content.Headers.Clear();
            request.Headers.Host = request.RequestUri.Host;
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            RAIRequest raiReq = new RAIRequest(request, conn);
            raiReq.Sign();
        }
    }

    public class DelveCloudClient : GeneratedDelveCloudClient
    {
        public DelveCloudClient(Connection conn) : base(new HttpClient())
        {
            this.conn = conn;
            this.BaseUrl = "https://" + this.conn.Host;
        }

        public ListComputesResponseProtocol listComputes() {return this.ComputeGetAsync().Result;}

        public ListDatabasesResponseProtocol listDatabases() {return this.DatabaseGetAsync().Result;}

        public ListUsersResponseProtocol listUsers() {return this.UserGetAsync().Result;}
    }
}
