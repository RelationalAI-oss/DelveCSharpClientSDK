namespace Com.RelationalAI
{
    public class Connection
    {
        public const TransactionMode DEFAULT_OPEN_MODE = TransactionMode.OPEN;
        public const string DEFAULT_HOST = "127.0.0.1";
        public const int DEFAULT_PORT = 8010;
        public const RAIInfra DEFAULT_INFRA = RAIInfra.AZURE;
        public const RAIRegion DEFAULT_REGION = RAIRegion.US_EAST;
        public const bool DEFAULT_VERIFY_SSL = true;

        public Connection(
            string dbname,
            TransactionMode defaultOpenMode = DEFAULT_OPEN_MODE,
            string host = DEFAULT_HOST,
            int port = DEFAULT_PORT,
            RAIInfra infra = DEFAULT_INFRA,
            RAIRegion region = DEFAULT_REGION,
            RAICredentials creds = null,
            bool verifySSL = DEFAULT_VERIFY_SSL)
        {
            this.dbname = dbname;
            this.defaultOpenMode = defaultOpenMode;
            this.host = host;
            this.port = port;
            this.infra = infra;
            this.region = region;
            this.creds = creds;
            this.verifySSL = verifySSL;
        }

        public string dbname { get; }
        public TransactionMode defaultOpenMode { get; }
        public string host { get; }
        public int port { get; }
        public RAIInfra infra { get; }
        public RAIRegion region { get; }
        public RAICredentials creds { get; }
        public bool verifySSL { get; }
    }
}
