using System.Collections.Generic;
using KanjiSeven.Data;
using KanjiSeven.Data.Entities;
using SQLite;

namespace KanjiSeven.Services
{
    public sealed class KanjiService
    {
        public static KanjiService Current { get; } = new KanjiService();
        private readonly LocalContext _context;

        private KanjiService()
        {
            _context = LocalContext.Current;
        }

        public IEnumerable<Kanji> List => _context.Conn.Table<Kanji>().ToList();

        public Kanji Get(int id)
        {
            return _context.Conn.Get<Kanji>(id);
        }

        public Kanji Insert(string kotoba, string furigana, string honyaku)
        {
            var kanji = new Kanji {Kotoba = kotoba, Furigana = furigana, Honyaku = honyaku};
            _context.Conn.Insert(kanji);
            return kanji;
        }
    }
}