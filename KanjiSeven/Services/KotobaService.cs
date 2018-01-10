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

        public IList<Kotoba> List => _context.Conn.Table<Kotoba>().ToList();

        public Kotoba Get(int id)
        {
            return _context.Conn.Get<Kotoba>(id);
        }

        public Kotoba Insert(string namae, string furigana, string romaji, string honyaku)
        {
            var kotoba = new Kotoba {Namae = namae, Furigana = furigana, Romaji = romaji, Honyaku = honyaku};
            _context.Conn.Insert(kotoba);
            return kotoba;
        }

        public Kotoba Update(Kotoba kotoba)
        {
            _context.Conn.Update(kotoba);
            return kotoba;
        }

        public void Delete(int id)
        {
            _context.Conn.Delete<Kotoba>(id);
        }
    }
}