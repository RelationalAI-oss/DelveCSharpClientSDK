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
            this.dbname = dbname;
            this.defaultOpenMode = defaultOpenMode;
            this.scheme = scheme;
            this.host = host;
            this.port = port;
        }

        public string dbname { get; }
        public TransactionMode defaultOpenMode { get; }
        public string scheme { get; }
        public string host { get; }
        public int port { get; }
        public virtual RAIInfra infra { get{ return DEFAULT_INFRA; } }
        public virtual RAIRegion region { get{ return DEFAULT_REGION; } }
        public virtual RAICredentials creds { get{ return null; } }
        public virtual bool verifySSL { get{ return DEFAULT_VERIFY_SSL; } }
        public virtual string computeName { get{ return null; } }

        public Uri baseUrl {
            get { return new UriBuilder(this.scheme, this.host, this.port).Uri; }
        }
    }

    public class CloudConnection : Connection
    {
        public override RAIInfra infra { get; }
        public override RAIRegion region { get; }
        public override RAICredentials creds { get; }
        public override bool verifySSL { get; }
        public override string computeName { get; }

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
            this.infra = infra;
            this.region = region;
            this.creds = creds;
            this.verifySSL = verifySSL;
            this.computeName = computeName;
        }
        public CloudConnection(
            Connection conn,
            RAIInfra infra = DEFAULT_INFRA,
            RAIRegion region = DEFAULT_REGION,
            RAICredentials creds = null,
            bool verifySSL = DEFAULT_VERIFY_SSL,
            string computeName = null
        ) : this(conn.dbname, conn.defaultOpenMode, conn.scheme, conn.host, conn.port, infra, region, creds, verifySSL, computeName)
        {
        }
    }
}
