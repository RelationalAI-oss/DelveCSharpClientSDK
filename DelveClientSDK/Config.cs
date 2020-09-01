using System;
using System.IO;
using System.Runtime.InteropServices;
using IniParser;
using IniParser.Model;

namespace Com.RelationalAI
{
    public class Config
    {
        private static FileIniDataParser parser = new FileIniDataParser();
        /*
            dotRaiConfigPath() -> String

        Returns the path of the config file.

        # Returns
        - `String`
        */
        public static string dotRaiConfigPath()
        {
            return Path.Combine(dotRaiDir(), "config");
        }
        public static string dotRaiDir()
        {
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);
            return Path.Combine(home, ".rai");
        }


        /*
            loadDotRaiConfig(path::AbstractString=dotRaiConfigPath()) -> IniData

        Returns the contents of the rAI .ini config file. Currently, this file is assumed to be
        stored at `~/.rai/config`. If no file is found, a `SystemError` is thrown.

        Example `~/.rai/config`:
        ```
        [default]
        region = us-east
        host = azure-dev.relationalai.com
        port = 443
        infra = AZURE
        accessKey = [...]
        privateKeyFilename = default_privatekey.json # pragma: allowlist secret

        [aws]
        region = us-east
        host = 127.0.0.1
        port = 8443
        infra = AWS
        accessKey = [...]
        privateKeyFilename = aws_privatekey.json # pragma: allowlist secret
        ```

        # Arguments
        - `path=dotRaiConfigPath()`: Path to the rAI config file
            (`~/.rai/config` by default)

        # Returns
        - `IniData`: Contents of the config file (.ini format)

        # Throws
        - `SystemError`: If the `path` doesn't exist
        */
        public static IniData loadDotRaiConfig()
        {
            return loadDotRaiConfig(dotRaiConfigPath());
        }
        public static IniData loadDotRaiConfig(string path=null)
        {
            if( path == null ) path = dotRaiConfigPath();
            return parser.ReadFile(path);
        }

        public static void storeDotRaiConfig(IniData ini, string path=null)
        {
            if( path == null ) path = dotRaiConfigPath();
            parser.WriteFile(path, ini);
        }

        public static string getIniValue(
            IniData ini,
            string profile,
            string key,
            string defaultValue="notfound"
        )
        {
            var KeyData = ini[profile].GetKeyData(key);
            return KeyData == null ? defaultValue : KeyData.Value;
        }

        /*
            raiGetInfra(IniData ini, string profile="default") -> string

        Returns the cloud provider used by the rAI service from the config file, associated with
        the `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `string`

        # Throws
        - `KeyError`: If the `profile` doesn't exist
        */
        public static string raiGetInfra(IniData ini, string profile="default")
        {
            return getIniValue(ini, profile, "infra", defaultValue:"AWS");
        }


        /*
        raiGetRegion(IniData ini, string profile="default") -> string

        Returns the region of the rAI service from the config file, associated with the
        `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `string`

        # Throws
        - `KeyError`: If the `profile` doesn't exist
        */
        public static string  raiGetRegion(IniData ini, string profile="default")
        {
            return getIniValue(ini, profile, "region", defaultValue:"us-east");
        }


        /*
            raiGetHost(IniData ini, string profile="default") -> String

        Returns the hostname of the rAI service from the config file, associated with the
        `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `String`

        # Throws
        - `KeyError`: If the `profile` doesn't exist
        */
        public static string raiGetHost(IniData ini, string profile="default")
        {
            return getIniValue(ini, profile, "host", defaultValue:"aws.relationalai.com");
        }


        /*
            raiGetPort(IniData ini, string profile="default") -> int

        Returns the port of the rAI service from the config file, associated with the `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `int`

        # Throws
        - `KeyError`: If the `profile` doesn't exist
        */
        public static int raiGetPort(IniData ini, string profile="default")
        {
            return int.Parse(getIniValue(ini, profile, "port", defaultValue:"443"));
        }

        /*
            raiSetInfra(
                IniData ini,
                string infra,
                string profile="default"
            ) -> void

        Sets the cloud provider used by the rAI service to `infra`, for the specific `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string infra`: The cloud provider to set
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `void`
        */
        public static void raiSetInfra(
            IniData ini,
            string infra,
            string profile="default"
        )
        {
            ini[profile]["infra"] = infra;
            return;
        }

        /*
        raiSetRegion(
                IniData ini,
                string region,
                string profile="default"
            ) -> void

        Sets the region key to `region`, for the specific `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string region`: The region to set
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `void`
        */
        public static void raiSetRegion(
            IniData ini,
            string region,
            string profile="default"
        )
        {
            ini[profile]["region"] = region;
            return;
        }

        /*
            raiSetAccessKey(
                IniData ini,
                string accessKey,
                string profile="default"
            ) -> void

        Sets the access-key key to `access_key`, for the specific `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string accessKey`: The access-key value to set
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `void`
        */
        public static void raiSetAccessKey(
            IniData ini,
            string accessKey,
            string profile="default"
        )
        {
            ini[profile]["access_key"] = accessKey;
            return;
        }

        /*
            raiSetPrivateKeyFilename(
                IniData ini,
                string privateKeyFilename,
                string profile="default"
            ) -> void

        Sets the private-key's filename to `privateKeyFilename`, for the specific `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string privateKeyFilename`: The private-key filename value to set
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `void`
        */
        public static void raiSetPrivateKeyFilename(
            IniData ini,
            string privateKeyFilename,
            string profile="default"
        )
        {
            ini[profile]["privateKeyFilename"] = privateKeyFilename;

            return;
        }

    }
}
