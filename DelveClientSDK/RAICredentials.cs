using System;
namespace Com.RelationalAI
{

    public class RAICredentials
    {
        public string access_key { get; }
        public string private_key { get; } // pragma: allowlist secret

        public RAICredentials(string access_key, string private_key)
        {
            this.access_key = access_key;
            this.private_key = private_key; // pragma: allowlist secret
        }
    }
}
