using System;
using System.IO;
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
            CreateTables();
        }

        private void CreateTables()
        {
            Conn.CreateTable<Kanji>();
        }
    }
}