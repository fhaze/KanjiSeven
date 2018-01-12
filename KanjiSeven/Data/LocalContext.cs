using System;
using System.IO;
using System.Linq;
using GLib;
using KanjiSeven.Data.Entities;
using SQLite;

namespace KanjiSeven.Data
{
    public sealed class LocalContext
    {
        public static LocalContext Current { get; } = new LocalContext();
        public SQLiteConnection Conn { get; private set; }
        
        private LocalContext()
        {
            Reload();
        }

        public void Reload()
        {
            Conn?.Close();

            var path = ConfigManager.Current.StorageDir;
            Conn = new SQLiteConnection(path);
            
            CreateTables();
        }
        
        public void CreateTables()
        {
            Conn.CreateTable<Tango>();

            if (Conn.Table<Tango>().Any()) return;

            // bootstrap init
            var tangoList = new[]
            {
                new Tango{ Namae = "水", Furigana = "みず", Romaji = "mizu", Honyaku = "water" },
                new Tango{ Namae = "山", Furigana = "やま", Romaji = "yama", Honyaku = "mountain; hill" },
                new Tango{ Namae = "火", Furigana = "ひ", Romaji = "hi", Honyaku = "fire; flame; blaze" },
                new Tango{ Namae = "横断歩道", Furigana = "おうだんほどう", Romaji = "oudanhodou", Honyaku = "crosswalk" },
                new Tango{ Namae = "中学生", Furigana = "ちゅうがくせい", Romaji = "chugakusei", Honyaku = "junior high school student; middle school pupil" },
                new Tango{ Namae = "学生", Furigana = "がくせい", Romaji = "gakusei", Honyaku = "student" },
                new Tango{ Namae = "学校", Furigana = "がっこう", Romaji = "gakkou", Honyaku = "school" }
            };
            Conn.InsertAll(tangoList);
        }
    }
}