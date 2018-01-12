using System.Collections.Generic;
using KanjiSeven.Data;
using KanjiSeven.Data.Entities;
using SQLite;

namespace KanjiSeven.Services
{
    public sealed class TangoService
    {
        public static TangoService    Current { get; } = new TangoService();
        private readonly LocalContext _context;

        private TangoService()
        {
            _context = LocalContext.Current;
        }

        public IList<Tango> List => _context.Conn.Table<Tango>().ToList();

        public Tango Get(int id)
        {
            return _context.Conn.Get<Tango>(id);
        }
        
        public void Mark(Tango tango, Mark mark)
        {
            switch (mark)
            {
                case Services.Mark.Answer:
                    tango.Answer++;
                    break;
                case Services.Mark.Wrong:
                    tango.Wrong++;
                    break;
                case Services.Mark.Seen:
                    tango.Seen++;
                    break;
            }

            _context.Conn.Update(tango);
        }

        public Tango Insert(string namae, string furigana, string romaji, string honyaku)
        {
            var tango = new Tango {Namae = namae, Furigana = furigana, Romaji = romaji, Honyaku = honyaku};
            _context.Conn.Insert(tango);
            return tango;
        }

        public void Update(Tango tango)
        {
            _context.Conn.Update(tango);
        }

        public void Delete(int id)
        {
            _context.Conn.Delete<Tango>(id);
        }
    }

    public enum Mark
    {
        Seen,
        Answer,
        Wrong
    }
}