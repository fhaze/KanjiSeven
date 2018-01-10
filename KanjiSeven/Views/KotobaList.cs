using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Gdk;
using GLib;
using Gtk;
using KanjiSeven.Data;
using KanjiSeven.Data.Entities;
using KanjiSeven.Services;
using Key = Gdk.Key;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class KotobaList : Window
    {
        private readonly KotobaService   _kotobaService      = KotobaService.Current;
        private readonly VBox            _mainVerticalBox    = new VBox();
        private readonly TreeView        _kotobaView          = new TreeView();
        private readonly Entry           _filterEntry        = new Entry() { WidthRequest = 250 };
        private readonly Button          _cleanButton        = new Button { Label = "クリーン", WidthRequest = 80};
        private readonly Button          _addKotobaButton    = new Button { Label = "新しい言葉"};
        private readonly Button          _editKotobaButton   = new Button { Label = "編集"};
        private readonly Button          _deleteKotobaButton = new Button { Label = "削除"};
        private TreeModelFilter _filter;
        
        private ListStore Store { set; get; }

        public KotobaList(Window parent) : base("言葉を登録")
        {
            SetSizeRequest(800, 600);
            Modal = true;
            TransientFor = parent;
            SetPosition(WindowPosition.CenterOnParent);

            KeyReleaseEvent += OnKeyReleaseEvent;
            
            var hbox = new HBox { BorderWidth = 5 };
            hbox.PackStart(new Label("フィルタ"), false, true, 5);
            hbox.PackStart(_filterEntry, false, false, 0);
            hbox.PackStart(_cleanButton, false, true, 5);
            _filterEntry.Changed += FilterEntryOnChanged;
            _cleanButton.Clicked += CleanButtonOnClicked;
            hbox.PackStart(new VSeparator(), false, false, 5);

            var hbbox = new HButtonBox { Layout = ButtonBoxStyle.Start, Spacing = 5};
            hbbox.PackStart(_addKotobaButton);
            hbbox.PackStart(_editKotobaButton);
            hbbox.PackStart(_deleteKotobaButton);
            _addKotobaButton.Clicked += AddKotobaButtonOnClicked;
            _editKotobaButton.Clicked += EditKotobaButtonOnClicked;
            _deleteKotobaButton.Clicked += DeleteKotobaButtonOnClicked;
            hbox.PackStart(hbbox, false, false, 0);
            _mainVerticalBox.PackStart(hbox, false, true, 5);

            _kotobaView.Events = EventMask.ButtonPressMask;
            _kotobaView.ButtonPressEvent += KotobaViewOnButtonPressEvent;
            _kotobaView.AppendColumn("ID", new CellRendererText(), "text", 0);
            _kotobaView.AppendColumn("言葉", new CellRendererText { Size = 24 }, "text", 1);
            _kotobaView.AppendColumn("ふりがな", new CellRendererText(), "text", 2);
            _kotobaView.AppendColumn("翻訳", new CellRendererText(), "text", 3);

            RefreshList();                
            
            _mainVerticalBox.PackStart(_kotobaView);
            Add(_mainVerticalBox);
            
            ShowAll();
        }

        private void DeleteKotobaButtonOnClicked(object sender, EventArgs eventArgs)
        {
            DeleteSelected();
        }

        private void OnKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            switch (args.Event.Key)
            {
                case Key.Return:
                    EditSelected();
                    break;
                case Key.Delete:
                    DeleteSelected();
                    break;
                case Key.Escape:
                    Destroy();
                    break;
            }
        }

        [ConnectBefore]
        private void KotobaViewOnButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            if (args.Event.Type != EventType.TwoButtonPress) return;
            
            EditSelected();
        }


        private void EditKotobaButtonOnClicked(object sender, EventArgs eventArgs)
        {
            EditSelected();
        }

        public void EditSelected()
        {
            var selection = _kotobaView.Selection;

            if (selection.GetSelected(out var model, out var iter))
                new KotobaEditor(this, Convert.ToInt32(model.GetValue(iter, 0)));            
        }

        public void DeleteSelected()
        {
            var selection = _kotobaView.Selection;

            if (selection.GetSelected(out var model, out var iter))
            {
                var md = new MessageDialog
                (
                    this,
                    DialogFlags.Modal,
                    MessageType.Question,
                    ButtonsType.YesNo,
                    $"\"{model.GetValue(iter, 1)}\" を削除する？"
                );
                md.Response += delegate(object o, ResponseArgs args)
                {
                    if (args.ResponseId == ResponseType.Yes)
                    {
                        _kotobaService.Delete(Convert.ToInt32(model.GetValue(iter, 0)));
                        RefreshList();
                    }
                };
                md.Run();
                md.Destroy();
            }
        }
        
        public void RefreshList()
        {
            Store = new ListStore(typeof(int), typeof(string), typeof(string), typeof(string));

            foreach (var kotoba in _kotobaService.List)
                Store.AppendValues(kotoba.Id, kotoba.Namae, kotoba.Furigana, kotoba.Honyaku);
            
            _filter = new TreeModelFilter(Store, null) { VisibleFunc = FilterKanji };
            _kotobaView.Model = _filter;
        }
        
        private void AddKotobaButtonOnClicked(object sender, EventArgs eventArgs)
        {
            new KotobaEditor(this);
        }

        private void FilterEntryOnChanged(object sender, EventArgs eventArgs)
        {
            _filter.Refilter();
        }

        private void CleanButtonOnClicked(object sender, EventArgs eventArgs)
        {
            _filterEntry.Text = string.Empty;
        }

        private bool FilterKanji(TreeModel model, TreeIter iter)
        {
            var id = (int)model.GetValue(iter, 0);
            var kotoba = model.GetValue(iter, 1).ToString();
            var furigana = model.GetValue(iter, 2).ToString();
            var honyaku = model.GetValue(iter, 3).ToString();

            if (_filterEntry.Text == string.Empty)
                return true;

            if (int.TryParse(_filterEntry.Text, out var intValue))
                return intValue == id;
            
            return
                kotoba.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1 ||
                furigana.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1 ||
                honyaku.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1;
        }
    }
}