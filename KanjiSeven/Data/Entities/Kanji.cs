using SQLite;

namespace KanjiSeven.Data.Entities
{
    public class Kanji
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string Kotoba { get; set; }
        public string Furigana { get; set; }
        public string Honyaku { get; set; }
    }
}