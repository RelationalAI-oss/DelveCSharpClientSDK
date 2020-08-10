using System;
using System.Net.Http;

namespace Com.RelationalAI
{
    public class DelveClient : GeneratedDelveClient
    {
        public DelveClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public DelveClient() : this(new HttpClient())
        {
        }
    }
}
