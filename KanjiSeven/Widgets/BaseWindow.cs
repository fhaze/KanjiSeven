using Gdk;
using Window = Gtk.Window;

namespace KanjiSeven.Widgets
{
    public class BaseWindow : Window
    {
        public BaseWindow(string title) : base(title)
        {
        }

        protected override bool OnKeyReleaseEvent(EventKey evnt)
        {
            switch (evnt.Key)
            {
                case Key.Escape:
                    Destroy();
                    break;
            }

            return true;
        }
    }
}