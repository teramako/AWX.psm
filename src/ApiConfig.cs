using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnsibleTower
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
        /// <summary>
        /// The URL of AnsibleTower or AWX.<br/>
        /// Should be `<c>scheme</c>://<c>domain</c>[:<c>port</c>]`
        /// </summary>
        [JsonPropertyName("origin")]
        public Uri Origin{ get; private set; }
        /// <summary>
        /// Personal Access Token for OAuth.<br/>
        /// This token is assigned to the <c>Authorization</c> HTTP request header
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; private set; }
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
            JsonSerializer.Serialize(fs, this);
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
                throw new FileNotFoundException($"Ansible Config is not found: {fileInfo.FullName}");
            }
            using var fs = fileInfo.OpenRead();
            var config = JsonSerializer.Deserialize<ApiConfig>(fs)
                ?? throw new Exception($"Could not load config.");
            config.File = fileInfo;
            _instance = config;
            return config;
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
        /// <list type="bullet">Windows: <c>%USEFPROIFLE%\.ansible_psm_config.json</c></list>
        /// <list type="bullet">Linux, MacOS: <c>$HOME/.ansible_psm_config.json</c></list>
        /// </summary>
        public static string DefaultConfigPath
        {
            get
            {
                string fileName = ".ansible_psm_config.json";
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Unix:
                    case PlatformID.MacOSX:
                        return Path.Join(Environment.GetEnvironmentVariable("HOME") ?? string.Empty, fileName);
                    case PlatformID.Win32NT:
                        return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), fileName);
                    default:
                        throw new NotSupportedException($"Not supported: {Environment.OSVersion.Platform}");

                }
            }
        }
    }
}
