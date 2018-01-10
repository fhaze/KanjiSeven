using System;
using System.IO;
using IniParser;
using IniParser.Model;

namespace KanjiSeven
{
    public static class ConfigManager
    {
        public static void Init()
        {
            Save(new Configuration { StorageDir = "KanjiSeven.db3", HintSpeed = 3 }, true);
        }

        public static Configuration Current
        {
            get
            {
                Console.WriteLine("read");
                
                var conf = new Configuration();
                var file = new FileIniDataParser();
                var data = file.ReadFile("KanjiSeven.ini");

                conf.StorageDir = data["Configuration"]["StorageDir"];
                conf.HintSpeed = Convert.ToInt32(data["Configuration"]["HintSpeed"]);

                return conf;
            }
        }

        public static void Save(Configuration configuration, bool conserve)
        {
            if (File.Exists("KanjiSeven.ini") && conserve) return;
            
            var file = new FileIniDataParser();
            var data = new IniData();

            data.Sections.AddSection("Configuration");
            data["Configuration"]["StorageDir"] = configuration.StorageDir;
            data["Configuration"]["HintSpeed"] = configuration.HintSpeed.ToString();
            
            file.WriteFile("KanjiSeven.ini", data);
        }
    }

    public class Configuration
    {
        public string StorageDir { get; set; }
        public int HintSpeed { get; set; }
    }
}