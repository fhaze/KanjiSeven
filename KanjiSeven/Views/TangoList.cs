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
    public class TangoList : BaseWindow
    {
        private readonly TangoService   _tangoService      = TangoService.Current;
        private readonly VBox           _mainVerticalBox   = new VBox();
        private readonly ScrolledWindow _tangoScroll       = new ScrolledWindow();
        private readonly TreeView       _tangoView         = new TreeView();
        private readonly Entry          _filterEntry       = new Entry { WidthRequest = 250 };
        private readonly Button         _cleanButton       = new Button { Label = "クリーン", WidthRequest = 80};
        private readonly Button         _addTangoButton    = new Button { Label = "新しい単語"};
        private readonly Button         _editTangoButton   = new Button { Label = "編集"};
        private readonly FhButton       _deleteTangoButton = new FhButton { Label = "削除"};
        private TreeModelFilter _filter;
        private TreeModelSort   _sort;
        
        private ListStore Store { set; get; }

        public TangoList(Window parent) : base("単語の登録")
        {
            SetSizeRequest(800, 600);
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
            hbbox.PackStart(_addTangoButton);
            hbbox.PackStart(_editTangoButton);
            hbbox.PackStart(_deleteTangoButton);
            _addTangoButton.Clicked += AddTangoButtonOnClicked;
            _editTangoButton.Clicked += EditTangoButtonOnClicked;
            _deleteTangoButton.Clicked += DeleteTangoButtonOnClicked;
            _deleteTangoButton.SetButtonColor(255, 150, 150);
            hbox.PackStart(hbbox, false, false, 0);
            _mainVerticalBox.PackStart(hbox, false, true, 5);

            _tangoView.Events = EventMask.ButtonPressMask;
            _tangoView.ButtonPressEvent += TangoViewOnButtonPressEvent;
            _tangoView.AppendColumn("ID", new CellRendererText(), "text", 0);
            _tangoView.AppendColumn("単語", new CellRendererText{ Scale = 1.5 }, "text", 1);
            _tangoView.AppendColumn("ふりがな", new CellRendererText(), "text", 2);
            _tangoView.AppendColumn("ローマ字", new CellRendererText(), "text", 3);
            _tangoView.AppendColumn("翻訳", new CellRendererText(), "text", 4);
            _tangoView.AppendColumn("見た", new CellRendererText(), "text", 5);
            _tangoView.AppendColumn("正解", new CellRendererText(), "text", 6);
            _tangoView.AppendColumn("間違", new CellRendererText(), "text", 7);
            _tangoView.AppendColumn("正解率", new CellRendererText(), "text", 8);

            var i = 0;
            foreach (var tangoViewColumn in _tangoView.Columns)
            {
                tangoViewColumn.SortColumnId = i++;
                tangoViewColumn.Clickable = true;
            }
            
            RefreshList();                
            
            _tangoScroll.Add(_tangoView);
            _mainVerticalBox.PackStart(_tangoScroll);
            Add(_mainVerticalBox);
            
            ShowAll();
        }

        private void DeleteTangoButtonOnClicked(object sender, EventArgs eventArgs)
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
        private void TangoViewOnButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            if (args.Event.Type != EventType.TwoButtonPress) return;
            
            EditSelected();
        }


        private void EditTangoButtonOnClicked(object sender, EventArgs eventArgs)
        {
            EditSelected();
        }

        public void EditSelected()
        {
            var selection = _tangoView.Selection;

            if (selection.GetSelected(out var model, out var iter))
                new TangoEditor(this, Convert.ToInt32(model.GetValue(iter, 0)));            
        }

        public void DeleteSelected()
        {
            var selection = _tangoView.Selection;

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
                        _tangoService.Delete(Convert.ToInt32(model.GetValue(iter, 0)));
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

            foreach (var tango in _tangoService.List)
                Store.AppendValues(tango.Id, tango.Namae, tango.Furigana, tango.Romaji, tango.Honyaku,
                    tango.Seen, tango.Answer, tango.Wrong, tango.Ratio);
            
            _filter = new TreeModelFilter(Store, null) { VisibleFunc = FilterKanji };
            _sort = new TreeModelSort(_filter);
            _tangoView.Model = _sort;
        }
        
        private void AddTangoButtonOnClicked(object sender, EventArgs eventArgs)
        {
            new TangoEditor(this);
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
            var tango = model.GetValue(iter, 1).ToString();
            var furigana = model.GetValue(iter, 2).ToString();
            var romaji = model.GetValue(iter, 3).ToString();
            var honyaku = model.GetValue(iter, 4).ToString();

            if (_filterEntry.Text == string.Empty)
                return true;

            if (int.TryParse(_filterEntry.Text, out var intValue))
                return intValue == id;
            
            return
                tango.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1 ||
                furigana.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1 ||
                romaji.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1 ||
                honyaku.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1;
        }
    }
}