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
        public HttpRequestMessage innerReq { get; }
        public RAIRegion region { get; }
        public string service { get; }

        public RAIRequest(
            HttpRequestMessage innerReq,
            Connection conn,
            string service = DEFAULT_SERVICE
        ): this(innerReq, conn.creds, conn.region, service)
        {
        }

        public RAIRequest(
            HttpRequestMessage innerReq,
            RAICredentials creds = null,
            RAIRegion region = Connection.DEFAULT_REGION,
            string service = DEFAULT_SERVICE
        )
        {
            this.creds = creds;
            this.innerReq = innerReq;
            this.region = region;
            this.service = service;
        }

        public void sign(int debugLevel=1) {
            sign(DateTime.UtcNow, debugLevel);
        }

        public void sign(DateTime t, int debugLevel=1) {
            if(this.creds == null) return;

            // ISO8601 date/time strings for time of request
            string date = String.Format("{0:yyyyMMdd}", t);

            // Authentication scope
            string scope = string.Join("/", new string[]{
                date, EnumString.GetDescription(this.region), this.service, "rai01_request"
            });

            // SHA256 hash of content
            Sha256 shaw256HashAlgo = new Sha256();
            byte[] reqContent = innerReq.Content.ReadAsByteArrayAsync().Result;
            byte[] sha256Hash = shaw256HashAlgo.Hash(reqContent);
            string contentHash = sha256Hash.ToHex();

            // HTTP headers
            innerReq.Headers.Authorization = null;

            var allHeaders = innerReq.Headers.Union(innerReq.Content.Headers);

            // Sort and lowercase() Headers to produce canonical form
            string canonicalHeaders = string.Join(
                "\n",
                from header in allHeaders
                orderby header.Key.ToLower()
                select string.Format(
                    "{0}:{1}",
                    header.Key.ToLower(),
                    string.Join(",", header.Value).Trim()
                )
            );
            string signedHeaders = string.Join(
                ";",
                from header in allHeaders
                orderby header.Key.ToLower()
                select header.Key.ToLower()
            );

            // Sort Query String
            var parsedQuery = HttpUtility.ParseQueryString(innerReq.RequestUri.Query);
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
            string canonicalForm = string.Format(
                "{0}\n{1}\n{2}\n{3}\n\n{4}\n{5}",
                innerReq.Method,
                HttpUtility.UrlPathEncode(innerReq.RequestUri.AbsolutePath),
                query,
                canonicalHeaders,
                signedHeaders,
                contentHash
            );

            if(debugLevel > 2) {
                Console.WriteLine("canonical_form:");
                Console.WriteLine(canonicalForm);
                Console.WriteLine();
            }

            sha256Hash = shaw256HashAlgo.Hash(Encoding.UTF8.GetBytes(canonicalForm));
            string canonicalHash = sha256Hash.ToHex();

            // Create and sign "String to Sign"
            string stringToSign = string.Format(
                "RAI01-ED25519-SHA256\n{0}\n{1}", scope, canonicalHash
            );

            byte[] seed = Convert.FromBase64String(creds.privateKey);

            // select the Ed25519 signature algorithm
            var algorithm = SignatureAlgorithm.Ed25519;

            // create a new key pair
            using var key = Key.Import(algorithm, seed, KeyBlobFormat.RawPrivateKey);
            // using var key = Key.Create(algorithm);

            // sign the data using the private key
            byte[] signature = algorithm.Sign(key, Encoding.UTF8.GetBytes(stringToSign));

            string sig = signature.ToHex();

            if(debugLevel > 2) {
                Console.WriteLine("string_to_sign:");
                Console.WriteLine(stringToSign);
                Console.WriteLine();
                Console.WriteLine("signature:");
                Console.WriteLine(sig);
                Console.WriteLine();
            }

            var authHeader = string.Format(
                "RAI01-ED25519-SHA256 Credential={0}/{1}, SignedHeaders={2}, Signature={3}",
                creds.accessKey,
                scope,
                signedHeaders,
                sig
            );

            innerReq.Headers.TryAddWithoutValidation("Authorization", authHeader);
        }
    }
}
