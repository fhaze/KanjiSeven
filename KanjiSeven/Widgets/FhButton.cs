using System.Security.Cryptography;
using Gdk;
using Gtk;
using KanjiSeven.Extensions;
using KanjiSeven.Utils;

namespace KanjiSeven.Widgets
{
    public class FhButton : Button
    {
        private readonly  EventBox _eventBox;
        private readonly  Label    _label;
        
        public new string Label
        {
            get => _label.Text;
            set => _label.Text = value;
        }
        
        public FhButton()
        {
            _label = new Label();
            _eventBox = new EventBox { _label };
            Add(_eventBox);
        }
        

        public FhButton SetButtonColor(Color color)
        {
            if (SystemEnviroment.IsUnix)
                this.SetBackgroundColor(color);
            else
                _eventBox.SetBackgroundColor(color);
            return this;
        }
        
        public FhButton SetButtonColor(byte r, byte g, byte b)
        {
            if (SystemEnviroment.IsUnix)
                this.SetBackgroundColor(r, g, b);
            else
                _eventBox.SetBackgroundColor(r, g, b);
            return this;
        }

        public FhButton SetFontSize(int size)
        {
            _label.SetFontSize(size);
            return this;
        }
    }
}