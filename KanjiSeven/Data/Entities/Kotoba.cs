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
    }
}