namespace KanjiSeven.Models
{
    public class Configuration
    {
        public string StorageDir { get; set; }
        public bool ShowHint { get; set; }
        public int HintSpeed { get; set; }
        public GameMode GameMode { get; set; }
    }
}