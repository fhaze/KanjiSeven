using KanjiSeven.Data.Entities;

namespace KanjiSeven.Models
{
    public class FlashCard
    {
        public int Number { get; set; }
        public Tango Tango { get; set; }

        public FlashCard(Tango tango)
        {
            Tango = tango;
        }
    }
}