using System.IO;
using IniParser;
using IniParser.Model;

namespace KanjiSeven
{
    public static class ConfigManager
    {
        public static void Init()
        {
            Save(new Configuration { StorageDir = "KanjiSeven.db3" }, true);
        }

        public static Configuration Current
        {
            get
            {
                var conf = new Configuration();
                var file = new FileIniDataParser();
                var data = file.ReadFile("KanjiSeven.ini");

                conf.StorageDir = data["Configuration"]["StorageDir"];

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
            
            file.WriteFile("KanjiSeven.ini", data);
        }
    }

    public class Configuration
    {
        public string StorageDir { get; set; }
    }
}