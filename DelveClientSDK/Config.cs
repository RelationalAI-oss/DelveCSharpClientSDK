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
            dot_rai_config_path() -> String

        Returns the path of the config file.

        # Returns
        - `String`
        */
        public static string dot_rai_config_path()
        {
            return Path.Combine(dot_rai_dir(), "config");
        }
        public static string dot_rai_dir()
        {
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);
            return Path.Combine(home, ".rai");
        }


        /*
            load_dot_rai_config(path::AbstractString=dot_rai_config_path()) -> Inifile

        Returns the contents of the rAI .ini config file. Currently, this file is assumed to be
        stored at `~/.rai/config`. If no file is found, a `SystemError` is thrown.

        Example `~/.rai/config`:
        ```
        [default]
        region = us-east
        host = azure-dev.relationalai.com
        port = 443
        infra = AZURE
        access_key = [...]
        private_key_filename = default_privatekey.json # pragma: allowlist secret

        [aws]
        region = us-east
        host = 127.0.0.1
        port = 8443
        infra = AWS
        access_key = [...]
        private_key_filename = aws_privatekey.json # pragma: allowlist secret
        ```

        # Arguments
        - `path::AbstractString=dot_rai_config_path()`: Path to the rAI config file
            (`~/.rai/config` by default)

        # Returns
        - `Inifile`: Contents of the config file (.ini format)

        # Throws
        - `SystemError`: If the `path` doesn't exist
        */
        public static IniData load_dot_rai_config()
        {
            return load_dot_rai_config(dot_rai_config_path());
        }
        public static IniData load_dot_rai_config(string path=null)
        {
            if( path == null ) path = dot_rai_config_path();
            return parser.ReadFile(path);
        }

        public static void store_dot_rai_config(IniData ini, string path=null)
        {
            if( path == null ) path = dot_rai_config_path();
            parser.WriteFile(path, ini);
        }

        private static string _get_ini_value(
            IniData ini,
            string profile,
            string key,
            string default_value="notfound"
        )
        {
            var KeyData = ini[profile].GetKeyData(key);
            return KeyData == null ? default_value : KeyData.Value;
        }

        /*
            rai_get_infra(IniData ini, string profile="default") -> Symbol

        Returns the cloud provider used by the rAI service from the config file, associated with
        the `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `Symbol`

        # Throws
        - `KeyError`: If the `profile` doesn't exist
        */
        public static string rai_get_infra(IniData ini, string profile="default")
        {
            return _get_ini_value(ini, profile, "infra", default_value:"AWS");
        }


        /*
        rai_get_region(IniData ini, string profile="default") -> Symbol

        Returns the region of the rAI service from the config file, associated with the
        `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `Symbol`

        # Throws
        - `KeyError`: If the `profile` doesn't exist
        */
        public static string  rai_get_region(IniData ini, string profile="default")
        {
            return _get_ini_value(ini, profile, "region", default_value:"us-east");
        }


        /*
            rai_get_host(IniData ini, string profile="default") -> String

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
        public static string rai_get_host(IniData ini, string profile="default")
        {
            string host = _get_ini_value(ini, profile, "host", default_value:"aws.relationalai.com");

            return host;
        }


        /*
            rai_get_port(IniData ini, string profile="default") -> Int

        Returns the port of the rAI service from the config file, associated with the `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `Int`

        # Throws
        - `KeyError`: If the `profile` doesn't exist
        */
        public static int rai_get_port(IniData ini, string profile="default")
        {
            int port = int.Parse(_get_ini_value(ini, profile, "port", default_value:"443"));

            return port;
        }

        /*
            rai_set_infra(
                IniData ini,
                string infra,
                string profile="default"
            ) -> Nothing

        Sets the cloud provider used by the rAI service to `infra`, for the specific `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string infra`: The cloud provider to set
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `Nothing`
        */
        public static void rai_set_infra(
            IniData ini,
            string infra,
            string profile="default"
        )
        {
            ini[profile]["infra"] = infra;
        }

        /*
        rai_set_region(
                IniData ini,
                string region,
                string profile="default"
            ) -> Nothing

        Sets the region key to `region`, for the specific `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string region`: The region to set
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `Nothing`
        */
        public static void rai_set_region(
            IniData ini,
            string region,
            string profile="default"
        )
        {
            ini[profile]["region"] = region;

            return;
        }

        /*
            rai_set_access_key(
                IniData ini,
                access_string key,
                string profile="default"
            ) -> Nothing

        Sets the access-key key to `access_key`, for the specific `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `access_string key`: The access-key value to set
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `Nothing`
        */
        public static void rai_set_access_key(
            IniData ini,
            string access_key,
            string profile="default"
        )
        {
            ini[profile]["access_key"] = access_key;
        }

        /*
            rai_set_private_key_filename(
                IniData ini,
                string private_key_filename,
                string profile="default"
            ) -> Nothing

        Sets the private-key's filename to `private_key_filename`, for the specific `[profile]`.

        # Arguments
        - `IniData ini`: The contents of the config file (.ini format)
        - `string private_key_filename`: The private-key filename value to set
        - `string profile="default"`: The [profile] to look for

        # Returns
        - `Nothing`
        */
        public static void rai_set_private_key_filename(
            IniData ini,
            string private_key_filename,
            string profile="default"
        )
        {
            ini[profile]["private_key_filename"] = private_key_filename;

            return;
        }

    }
}
