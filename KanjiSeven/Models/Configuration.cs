namespace KanjiSeven.Models
{
    public class Configuration
    {
        public string StorageDir { get; set; }
        public bool ShowHint { get; set; }
        public int HintSpeed { get; set; }
        public bool AutoMode { get; set; }
        public int AutoModeSpeed { get; set; }
        public GameMode GameMode { get; set; }
        public TangoType QuestionType { get; set; }
        public TangoType AnswerType { get; set; }
    }
}