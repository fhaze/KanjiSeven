using System;
using System.IO;
using IniParser;
using IniParser.Model;
using KanjiSeven.Models;

namespace KanjiSeven
{
    public static class ConfigManager
    {
        public static void Init()
        {
            Save(new Configuration
            {
                StorageDir = "KanjiSeven.db3",
                ShowHint = true,
                HintSpeed = 10,
                GameMode = GameMode.Simple,
                QuestionType = TangoType.Kanji,
                AnswerType = TangoType.Honyaku
            }, true);
        }

        public static Configuration Current
        {
            get
            {
                var conf = new Configuration();
                var file = new FileIniDataParser();
                var data = file.ReadFile("KanjiSeven.ini");

                conf.StorageDir = data["Configuration"]["StorageDir"];
                
                if (bool.TryParse(data["Configuration"]["ShowHint"], out var showHint))
                    conf.ShowHint = showHint; 
                        
                conf.HintSpeed = Convert.ToInt32(data["Configuration"]["HintSpeed"]);
                
                if (Enum.TryParse<GameMode>(data["Configuration"]["GameStyle"], out var gameStyle))
                    conf.GameMode = gameStyle;
                else
                    conf.GameMode = GameMode.Simple;

                if (Enum.TryParse<TangoType>(data["Configuration"]["QuestionType"], out var questionType))
                    conf.QuestionType = questionType;
                else
                    conf.QuestionType = TangoType.Kanji;

                if (Enum.TryParse<TangoType>(data["Configuration"]["AnswerType"], out var answerType))
                    conf.AnswerType = answerType;
                else
                    conf.AnswerType = TangoType.Honyaku;
                
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
            data["Configuration"]["ShowHint"] = configuration.ShowHint.ToString();
            data["Configuration"]["HintSpeed"] = configuration.HintSpeed.ToString();
            data["Configuration"]["GameStyle"] = configuration.GameMode.ToString();
            data["Configuration"]["QuestionType"] = configuration.QuestionType.ToString();
            data["Configuration"]["AnswerType"] = configuration.AnswerType.ToString();
            
            file.WriteFile("KanjiSeven.ini", data);
        }
    }
}