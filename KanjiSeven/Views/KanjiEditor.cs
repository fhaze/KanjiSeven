using System.Reflection;
using Gtk;
using KanjiSeven.Data.Entities;

namespace KanjiSeven.Views
{
    public class KanjiEditor : Window
    {
        private readonly TreeView _kanjiView = new TreeView();
        private ListStore Store { set; get; } = new ListStore(typeof(string), typeof(string), typeof(string));

        public KanjiEditor(Window parent) : base("漢字を登録")
        {
            SetSizeRequest(400, 400);
            Modal = true;
            TransientFor = parent;
            SetPosition(WindowPosition.CenterOnParent);
            
            _kanjiView.AppendColumn("言葉", new CellRendererText(), "text", 0);
            _kanjiView.AppendColumn("ふりがな", new CellRendererText(), "text", 1);
            _kanjiView.AppendColumn("翻訳", new CellRendererText(), "text", 2);

            _kanjiView.Model = Store;

            Store.AppendValues("水", "みず", "Water");
            Store.AppendValues("山", "やま", "Mountain");
            Store.AppendValues("火", "ひ", "Fire");
            
            Add(_kanjiView);
            _kanjiView.ShowAll();
            
            ShowAll();
        }
    }
}