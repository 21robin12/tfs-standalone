namespace TfsStandalone.UI.Infrastructure
{
    using System;
    using System.IO;
    using System.Linq;
    using Config;
    using Newtonsoft.Json;

    public static class ConfigManager
    {
        private static TfsStandaloneConfig _config;

        public static TfsStandaloneConfig Config
        {
            get
            {

                if (_config == null)
                {
                    var configPath = ConfigPath();
                    if (!File.Exists(configPath))
                    {
                        throw new FileNotFoundException($"No config file found at {configPath}");
                    }

                    var text = File.ReadAllText(configPath);
                    _config = JsonConvert.DeserializeObject<TfsStandaloneConfig>(text);
                }

                return _config;
            }
        }

        public static TfsProject Project(string projectId)
        {
            return Config.ProjectCollections.SelectMany(x => x.Projects).FirstOrDefault(x => x.Id == projectId);
        }

        public static TfsProjectCollection ProjectCollection(int projectCollectionIndex)
        {
            return Config.ProjectCollections.ElementAt(projectCollectionIndex);
        }

        public static TfsProjectCollection ProjectCollection(TfsProject project)
        {
            return Config.ProjectCollections.FirstOrDefault(x => x.Projects.Any(y => y.Id == project.Id));
        }

        public static void SaveConfig(TfsStandaloneConfig config)
        {
            var text = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigPath(), text);
        }

        private static string ConfigPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        }
    }
}
