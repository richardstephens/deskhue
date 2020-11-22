using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DeskHue.Entities;
using Newtonsoft.Json;

namespace DeskHue
{
    class ConfigService
    {
        private static HueConfig config = null;

        private static string configPath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\.deskhue";

        public static HueConfig loadConfig()
        {
            if (config == null)
            {
                try
                {
                    string configString = System.IO.File.ReadAllText(configPath);
                    config = JsonConvert.DeserializeObject<HueConfig>(configString);
                    return config;
                }
                catch (FileNotFoundException ex)
                {
                    config = new HueConfig();
                    return config;
                }
            }
            else
            {
                return config;
            }
        }

        public static void saveConfig(HueConfig configToSave)
        {
            if (configToSave != null)
            {
                config = configToSave;
                System.IO.File.WriteAllText(configPath, JsonConvert.SerializeObject(configToSave));
            }
        }
    }
}
