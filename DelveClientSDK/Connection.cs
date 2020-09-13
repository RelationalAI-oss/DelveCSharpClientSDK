using System;
using System.Collections.Generic;

namespace Com.RelationalAI
{
    using AnyValue = System.Object;

    public abstract class Connection
    {
        public const string DEFAULT_SCHEME = "http";
        public const string DEFAULT_HOST = "127.0.0.1";
        public const int DEFAULT_PORT = 8010;
        public const TransactionMode DEFAULT_OPEN_MODE = TransactionMode.OPEN;
        public const RAIInfra DEFAULT_INFRA = RAIInfra.AZURE;
        public const RAIRegion DEFAULT_REGION = RAIRegion.US_EAST;
        public const bool DEFAULT_VERIFY_SSL = true;
        public const int DEFAULT_DEBUG_LEVEL = 0;

        public virtual string DbName => throw new InvalidOperationException();

        public virtual TransactionMode DefaultOpenMode => throw new InvalidOperationException();

        public virtual string Scheme => throw new InvalidOperationException();

        public virtual string Host => throw new InvalidOperationException();

        public virtual int Port => throw new InvalidOperationException();

        public virtual RAIInfra Infra => throw new InvalidOperationException();

        public virtual RAIRegion Region => throw new InvalidOperationException();

        public virtual RAICredentials Creds => throw new InvalidOperationException();

        public virtual bool VerifySSL => throw new InvalidOperationException();

        public virtual string ComputeName => throw new InvalidOperationException();

        public Uri BaseUrl {
            get { return new UriBuilder(this.Scheme, this.Host, this.Port).Uri; }
        }

        public DelveCloudClient Client { get; set; }

        public int DebugLevel {
            get{ return Client is DelveClient ? ((DelveClient) Client).DebugLevel : DEFAULT_DEBUG_LEVEL; }
            set { if(Client is DelveClient) ((DelveClient) Client).DebugLevel = value; }
        }
    }

    /// <summary>
    /// Connection working with databases in a locally running Delve server.
    /// </summary>
    public class LocalConnection : Connection
    {
        /// <summary>
        /// Connection working with databases in a locally running Delve server.
        /// </summary>
        /// <param name="dbname">database to execute transactions with</param>
        /// <param name="defaultOpenMode">`= TransactionMode.OPEN`: How to open the database `dbname`</param>
        /// <param name="scheme">= `http`: The scheme used for connecting to a running server (e.g., `http`)</param>
        /// <param name="host"> = `127.0.0.1`: The host of a running server.</param>
        /// <param name="port"> = `8010`: The port of a running server.</param>
        public LocalConnection(
            string dbname,
            TransactionMode defaultOpenMode = DEFAULT_OPEN_MODE,
            string scheme = DEFAULT_SCHEME,
            string host = DEFAULT_HOST,
            int port = DEFAULT_PORT)
        {
            this.DbName = dbname;
            this.DefaultOpenMode = defaultOpenMode;
            this.Scheme = scheme;
            this.Host = host;
            this.Port = port;

            if(this.GetType() == typeof(LocalConnection))
            {
                new DelveClient(this); //to register the connection with a client
            }
            else
            {
                // If it's a subtype of `LocalConnection`, then its association to a `DelveClient`
                // is done separately in the leaf class.
            }
        }

        public override string DbName { get; }
        public override TransactionMode DefaultOpenMode { get; }
        public override string Scheme { get; }
        public override string Host { get; }
        public override int Port { get; }
        public override bool VerifySSL => DEFAULT_VERIFY_SSL;

        public bool BranchDatabase(string sourceDbname)
        {
            Client.conn = this;
            return ((DelveClient) Client).BranchDatabase(sourceDbname);
        }

        public bool CreateDatabase(bool overwrite = false)
        {
            Client.conn = this;
            return ((DelveClient) Client).CreateDatabase(overwrite);
        }

        public bool InstallSource(Source src)
        {
            Client.conn = this;
            return ((DelveClient) Client).InstallSource(src);
        }

        public bool InstallSource(String name, String srcStr)
        {
            Client.conn = this;
            return ((DelveClient) Client).InstallSource(name, srcStr);
        }

