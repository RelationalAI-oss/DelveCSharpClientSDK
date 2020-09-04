using System;

namespace Com.RelationalAI
{
    public class Connection
    {
        public const TransactionMode DEFAULT_OPEN_MODE = TransactionMode.OPEN;
        public const string DEFAULT_SCHEME = "http";
        public const string DEFAULT_HOST = "127.0.0.1";
        public const int DEFAULT_PORT = 8010;
        public const RAIInfra DEFAULT_INFRA = RAIInfra.AZURE;
        public const RAIRegion DEFAULT_REGION = RAIRegion.US_EAST;
        public const bool DEFAULT_VERIFY_SSL = true;

        public Connection(
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
        }

        public string DbName { get; }
        public TransactionMode DefaultOpenMode { get; }
        public string Scheme { get; }
        public string Host { get; }
        public int Port { get; }
        public virtual RAIInfra Infra { get{ return DEFAULT_INFRA; } }
        public virtual RAIRegion Region { get{ return DEFAULT_REGION; } }
        public virtual RAICredentials Creds { get{ return null; } }
        public virtual bool VerifySSL { get{ return DEFAULT_VERIFY_SSL; } }
        public virtual string ComputeName { get{ return null; } }

        public Uri BaseUrl {
            get { return new UriBuilder(this.Scheme, this.Host, this.Port).Uri; }
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
            int port = DEFAULT_PORT) : base(dbname, defaultOpenMode, scheme, host, port)
        {
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
    public class CloudConnection : Connection
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
            RAIInfra infra = DEFAULT_INFRA,
            RAIRegion region = DEFAULT_REGION,
            RAICredentials creds = null,
            bool verifySSL = DEFAULT_VERIFY_SSL,
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
            TransactionMode defaultOpenMode = DEFAULT_OPEN_MODE,
            string scheme = DEFAULT_SCHEME,
            string host = DEFAULT_HOST,
            int port = DEFAULT_PORT,
            RAIInfra infra = DEFAULT_INFRA,
            RAIRegion region = DEFAULT_REGION,
            RAICredentials creds = null,
            bool verifySSL = DEFAULT_VERIFY_SSL,
            string computeName = null
        ) : base(dbname, defaultOpenMode, scheme, host, port)
        {
            this.Infra = infra;
            this.Region = region;
            this.Creds = creds == null ? RAICredentials.FromFile() : creds;
            this.VerifySSL = verifySSL;
            this.ComputeName = computeName;
        }

        public override RAIInfra Infra { get; }
        public override RAIRegion Region { get; }
        public override RAICredentials Creds { get; }
        public override bool VerifySSL { get; }
        public override string ComputeName { get; }
    }
}
