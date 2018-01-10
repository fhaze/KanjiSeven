using System;
using Gdk;
using Gtk;
using KanjiSeven.Data.Entities;
using KanjiSeven.Services;
using KanjiSeven.Widgets;
using Key = Gdk.Key;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class KotobaEditor : BaseWindow
    {
        private readonly VBox   _mainVerticalBox = new VBox { BorderWidth = 10};
        private readonly Entry  _idEntry         = new Entry{ WidthRequest = 50, IsEditable = false };
        private readonly Entry  _kotobaEntry     = new Entry();
        private readonly Entry  _furiganaEntry   = new Entry();
        private readonly Entry  _romajiEntry    = new Entry();
        private readonly Entry  _honyakuEntry    = new Entry();
        private readonly Button _confirmButton   = new Button {Label = "OK"};
        private readonly Button _closeButton     = new Button {Label = "閉じる" };

        private readonly Window        _parentWindow;
        private readonly KotobaService _kotobaService = KotobaService.Current;
        private readonly Kotoba        _kotoba;

        public KotobaEditor(Window parent, int id) : this(parent)
        {
            _kotoba = _kotobaService.Get(id);
            _idEntry.Text = _kotoba.Id.ToString();
            _kotobaEntry.Text = _kotoba.Namae;
            _furiganaEntry.Text = _kotoba.Furigana;
            _romajiEntry.Text = _kotoba.Romaji;
            _honyakuEntry.Text = _kotoba.Honyaku;
        }
        
        public KotobaEditor(Window parent) : base("言葉を編集")
        {
            _parentWindow = parent;
            WidthRequest = 400;
            Modal = true;
            TransientFor = _parentWindow;
            Resizable = true;
            SetPosition(WindowPosition.CenterOnParent);
            
            var table = new Table(4, 2, false)
            {
                ColumnSpacing = 5,
                RowSpacing = 5
            };
            table.Attach(new Label("ID")
                { WidthRequest = 50, Xalign = 1 }, 0, 1, 0, 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("言葉")
                { WidthRequest = 50, Xalign = 1 }, 0, 1, 1, 2, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("ふりがな")
                { WidthRequest = 50, Xalign = 1 }, 0, 1, 2, 3, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("ローマ字")
                { WidthRequest = 50, Xalign = 1 }, 0, 1, 3, 4, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("翻訳")
                {WidthRequest = 50,  Xalign = 1 }, 0, 1, 4, 5, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            
            var hbox = new HBox();
            hbox.PackStart(_idEntry, false, false, 0);
            table.Attach(hbox, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            table.Attach(_kotobaEntry, 1, 2, 1, 2);
            table.Attach(_furiganaEntry, 1, 2, 2, 3);
            table.Attach(_romajiEntry, 1, 2, 3, 4);
            table.Attach(_honyakuEntry, 1, 2, 4, 5);

            _kotobaEntry.Changed += EntryOnChanged;
            _furiganaEntry.Changed += EntryOnChanged;
            _honyakuEntry.Changed += EntryOnChanged;
            
            _mainVerticalBox.PackStart(table, false, false, 0);
            _mainVerticalBox.PackStart(new HSeparator(), false, true, 5);
            
            var hbbox = new HButtonBox {Layout = ButtonBoxStyle.End, Spacing = 10};
            _closeButton.Clicked += CloseButtonOnClicked;
            _confirmButton.Clicked += ConfirmButtonOnClicked;
            hbbox.PackStart(_confirmButton);
            hbbox.PackStart(_closeButton);

            _mainVerticalBox.PackStart(hbbox, false, false, 0);
            
            Add(_mainVerticalBox);
            _kotobaEntry.GrabFocus();
            
            ShowAll();
            EntryOnChanged(null, null);
        }

        private void CloseButtonOnClicked(object sender, EventArgs eventArgs)
        {
            Destroy();
        }

        private void EntryOnChanged(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(_kotobaEntry.Text.Trim()) ||
                string.IsNullOrEmpty(_furiganaEntry.Text.Trim()) ||
                string.IsNullOrEmpty(_romajiEntry.Text.Trim()) ||
                string.IsNullOrEmpty(_honyakuEntry.Text.Trim()))
                _confirmButton.Sensitive = false;
            else
                _confirmButton.Sensitive = true;
        }


        private void ConfirmButtonOnClicked(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(_idEntry.Text))
                _kotobaService.Insert
                (
                    _kotobaEntry.Text.Trim(),
                    _furiganaEntry.Text.Trim(),
                    _romajiEntry.Text.Trim(),
                    _honyakuEntry.Text.Trim()
                );
            else
            {
                _kotoba.Namae = _kotobaEntry.Text.Trim();
                _kotoba.Furigana = _furiganaEntry.Text.Trim();
                _kotoba.Romaji = _romajiEntry.Text.Trim();
                _kotoba.Honyaku = _honyakuEntry.Text.Trim();
                _kotobaService.Update(_kotoba);
            }

            if (_parentWindow.GetType() == typeof(KotobaList))
            {
                var list = _parentWindow as KotobaList;
                list.RefreshList();
            }
            Destroy();
        }
    }
}