        public bool InstallSource(String name, String path, String srcStr)
        {
            Client.conn = this;
            return ((DelveClient) Client).InstallSource(name, path, srcStr);
        }

        public bool InstallSource(ICollection<Source> srcList)
        {
            Client.conn = this;
            return ((DelveClient) Client).InstallSource(srcList);
        }

        public bool DeleteSource(ICollection<string> srcNameList)
        {
            Client.conn = this;
            return ((DelveClient) Client).DeleteSource(srcNameList);
        }

        public bool DeleteSource(string srcName)
        {
            Client.conn = this;
            return ((DelveClient) Client).DeleteSource(srcName);
        }

        public IDictionary<string, Source> ListSource()
        {
            Client.conn = this;
            return ((DelveClient) Client).ListSource();
        }

        public IDictionary<RelKey, Relation> Query(
            string output,
            string name = "query",
            string path = null,
            string srcStr = "",
            ICollection<Relation> inputs = null,
            ICollection<string> persist = null,
            bool? isReadOnly = null,
            TransactionMode? mode = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).Query(output, name, path, srcStr, inputs, persist, isReadOnly, mode);
        }

        public IDictionary<RelKey, Relation> Query(
            string name = "query",
            string path = null,
            string srcStr = "",
            ICollection<Relation> inputs = null,
            ICollection<string> outputs = null,
            ICollection<string> persist = null,
            bool? isReadOnly = null,
            TransactionMode? mode = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).Query(name, path, srcStr, inputs, outputs, persist, isReadOnly, mode);
        }

        public IDictionary<RelKey, Relation> Query(
            Source src = null,
            ICollection<Relation> inputs = null,
            ICollection<string> outputs = null,
            ICollection<string> persist = null,
            bool? isReadOnly = null,
            TransactionMode? mode = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).Query(src, inputs, outputs, persist, isReadOnly, mode);
        }

        public bool UpdateEdb(
            RelKey rel,
            ICollection<Tuple<AnyValue, AnyValue>> updates = null,
            ICollection<Tuple<AnyValue, AnyValue>> delta = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).UpdateEdb(rel, updates, delta);
        }

        public LoadData JsonString(
            string data,
            AnyValue key = null,
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).JsonString(data, key, syntax, schema);
        }

        public LoadData JsonFile(
            string filePath,
            AnyValue key = null,
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).JsonString(filePath, key, syntax, schema);
        }

        public LoadData CsvString(
            string data,
            AnyValue key = null,
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).JsonString(data, key, syntax, schema);
        }

        public LoadData CsvFile(
            string filePath,
            AnyValue key = null,
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).CsvFile(filePath, key, syntax, schema);
        }

        public bool LoadEdb(
            string rel,
            string contentType,
            string data = null,
            string path = null,
            AnyValue key = null,
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).LoadEdb(rel, contentType, data, path, key, syntax, schema);
        }

        public bool LoadEdb(string rel, LoadData value)
        {
            Client.conn = this;
            return ((DelveClient) Client).LoadEdb(rel, value);
        }

        public bool LoadEdb(string relName, AnyValue[][] columns)
        {
            Client.conn = this;
            return ((DelveClient) Client).LoadEdb(relName, columns);
        }

        public bool LoadEdb(string relName, ICollection<ICollection<AnyValue>> columns)
        {
            Client.conn = this;
            return ((DelveClient) Client).LoadEdb(relName, columns);
        }

        public bool LoadEdb(RelKey relKey, ICollection<ICollection<AnyValue>> columns)
        {
            Client.conn = this;
            return ((DelveClient) Client).LoadEdb(relKey, columns);
        }

        public bool LoadEdb(Relation value)
        {
            Client.conn = this;
            return ((DelveClient) Client).LoadEdb(value);
        }

        public bool LoadEdb(ICollection<Relation> value)
        {
            Client.conn = this;
            return ((DelveClient) Client).LoadEdb(value);
        }

        public bool LoadCSV(
            string rel,
            string data = null,
            string path = null,
            AnyValue[] key = null,
            FileSyntax syntax = null,
            FileSchema schema = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).LoadCSV(rel, data, path, key, syntax, schema);
        }

        public bool LoadJSON(
            string rel,
            string data = null,
            string path = null,
            AnyValue[] key = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).LoadJSON(rel, data, path, key);
        }

        public ICollection<RelKey> ListEdb(string relName = null)
        {
            Client.conn = this;
            return ((DelveClient) Client).ListEdb(relName);
        }

        public ICollection<RelKey> DeleteEdb(string relName)
        {
            Client.conn = this;
            return ((DelveClient) Client).DeleteEdb(relName);
        }

        public bool EnableLibrary(string srcName)
        {
            Client.conn = this;
            return ((DelveClient) Client).EnableLibrary(srcName);
        }

        public ICollection<Relation> Cardinality(string relName = null)
        {
            Client.conn = this;
            return ((DelveClient) Client).Cardinality(relName);
        }

        public ICollection<AbstractProblem> CollectProblems()
        {
            Client.conn = this;
            return ((DelveClient) Client).CollectProblems();
        }

        public bool Configure(
            bool? debug = null,
            bool? debugTrace = null,
            bool? broken = null,
            bool? silent = null,
            bool? abortOnError = null
        )
        {
            Client.conn = this;
            return ((DelveClient) Client).Configure(debug, debugTrace, broken, silent, abortOnError);
        }
    }

    public class ManagementConnection : Connection
    {

        public ManagementConnection(
            string scheme = Connection.DEFAULT_SCHEME,
            string host = Connection.DEFAULT_HOST,
            int port = Connection.DEFAULT_PORT,
            RAIInfra infra = Connection.DEFAULT_INFRA,
            RAIRegion region = Connection.DEFAULT_REGION,
            RAICredentials creds = null,
            bool verifySSL = Connection.DEFAULT_VERIFY_SSL
        )
        {
            Scheme = scheme;
            Host = host;
            Port = port;
            Infra = infra;
            Region = region;
            Creds = creds;
            VerifySSL = verifySSL;

            if(creds == null) this.Creds = RAICredentials.FromFile();

            new DelveClient(this); //to register the connection with a client
        }

        public override string Scheme { get; }
        public override string Host { get; }
        public override int Port { get; }
        public override RAIInfra Infra { get; }
        public override RAIRegion Region { get; }
        public override RAICredentials Creds { get; }
        public override bool VerifySSL { get; }

        public ICollection<ComputeData> ListComputes()
        {
            Client.conn = this;
            return ((DelveCloudClient) Client).ListComputes();
        }

        public CreateComputeResponseProtocol CreateCompute(string computeName, RAIComputeSize size, bool dryRun = false)
        {
            Client.conn = this;
            return ((DelveCloudClient) Client).CreateCompute(computeName, size, dryRun);
        }

        public DeleteComputeResponseProtocol DeleteCompute(string computeName, bool dryRun = false)
        {
            Client.conn = this;
            return ((DelveCloudClient) Client).DeleteCompute(computeName, dryRun);
        }

        public ListUsersResponseProtocol ListUsers()
        {
            Client.conn = this;
            return ((DelveCloudClient) Client).ListUsers();
        }

        public CreateUserResponseProtocol CreateUser(string username, string firstName, string lastName, string email, bool dryRun = false)
        {
            Client.conn = this;
            return ((DelveCloudClient) Client).CreateUser(username, firstName, lastName, email, dryRun);
        }

        public ICollection<DatabaseInfo> ListDatabases()
        {
            Client.conn = this;
            return ((DelveCloudClient) Client).ListDatabases();
        }

        public void RemoveDefaultCompute(string dbname)
        {
            Client.conn = this;
            ((DelveCloudClient) Client).RemoveDefaultCompute(dbname);
        }

    }

    /// <summary>
    /// Connection for working with databases in the rAI Cloud.
    ///
    /// All details required for communicating with the rAI Cloud frontend are offloaded to the
    /// `management_conn`, i.e. `management_conn` knows where and how to connect/authenticate.
    ///
    /// Executing a transaction on the rAI Cloud requires a compute. A compute is where the actual
    /// database processing is taking place. Each database operation has to be directed to a
    /// compute, either implicitly or explicitly. When a compute is specified in `CloudConnetion`,
    /// it will be used for all transactions using this connetion. Otherwise, the default compute
    /// will be picked to fulfill the transaction. A default compute can be set through
    /// `createDatabase` (implicitly) or through `setDefaultCompute` (explicitly).
    /// </summary>
    public class CloudConnection : LocalConnection
    {
        /// <summary>
        /// CloudConnection constructor
        /// </summary>
        /// <param name="conn">The base connection to use `LocalConnection` parameters from.</param>
        /// <param name="infra">Underlying cloud provider (AWS/AZURE)</param>
        /// <param name="region">Region of rAI Cloud deployments</param>
        /// <param name="creds">Credentials for authenticating with rAI Cloud</param>
        /// <param name="verifySSL">Verify SSL configuration</param>
        /// <param name="computeName">Compute to be used for this connection. If not specified, the default compute will be used.</param>
        public CloudConnection(
            Connection conn,
            RAIInfra infra = Connection.DEFAULT_INFRA,
            RAIRegion region = Connection.DEFAULT_REGION,
            RAICredentials creds = null,
            bool verifySSL = Connection.DEFAULT_VERIFY_SSL,
            string computeName = null
        ) : this(conn.DbName, conn.DefaultOpenMode, conn.Scheme, conn.Host, conn.Port, infra, region, creds, verifySSL, computeName)
        {
        }

        /// <summary>
        /// CloudConnection constructor
        /// </summary>
        /// <param name="dbname">database to execute transactions with</param>
        /// <param name="defaultOpenMode">`= TransactionMode.OPEN`: How to open the database `dbname`</param>
        /// <param name="scheme">= `http`: The scheme used for connecting to a running server (e.g., `http`)</param>
        /// <param name="host"> = `127.0.0.1`: The host of a running server.</param>
        /// <param name="port"> = `8010`: The port of a running server.</param>
        /// <param name="infra">Underlying cloud provider (AWS/AZURE)</param>
        /// <param name="region">Region of rAI Cloud deployments</param>
        /// <param name="creds">Credentials for authenticating with rAI Cloud</param>
        /// <param name="verifySSL">Verify SSL configuration</param>
        /// <param name="computeName">Compute to be used for this connection. If not specified, the default compute will be used.</param>
        public CloudConnection(
            string dbname,
            TransactionMode defaultOpenMode = Connection.DEFAULT_OPEN_MODE,
            string scheme = Connection.DEFAULT_SCHEME,
            string host = Connection.DEFAULT_HOST,
            int port = Connection.DEFAULT_PORT,
            RAIInfra infra = Connection.DEFAULT_INFRA,
            RAIRegion region = Connection.DEFAULT_REGION,
            RAICredentials creds = null,
            bool verifySSL = Connection.DEFAULT_VERIFY_SSL,
            string computeName = null
        ) : base(dbname, defaultOpenMode, scheme, host, port)
        {
            this.managementConn = new ManagementConnection(scheme, host, port, infra, region, creds, verifySSL);
            this.ComputeName = computeName;

            new DelveClient(this); //to register the connection with a client
        }

        /// <summary>
        /// CloudConnection constructor
        /// </summary>
        /// <param name="dbname">database to execute transactions with</param>
        /// <param name="managementConn">the management connection instance</param>
        /// <param name="defaultOpenMode">`= TransactionMode.OPEN`: How to open the database `dbname`</param>
        /// <param name="computeName">Compute to be used for this connection. If not specified, the default compute will be used.</param>
        public CloudConnection(
            string dbname,
            ManagementConnection managementConn,
            TransactionMode defaultOpenMode = Connection.DEFAULT_OPEN_MODE,
            string computeName = null
        ) : base(dbname, defaultOpenMode, managementConn.Scheme, managementConn.Host, managementConn.Port)
        {
            this.managementConn = managementConn;
            this.ComputeName = computeName;

            new DelveClient(this); //to register the connection with a client
        }

        private ManagementConnection managementConn { get; }
        public override RAIInfra Infra { get { return managementConn.Infra; } }
        public override RAIRegion Region { get { return managementConn.Region; } }
        public override RAICredentials Creds { get { return managementConn.Creds; } }
        public override bool VerifySSL { get { return managementConn.VerifySSL; } }
        public override string ComputeName { get; }
    }
}
