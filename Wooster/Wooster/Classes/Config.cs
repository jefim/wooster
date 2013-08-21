using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using Wooster.Utils;

namespace Wooster.Classes
{
    public class Config
    {
        public Config()
        {
            this.Theme = new Theme();
            this.AutoSelectFirstAvailableAction = true;
            this.MaxActionsShown = 10;
            this.HotkeyConfig = new HotkeyConfig();
            this.SearchByFirstLettersEnabled = true;
        }

        public bool AutoSelectFirstAvailableAction { get; set; }

        public bool SearchByFirstLettersEnabled { get; set; }

        public int MaxActionsShown { get; set; }

        public HotkeyConfig HotkeyConfig { get; set; }

        public Theme Theme { get; set; }

        private static string GetDefaultConfigPath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = "Wooster";
            var configFileName = "Wooster.config.xml";
            return Path.Combine(appDataPath, appFolder, configFileName);
        }

        public void Save()
        {
            var serializer = new XmlSerializer(typeof(Config));
            var realConfigPath = GetRealConfigPath();

            var dir = Path.GetDirectoryName(realConfigPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using (var writer = XmlTextWriter.Create(realConfigPath, new XmlWriterSettings { Indent = true }))
            {
                serializer.Serialize(writer, this);
            }
        }

        internal static string GetRealConfigPath()
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
#if DEBUG
            return new Config();
#endif
            Config config = null;
            var configPath = GetRealConfigPath();
            try
            {
                config = LoadConfig(configPath);
            }
            catch (Exception ex)
            {
                // Failed - show user a message.
                MessageBox.Show(string.Format("Failed to load config from path '{0}'; will use default configuration.\r\n\r\nError message: \r\n{1}", configPath, ex.Message));
            }

            //var defaultConfigPath = GetDefaultConfigPath();

            //// Try loading user config path
            //if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ConfigFilePath))
            //{
            //    var userConfigPath = Properties.Settings.Default.ConfigFilePath;
            //    try
            //    {
            //        config = LoadConfig(userConfigPath);
            //    }
            //    catch (Exception ex)
            //    {
            //        // Failed - show user a message.
            //        MessageBox.Show(string.Format("Failed to load config from path '{0}'; will use default configuration.\r\n\r\nError message: \r\n{1}", userConfigPath, ex.Message));
            //    }
            //}

            // If still no config - use the default config
            if (config == null)
            {
                config = new Config();
                config.Save();
            }

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

    public class HotkeyConfig
    {
        public HotkeyConfig()
        {
            this.Alt = true;
            this.Control = false;
            this.Shift = false;
            this.Win = false;
            this.Key = Keys.Space;
        }

        public bool Alt { get; set; }

        public bool Control { get; set; }

        public bool Shift { get; set; }

        public bool Win { get; set; }

        public Keys Key { get; set; }
    }
}
