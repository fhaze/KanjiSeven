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
            Conn.CreateTable<Kotoba>();

            if (Conn.Table<Kotoba>().Any()) return;

            // bootstrap init
            var kotobaList = new[]
            {
                new Kotoba{ Namae = "水", Furigana = "みず", Romaji = "mizu", Honyaku = "water" },
                new Kotoba{ Namae = "山", Furigana = "やま", Romaji = "yama", Honyaku = "mountain; hill" },
                new Kotoba{ Namae = "火", Furigana = "ひ", Romaji = "hi", Honyaku = "fire; flame; blaze" },
                new Kotoba{ Namae = "横断歩道", Furigana = "おうだんほどう", Romaji = "oudanhodou", Honyaku = "crosswalk" },
                new Kotoba{ Namae = "中学生", Furigana = "ちゅうがくせい", Romaji = "chugakusei", Honyaku = "junior high school student; middle school pupil" },
                new Kotoba{ Namae = "学生", Furigana = "がくせい", Romaji = "gakusei", Honyaku = "student" },
                new Kotoba{ Namae = "学校", Furigana = "がっこう", Romaji = "gakkou", Honyaku = "school" }
            };
            Conn.InsertAll(kotobaList);
        }
    }
}