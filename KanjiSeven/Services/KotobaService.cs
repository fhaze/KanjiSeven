using System.Collections.Generic;
using KanjiSeven.Data;
using KanjiSeven.Data.Entities;
using SQLite;

namespace KanjiSeven.Services
{
    public sealed class KotobaService
    {
        public static KotobaService   Current { get; } = new KotobaService();
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
        
        public void GetAndIncrementSeen(int id)
        {
            var kotoba = _context.Conn.Get<Kotoba>(id);
            kotoba.Seen += 1;
            Update(kotoba);
        }
        
        public void Mark(Kotoba kotoba, Mark mark)
        {
            switch (mark)
            {
                case Services.Mark.Answer:
                    kotoba.Answer++;
                    break;
                case Services.Mark.Wrong:
                    kotoba.Wrong++;
                    break;
                case Services.Mark.Seen:
                    kotoba.Seen++;
                    break;
            }

            _context.Conn.Update(kotoba);
        }

        public Kotoba Insert(string namae, string furigana, string romaji, string honyaku)
        {
            var kotoba = new Kotoba {Namae = namae, Furigana = furigana, Romaji = romaji, Honyaku = honyaku};
            _context.Conn.Insert(kotoba);
            return kotoba;
        }

        public void Update(Kotoba kotoba)
        {
            _context.Conn.Update(kotoba);
        }

        public void Delete(int id)
        {
            _context.Conn.Delete<Kotoba>(id);
        }
    }

    public enum Mark
    {
        Seen,
        Answer,
        Wrong
    }
}