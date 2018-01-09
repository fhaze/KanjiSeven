using System;
using System.IO;
using System.Linq;
using KanjiSeven.Data.Entities;
using SQLite;

namespace KanjiSeven.Data
{
    public sealed class LocalContext
    {
        public static LocalContext Current { get; } = new LocalContext();
        public SQLiteConnection Conn { get; }
        
        private LocalContext()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var file = Path.Combine(path, "KanjiSeven.db3");
            
            Conn = new SQLiteConnection(file);
        }

        public void CreateTables()
        {
            Conn.CreateTable<Kotoba>();

            if (Conn.Table<Kotoba>().Any()) return;

            // bootstrap init
            var kotobaList = new[]
            {
                new Kotoba{ Namae = "水", Furigana = "みず", Honyaku = "water" },
                new Kotoba{ Namae = "山", Furigana = "やま", Honyaku = "mountain; hill" },
                new Kotoba{ Namae = "火", Furigana = "ひ", Honyaku = "fire; flame; blaze" },
                new Kotoba{ Namae = "横断歩道", Furigana = "おうだんほどう", Honyaku = "crosswalk" },
                new Kotoba{ Namae = "中学生", Furigana = "ちゅうがくせい", Honyaku = "junior high school student; middle school pupil" },
                new Kotoba{ Namae = "学生", Furigana = "がくせい", Honyaku = "student" },
                new Kotoba{ Namae = "学校", Furigana = "がっこう", Honyaku = "school" }
            };
            Conn.InsertAll(kotobaList);
        }
    }
}