using System;
using System.Linq;
using System.Reflection;
using Gdk;
using Gtk;
using KanjiSeven.Data;
using KanjiSeven.Data.Entities;
using KanjiSeven.Services;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class KanjiEditor : Window
    {
        private readonly KotobaService   _kotobaService      = KotobaService.Current;
        private readonly VBox            _mainVerticalBox    = new VBox();
        private readonly TreeView        _kanjiView          = new TreeView();
        private readonly Entry           _filterEntry        = new Entry();
        private readonly Button          _cleanButton        = new Button { Label = "クリーン" };
        private readonly Button          _addKotobaButton    = new Button { Label = "新しい言葉"};
        private readonly Button          _editKotobaButton   = new Button { Label = "編集"};
        private readonly Button          _deleteKotobaButton = new Button { Label = "削除"};
        private readonly TreeModelFilter _filter;
        
        private ListStore Store { set; get; } = new ListStore(typeof(string), typeof(string), typeof(string));

        public KanjiEditor(Window parent) : base("漢字を登録")
        {
            SetSizeRequest(800, 600);
            Modal = true;
            TransientFor = parent;
            SetPosition(WindowPosition.CenterOnParent);

            var hbox = new HBox();
            hbox.PackStart(new Label("フィルタ"), false, true, 5);
            hbox.PackStart(_filterEntry, true, true, 0);
            hbox.PackStart(_cleanButton, false, true, 5);
            _filterEntry.Changed += FilterEntryOnChanged;
            _cleanButton.Clicked += CleanButtonOnClicked;
            _mainVerticalBox.PackStart(hbox, false, true, 5);

            var hbbox = new HButtonBox {Layout = ButtonBoxStyle.Start, Spacing = 5};
            hbbox.PackStart(_addKotobaButton);
            hbbox.PackStart(_editKotobaButton);
            hbbox.PackStart(_deleteKotobaButton);
            _addKotobaButton.Clicked += AddKotobaButtonOnClicked;
            _mainVerticalBox.PackStart(hbbox, false, true, 5);
            
            _kanjiView.AppendColumn("言葉", new CellRendererText { Size = 24 }, "text", 0);
            _kanjiView.AppendColumn("ふりがな", new CellRendererText(), "text", 1);
            _kanjiView.AppendColumn("翻訳", new CellRendererText(), "text", 2);

            foreach (var kotoba in _kotobaService.List)
                Store.AppendValues(kotoba.Namae, kotoba.Furigana, kotoba.Honyaku);                

            _filter = new TreeModelFilter(Store, null) { VisibleFunc = FilterKanji };
            _kanjiView.Model = _filter;
            
            _mainVerticalBox.PackStart(_kanjiView);
            Add(_mainVerticalBox);
            
            ShowAll();
        }

        private void AddKotobaButtonOnClicked(object sender, EventArgs eventArgs)
        {
            new KanjiEditorAddKotoba(this);
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
            var kotoba = model.GetValue(iter, 0).ToString();
            var furigana = model.GetValue(iter, 1).ToString();
            var honyaku = model.GetValue(iter, 2).ToString();

            if (_filterEntry.Text == string.Empty)
                return true;

            return
                kotoba.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1 ||
                furigana.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1 ||
                honyaku.IndexOf(_filterEntry.Text, StringComparison.Ordinal) > -1;
        }
    }
}