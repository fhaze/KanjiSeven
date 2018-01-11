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
using KanjiSeven.Widgets;
using Key = Gdk.Key;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class KotobaList : BaseWindow
    {
        private readonly KotobaService   _kotobaService      = KotobaService.Current;
        private readonly VBox            _mainVerticalBox    = new VBox();
        private readonly ScrolledWindow  _kotobaScroll       = new ScrolledWindow();
        private readonly TreeView        _kotobaView         = new TreeView();
        private readonly Entry           _filterEntry        = new Entry { WidthRequest = 250 };
        private readonly Button          _cleanButton        = new Button { Label = "クリーン", WidthRequest = 80};
        private readonly Button          _addKotobaButton    = new Button { Label = "新しい言葉"};
        private readonly Button          _editKotobaButton   = new Button { Label = "編集"};
        private readonly Button          _deleteKotobaButton = new Button { Label = "削除"};
        private TreeModelFilter _filter;
        private TreeModelSort   _sort;
        
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
            _kotobaView.AppendColumn("言葉", new CellRendererText{ Scale = 1.5 }, "text", 1);
            _kotobaView.AppendColumn("ふりがな", new CellRendererText(), "text", 2);
            _kotobaView.AppendColumn("ローマ字", new CellRendererText(), "text", 3);
            _kotobaView.AppendColumn("翻訳", new CellRendererText(), "text", 4);
            _kotobaView.AppendColumn("見た", new CellRendererText(), "text", 5);
            _kotobaView.AppendColumn("正解", new CellRendererText(), "text", 6);
            _kotobaView.AppendColumn("間違った答え", new CellRendererText(), "text", 7);
            _kotobaView.AppendColumn("正解率", new CellRendererText(), "text", 8);

            var i = 0;
            foreach (var kotobaViewColumn in _kotobaView.Columns)
            {
                kotobaViewColumn.SortColumnId = i++;
                kotobaViewColumn.Clickable = true;
            }
            
            RefreshList();                
            
            _kotobaScroll.Add(_kotobaView);
            _mainVerticalBox.PackStart(_kotobaScroll);
            Add(_mainVerticalBox);
            
            ShowAll();
        }

        private void DeleteKotobaButtonOnClicked(object sender, EventArgs eventArgs)
        {
            DeleteSelected();
        }

        [ConnectBefore]
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
            Store = new ListStore(typeof(int), typeof(string), typeof(string), typeof(string), typeof(string),
                typeof(int), typeof(int), typeof(int), typeof(int));

            foreach (var kotoba in _kotobaService.List)
                Store.AppendValues(kotoba.Id, kotoba.Namae, kotoba.Furigana, kotoba.Romaji, kotoba.Honyaku,
                    kotoba.Seen, kotoba.Answer, kotoba.Wrong, kotoba.Ratio);
            
            _filter = new TreeModelFilter(Store, null) { VisibleFunc = FilterKanji };
            _sort = new TreeModelSort(_filter);
            _kotobaView.Model = _sort;
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
            var romaji = model.GetValue(iter, 3).ToString();
            var honyaku = model.GetValue(iter, 4).ToString();

            if (_filterEntry.Text == string.Empty)
                return true;

            if (int.TryParse(_filterEntry.Text, out var intValue))
                return intValue == id;
            
            return
                kotoba.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1 ||
                furigana.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1 ||
                romaji.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1 ||
                honyaku.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1;
        }
    }
}