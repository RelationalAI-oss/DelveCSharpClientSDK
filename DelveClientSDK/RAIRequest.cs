using System;
using System.IO;
using System.Net.Http;
using System.ComponentModel;
using System.Reflection;
using NSec.Cryptography;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Com.RelationalAI
{
    public enum RAIInfra {
        AWS,
        AZURE,
    }

    public enum RAIRegion {
        [Description("us-east")]
        US_EAST,
    }

    public class RAIRequest {
        public const string DEFAULT_SERVICE = "transaction";

        public RAICredentials creds { get; }
        // verb, url, headers, content
        public HttpRequestMessage inner_req { get; }
        public RAIRegion region { get; }
        public string service { get; }
        public bool return_stream { get; }
        public BufferedStream response_stream { get; }

        public RAIRequest(
            HttpRequestMessage inner_req,
            Connection conn,
            string service = DEFAULT_SERVICE,
            bool return_stream = true
        ): this(inner_req, conn.creds, conn.region, service, return_stream)
        {
        }

        public RAIRequest(
            HttpRequestMessage inner_req,
            RAICredentials creds = null,
            RAIRegion region = Connection.DEFAULT_REGION,
            string service = DEFAULT_SERVICE,
            bool return_stream = true
        )
        {
            this.creds = creds;
            this.inner_req = inner_req;
            this.region = region;
            this.service = service;
            this.return_stream = return_stream;
            this.response_stream = null;
        }

        public void sign(int debug_level=1) {
            sign(DateTime.UtcNow, debug_level);
        }

        public void sign(DateTime t, int debug_level=1) {
            // ISO8601 date/time strings for time of request
            string date = String.Format("{0:yyyyMMdd}", t);

            // Authentication scope
            string scope = string.Join("/", new string[]{
                date, EnumString.GetDescription(this.region), this.service, "rai01_request"
            });

            // SHA256 hash of content
            Sha256 shaw256HashAlgo = new Sha256();
            byte[] reqContent = inner_req.Content.ReadAsByteArrayAsync().Result;
            byte[] sha256Hash = shaw256HashAlgo.Hash(reqContent);
            string contentHash = sha256Hash.ToHex();

            // HTTP headers
            inner_req.Headers.Authorization = null;

            // Sort and lowercase() Headers to produce canonical form
            string canonical_headers = string.Join(
                "\n",
                from header in inner_req.Headers.Union(inner_req.Content.Headers)
                orderby header.Key.ToLower()
                select string.Format(
                    "{0}:{1}",
                    header.Key.ToLower(),
                    string.Join(",", header.Value).Trim()
                )
            );
            string signed_headers = string.Join(
                ";",
                from header in inner_req.Headers.Union(inner_req.Content.Headers)
                orderby header.Key.ToLower()
                select header.Key.ToLower()
            );

            // Sort Query String
            var parsedQuery = HttpUtility.ParseQueryString(inner_req.RequestUri.Query);
            var parsedQueryDict = parsedQuery.AllKeys.SelectMany(
                parsedQuery.GetValues, (k, v) => new {key = k, value = v}
            );
            string query = string.Join(
                "&",
                from qparam in parsedQueryDict
                orderby qparam.key
                select string.Format(
                    "{0}={1}",
                    HttpUtility.UrlEncode(qparam.key),
                    HttpUtility.UrlEncode(qparam.value))
            );

            // Create hash of canonical request
            string canonical_form = string.Format(
                "{0}\n{1}\n{2}\n{3}\n\n{4}\n{5}",
                inner_req.Method,
                HttpUtility.UrlPathEncode(inner_req.RequestUri.AbsolutePath),
                HttpUtility.UrlEncode(query),
                canonical_headers,
                signed_headers,
                contentHash
            );

            if(debug_level > 2) {
                Console.WriteLine("canonical_form:");
                Console.WriteLine(canonical_form);
                Console.WriteLine();
            }

            sha256Hash = shaw256HashAlgo.Hash(Encoding.UTF8.GetBytes(canonical_form));
            string canonicalHash = sha256Hash.ToHex();

            // Create and sign "String to Sign"
            string string_to_sign = string.Format(
                "RAI01-ED25519-SHA256\n{0}\n{1}", scope, canonicalHash
            );

            byte[] seed = Convert.FromBase64String(creds.private_key);

            // select the Ed25519 signature algorithm
            var algorithm = SignatureAlgorithm.Ed25519;

            // create a new key pair
            using var key = Key.Import(algorithm, seed, KeyBlobFormat.RawPrivateKey);
            // using var key = Key.Create(algorithm);

            // sign the data using the private key
            byte[] signature = algorithm.Sign(key, Encoding.UTF8.GetBytes(string_to_sign));

            string sig = signature.ToHex();

            if(debug_level > 2) {
                Console.WriteLine("string_to_sign:");
                Console.WriteLine(string_to_sign);
                Console.WriteLine();
                Console.WriteLine("signature:");
                Console.WriteLine(sig);
                Console.WriteLine();
            }

            var authHeader = string.Format(
                "RAI01-ED25519-SHA256 Credential={0}/{1}, SignedHeaders={2}, Signature={3}",
                creds.access_key,
                scope,
                signed_headers,
                sig
            );

            inner_req.Headers.TryAddWithoutValidation("Authorization", authHeader);
        }
    }
}
