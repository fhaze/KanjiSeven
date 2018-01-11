using System;
using SQLite;

namespace KanjiSeven.Data.Entities
{
    public class Kotoba
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Namae { get; set; }
        public string Furigana { get; set; }
        public string Romaji { get; set; }
        public string Honyaku { get; set; }

        public int Seen { get; set; }
        public int Answer { get; set; }
        public int Wrong { get; set; }

        public int Ratio
        {
            get
            {
                var result = (float) Answer / (Seen + Wrong) * 100;
                return result > 0 ? Convert.ToInt32(result) : 0;
            }
        }
    }
}