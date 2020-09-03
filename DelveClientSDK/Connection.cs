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

    public class CloudConnection : Connection
    {
        public override RAIInfra Infra { get; }
        public override RAIRegion Region { get; }
        public override RAICredentials Creds { get; }
        public override bool VerifySSL { get; }
        public override string ComputeName { get; }

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
            this.Creds = creds;
            this.VerifySSL = verifySSL;
            this.ComputeName = computeName;
        }
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
    }
}
