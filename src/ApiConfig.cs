using System.Management.Automation;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AWX
{
    public class ApiConfig
    {

        [JsonConstructor]
        public ApiConfig(Uri origin, string token, DateTime? lastSaved) : this(origin, token)
        {
            LastSaved = lastSaved;
        }
        public ApiConfig(Uri uri, string token)
        {
            Origin = new Uri($"{uri.Scheme}://{uri.Authority}");
            Token = token;
        }
        public ApiConfig()
        {
        }
        /// <summary>
        /// The URL of AWX.<br/>
        /// Should be `<c>scheme</c>://<c>domain</c>[:<c>port</c>]`
        /// </summary>
        [JsonPropertyName("origin")]
        public Uri Origin { get; private set; } = new Uri("http://localhost");
        /// <summary>
        /// Personal Access Token for OAuth.<br/>
        /// This token is assigned to the <c>Authorization</c> HTTP request header
        /// </summary>
        [JsonPropertyName("token")]
        [Hidden]
        public string Token { get; private set; } = string.Empty;
        [JsonPropertyName("last_saved")]
        public DateTime? LastSaved { get; private set; }
        /// <summary>
        /// Save to or Loaded file
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public FileInfo? File { get; set; }


        private static ApiConfig? _instance = null;

        /// <summary>
        /// Current config
        /// </summary>
        public static ApiConfig Instance
        {
            get
            {
                if (_instance != null) return _instance;
                return Load();
            }
        }
        /// <summary>
        /// Save the config to <paramref name="fileInfo"/> or
        /// default config path (<see cref="DefaultConfigPath"/>)
        /// </summary>
        /// <param name="fileInfo"></param>
        public void Save(FileInfo? fileInfo = null)
        {
            if (fileInfo == null)
            {
                File ??= new FileInfo(DefaultConfigPath);
                fileInfo = File;
            }
            else
            {
                File = fileInfo;
            }
            LastSaved = DateTime.UtcNow;
            using var fs = fileInfo.OpenWrite();
            JsonSerializer.Serialize(fs, this, Json.DeserializeOptions);
        }
        public static ApiConfig Load(ApiConfig config)
        {
            _instance = config;
            return config;
        }
        /// <summary>
        /// Load config from specified file
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns><see cref="ApiConfig"/></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public static ApiConfig Load(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return Load(new ApiConfig());
            }
            using var fs = fileInfo.OpenRead();
            var config = JsonSerializer.Deserialize<ApiConfig>(fs, Json.DeserializeOptions)
                ?? throw new Exception($"Could not load config.");
            config.File = fileInfo;
            return Load(config);
        }
        /// <summary>
        /// Load config file from default file (<see cref="DefaultConfigPath"/>)
        /// </summary>
        /// <returns><see cref="ApiConfig"/></returns>
        public static ApiConfig Load()
        {
            return Load(new FileInfo(DefaultConfigPath));
        }
        /// <summary>
        /// Default config file
        /// <list type="bullet">
        /// <item><term>Windows</term><description><c>%USEFPROIFLE%\.ansible_psm_config.json</c></description></item>
        /// <item><term>Linux, MacOS</term><description><c>$HOME/.ansible_psm_config.json</c></description></item>
        /// </list>
        /// </summary>
        public static string DefaultConfigPath
        {
            get
            {
                var envPath = Environment.GetEnvironmentVariable(ENV_CONFIG);
                if (!string.IsNullOrEmpty(envPath) && System.IO.File.Exists(envPath))
                {
                    return envPath;
                }
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Unix:
                    case PlatformID.MacOSX:
                        return Path.Join(Environment.GetEnvironmentVariable("HOME") ?? string.Empty, DEFULT_CONFIG_NAME);
                    case PlatformID.Win32NT:
                        return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), DEFULT_CONFIG_NAME);
                    default:
                        throw new NotSupportedException($"Not supported: {Environment.OSVersion.Platform}");

                }
            }
        }
        const string ENV_CONFIG = "ANSIBLE_API_CONFIG";
        const string DEFULT_CONFIG_NAME = ".ansible_api_config.json";
    }
}
