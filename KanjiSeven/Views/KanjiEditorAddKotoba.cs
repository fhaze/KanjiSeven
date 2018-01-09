using Gtk;

namespace KanjiSeven.Views
{
    public class KanjiEditorAddKotoba : Window
    {
        public KanjiEditorAddKotoba(Window parent) : base("新しい言葉")
        {
            SetSizeRequest(400, 400);
            Modal = true;
            TransientFor = parent;
            SetPosition(WindowPosition.CenterOnParent);
            
            ShowAll();
        }
    }
}