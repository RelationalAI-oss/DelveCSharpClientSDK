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
        public DelveCloudClient(HttpClient client) : base(client)
        {
        }

        public DelveCloudClient(CloudConnection conn) : base(DelveClient.CreateHttpClient(conn.VerifySSL))
        {
            this.conn = conn;
            this.conn.Client = this;
            this.BaseUrl = conn.BaseUrl.ToString();
        }

        public ListComputesResponseProtocol ListComputes() {return this.ComputeGetAsync().Result;}

        public CreateComputeResponseProtocol CreateCompute(string computeName, string size, bool dryRun = false)
        {
            return this.CreateCompute(computeName, size, EnumString.GetDescription(this.conn.Region), dryRun);
        }

        public CreateComputeResponseProtocol CreateCompute(string displayName, string size, string region, bool dryRun = false)
        {
            CreateComputeRequestProtocol request = new CreateComputeRequestProtocol();
            request.Region = region;
            request.Display_name = displayName;
            request.Size = size;
            request.Dryrun = dryRun;
            return this.ComputePutAsync(request).Result;
        }

        public DeleteComputeResponseProtocol DeleteCompute(string computeName, bool dryRun = false)
        {
            DeleteComputeRequestProtocol request = new DeleteComputeRequestProtocol();
            request.Compute_name = computeName;
            request.Dryrun = dryRun;
            return this.ComputeDeleteAsync(request).Result;
        }

        public ListDatabasesResponseProtocol ListDatabases() {return this.DatabaseGetAsync().Result;}

        public void RemoveDefaultCompute(string dbname)
        {
            this.UpdateDatabase(dbname, null, removeDefaultCompute: true, dryRun: false);
        }

        public void UpdateDatabase(string displayName, string defaultComputeName, bool removeDefaultCompute, bool dryRun = false)
        {
            UpdateDatabaseRequestProtocol request = new UpdateDatabaseRequestProtocol();
            request.Display_name = displayName;
            request.Default_compute_name = defaultComputeName;
            request.Remove_default_compute = removeDefaultCompute;
            request.Dryrun = dryRun;
            this.DatabasePostAsync(request).Wait();
        }
        public ListUsersResponseProtocol ListUsers() {return this.UserGetAsync().Result;}

        public CreateUserResponseProtocol CreateUser(string username, string firstName, string lastName, string email, bool dryRun = false)
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
