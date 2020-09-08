using System;

namespace Com.RelationalAI
{
    public abstract class Connection
    {
        public const string DEFAULT_SCHEME = "http";
        public const string DEFAULT_HOST = "127.0.0.1";
        public const int DEFAULT_PORT = 8010;
        public const TransactionMode DEFAULT_OPEN_MODE = TransactionMode.OPEN;
        public const RAIInfra DEFAULT_INFRA = RAIInfra.AZURE;
        public const RAIRegion DEFAULT_REGION = RAIRegion.US_EAST;
        public const bool DEFAULT_VERIFY_SSL = true;

        public virtual string DbName => throw new NotImplementedException();

        public virtual TransactionMode DefaultOpenMode => throw new NotImplementedException();

        public virtual string Scheme => throw new NotImplementedException();

        public virtual string Host => throw new NotImplementedException();

        public virtual int Port => throw new NotImplementedException();

        public virtual RAIInfra Infra => throw new NotImplementedException();

        public virtual RAIRegion Region => throw new NotImplementedException();

        public virtual RAICredentials Creds => throw new NotImplementedException();

        public virtual bool VerifySSL => throw new NotImplementedException();

        public virtual string ComputeName => throw new NotImplementedException();

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
            int port = DEFAULT_PORT)
        {
            this.DbName = dbname;
            this.DefaultOpenMode = defaultOpenMode;
            this.Scheme = scheme;
            this.Host = host;
            this.Port = port;
        }

        public override string DbName { get; }
        public override TransactionMode DefaultOpenMode { get; }
        public override string Scheme { get; }
        public override string Host { get; }
        public override int Port { get; }
    }

    public class ManagementConnection : Connection
    {

        public ManagementConnection(string scheme, string host, int port, RAIInfra infra, RAIRegion region, RAICredentials creds, bool verifySSL)
        {
            Scheme = scheme;
            Host = host;
            Port = port;
            Infra = infra;
            Region = region;
            Creds = creds;
            VerifySSL = verifySSL;
        }

        public override string Scheme { get; }
        public override string Host { get; }
        public override int Port { get; }
        public override RAIInfra Infra { get; }
        public override RAIRegion Region { get; }
        public override RAICredentials Creds { get; }
        public override bool VerifySSL { get; }
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
        )
        {
            this.DbName = dbname;
            this.DefaultOpenMode = defaultOpenMode;
            this.managementConn = new ManagementConnection(scheme, host, port, infra, region, creds, verifySSL);
            this.ComputeName = computeName;
        }

        public override string DbName { get; }
        public override TransactionMode DefaultOpenMode { get; }
        private ManagementConnection managementConn { get; }
        public override string Scheme { get { return managementConn.Scheme; } }
        public override string Host { get { return managementConn.Host; } }
        public override int Port { get { return managementConn.Port; } }
        public override RAIInfra Infra { get { return managementConn.Infra; } }
        public override RAIRegion Region { get { return managementConn.Region; } }
        public override RAICredentials Creds { get { return managementConn.Creds; } }
        public override bool VerifySSL { get { return managementConn.VerifySSL; } }
        public override string ComputeName { get; }
    }
}
