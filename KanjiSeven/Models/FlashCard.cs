using KanjiSeven.Data.Entities;

namespace KanjiSeven.Models
{
    public class FlashCard
    {
        public int Number { get; set; }
        public Kotoba Kotoba { get; set; }

        public FlashCard(Kotoba kotoba)
        {
            Kotoba = kotoba;
        }
    }
}