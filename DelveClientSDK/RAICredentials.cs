using System;
namespace Com.RelationalAI
{

    public class RAICredentials
    {
        public string accessKey { get; }
        public string privateKey { get; } // pragma: allowlist secret

        public RAICredentials(string accessKey, string privateKey)
        {
            this.accessKey = accessKey;
            this.privateKey = privateKey; // pragma: allowlist secret
        }
    }
}
