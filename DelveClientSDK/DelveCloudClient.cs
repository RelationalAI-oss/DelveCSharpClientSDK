using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Com.RelationalAI
{

    public enum RAIComputeSize
    {
        XS, S, M, L, XL
    }

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
            DelveClient.AddExtraHeaders(request);
        }
    }

    public class DelveCloudClient : GeneratedDelveCloudClient
    {
        public DelveCloudClient(HttpClient client) : base(client)
        {
            // This constructor is only here to avoid touching the generated code.
            // THIS CONSTRUCTOR SHOULD NOT BE USED.
            throw new InvalidOperationException();
        }

        public DelveCloudClient(Connection conn) : base(DelveClient.GetHttpClient(conn.BaseUrl, conn.VerifySSL, conn.ConnectionTimeout))
        {
            this.conn = conn;
            this.conn.CloudClient = this;
            this.BaseUrl = conn.BaseUrl.ToString();
        }

        public ICollection<ComputeData> ListComputes()
        {
            var res = this.ComputeGetAsync().Result;
            if ( res == null ) return null;

            return res.Compute_requests_list;
        }

        public ComputeData CreateCompute(string displayName, RAIComputeSize size = RAIComputeSize.XS, string region = null, bool dryRun = false)
        {
            if(region == null) region = EnumString.GetDescription(this.conn.Region);

            CreateComputeRequestProtocol request = new CreateComputeRequestProtocol();
            request.Region = region;
            request.Display_name = displayName;
            request.Size = EnumString.GetDescription(size);
            request.Dryrun = dryRun;
            return this.ComputePutAsync(request).Result.Compute_data;
        }

        public DeleteComputeStatus DeleteCompute(string computeName, bool dryRun = false)
        {
            DeleteComputeRequestProtocol request = new DeleteComputeRequestProtocol();
            request.Compute_name = computeName;
            request.Dryrun = dryRun;
            return this.ComputeDeleteAsync(request).Result.Delete_status;
        }

        public ICollection<DatabaseInfo> ListDatabases() {
            var res = this.DatabaseGetAsync().Result;
            if ( res == null ) return null;

            return res.Databases;
        }

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
        public ICollection<UserInfoProtocol> ListUsers()
        {
            return this.UserGetAsync().Result.Users;
        }

        public Tuple<UserInfoProtocol, string> CreateUser(string username, bool dryRun = false)
        {
            CreateUserRequestProtocol request = new CreateUserRequestProtocol();
            request.Username = username;
            request.Dryrun = dryRun;
            var res = this.UserPutAsync(request).Result;
            return new Tuple<UserInfoProtocol, string>(res.User, res.Private_key);
        }
    }
}
