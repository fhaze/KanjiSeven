using System.Collections.Generic;
using KanjiSeven.Data;
using KanjiSeven.Data.Entities;
using SQLite;

namespace KanjiSeven.Services
{
    public sealed class KotobaService
    {
        public static KotobaService Current { get; } = new KotobaService();
        private readonly LocalContext _context;

        private KotobaService()
        {
            _context = LocalContext.Current;
        }

        public IEnumerable<Kotoba> List => _context.Conn.Table<Kotoba>().ToList();

        public Kotoba Get(int id)
        {
            return _context.Conn.Get<Kotoba>(id);
        }

        public Kotoba Insert(string namae, string furigana, string honyaku)
        {
            var kanji = new Kotoba {Namae = namae, Furigana = furigana, Honyaku = honyaku};
            _context.Conn.Insert(kanji);
            return kanji;
        }
    }
}