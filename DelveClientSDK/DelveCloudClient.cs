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
            raiReq.sign();
        }
    }

    public class DelveCloudClient : GeneratedDelveCloudClient
    {
        public DelveCloudClient(CloudConnection conn) : base(DelveClient.createHttpClient(conn.verifySSL))
        {
            this.conn = conn;
            this.BaseUrl = conn.baseUrl.ToString();
        }

        public ListComputesResponseProtocol listComputes() {return this.ComputeGetAsync().Result;}

        public CreateComputeResponseProtocol createCompute(CloudConnection conn, string size, bool dryRun = false)
        {
            return this.createCompute(conn.computeName, size, EnumString.GetDescription(conn.region), dryRun);
        }

        public CreateComputeResponseProtocol createCompute(string displayName, string size, string region, bool dryRun = false)
        {
            CreateComputeRequestProtocol request = new CreateComputeRequestProtocol();
            request.Region = region;
            request.Display_name = displayName;
            request.Size = size;
            request.Dryrun = dryRun;
            return this.ComputePutAsync(request).Result;
        }

        public DeleteComputeResponseProtocol deleteCompute(CloudConnection conn, bool dryRun = false)
        {
            return this.deleteCompute(conn.computeName, dryRun);
        }

        public DeleteComputeResponseProtocol deleteCompute(string computeName, bool dryRun = false)
        {
            DeleteComputeRequestProtocol request = new DeleteComputeRequestProtocol();
            request.Compute_name = computeName;
            request.Dryrun = dryRun;
            return this.ComputeDeleteAsync(request).Result;
        }

        public ListDatabasesResponseProtocol listDatabases() {return this.DatabaseGetAsync().Result;}

        public void removeDefaultCompute(CloudConnection conn)
        {
            this.updateDatabase(conn.dbname, null, removeDefaultCompute: true, dryRun: false);
        }

        public void updateDatabase(string displayName, string defaultComputeName, bool removeDefaultCompute, bool dryRun = false)
        {
            UpdateDatabaseRequestProtocol request = new UpdateDatabaseRequestProtocol();
            request.Display_name = displayName;
            request.Default_compute_name = defaultComputeName;
            request.Remove_default_compute = removeDefaultCompute;
            request.Dryrun = dryRun;
            this.DatabasePostAsync(request).Wait();
        }
        public ListUsersResponseProtocol listUsers() {return this.UserGetAsync().Result;}

        public CreateUserResponseProtocol createUser(string username, string firstName, string lastName, string email, bool dryRun = false)
        {
            CreateUserRequestProtocol request = new CreateUserRequestProtocol();
            request.Username = username;
            request.First_name = firstName;
            request.Last_name = lastName;
            request.Email = email;
            request.Dryrun = dryRun;
            return this.UserPutAsync(request).Result;
        }
    }
}

