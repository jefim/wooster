using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace Wooster.Classes
{
    public class Config
    {
        public Config()
        {
            this.Theme = new Theme();
        }

        public Theme Theme { get; set; }

        public static string GetDefaultConfigPath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = "Wooster";
            var configFileName = "Wooster.config.xml";
            return Path.Combine(appDataPath, appFolder, configFileName);
        }

        public void Save()
        {
            var serializer = new XmlSerializer(typeof(Config));
            using (var writer = XmlTextWriter.Create(GetRealConfigPath(), new XmlWriterSettings { Indent = true }))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static string GetRealConfigPath()
        {
            var userConfigPath = Properties.Settings.Default.ConfigFilePath;
            if (string.IsNullOrWhiteSpace(userConfigPath))
            {
                return GetDefaultConfigPath();
            }
            else
            {
                return userConfigPath;
            }
        }

        /// <summary>
        /// Loads config from all places - first tries the user config path then uses the default config path.
        /// </summary>
        /// <returns></returns>
        public static Config Load()
        {
            Config config = null;
            var defaultConfigPath = GetDefaultConfigPath();

            // Try loading user config path
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ConfigFilePath))
            {
                var userConfigPath = Properties.Settings.Default.ConfigFilePath;
                try
                {
                    config = LoadConfig(userConfigPath);
                }
                catch (Exception ex)
                {
                    // Failed - show user a message.
                    MessageBox.Show(string.Format("Failed to load config from path '{0}'; will use default configuration.\r\n\r\nError message: \r\n{1}", userConfigPath, ex.Message));
                }
            }

            // If still no config - use the default config
            if(config == null) config = new Config();

            return config;
        }

        private static Config LoadConfig(string configPath)
        {
            var config = new Config();
            var serializer = new XmlSerializer(typeof(Config));
            try
            {
                using (var reader = XmlTextReader.Create(configPath))
                {
                    config = (Config)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
            }
            return config;
        }
    }
}
