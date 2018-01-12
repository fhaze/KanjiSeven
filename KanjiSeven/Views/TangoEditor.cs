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
    public class TangoEditor : BaseWindow
    {
        private readonly VBox   _mainVerticalBox = new VBox { BorderWidth = 10};
        private readonly Entry  _idEntry         = new Entry{ WidthRequest = 50, IsEditable = false };
        private readonly Entry  _tangoEntry      = new Entry();
        private readonly Entry  _furiganaEntry   = new Entry();
        private readonly Entry  _romajiEntry     = new Entry();
        private readonly Entry  _honyakuEntry    = new Entry();
        private readonly Button _confirmButton   = new Button {Label = "OK"};
        private readonly Button _closeButton     = new Button {Label = "閉じる" };

        private readonly Window        _parentWindow;
        private readonly TangoService _tangoService = TangoService.Current;
        private readonly Tango        _tango;

        public TangoEditor(Window parent, int id) : this(parent)
        {
            _tango = _tangoService.Get(id);
            _idEntry.Text = _tango.Id.ToString();
            _tangoEntry.Text = _tango.Namae;
            _furiganaEntry.Text = _tango.Furigana;
            _romajiEntry.Text = _tango.Romaji;
            _honyakuEntry.Text = _tango.Honyaku;
        }
        
        public TangoEditor(Window parent) : base("単語の編集")
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
            table.Attach(new Label("単語")
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
            table.Attach(_tangoEntry, 1, 2, 1, 2);
            table.Attach(_furiganaEntry, 1, 2, 2, 3);
            table.Attach(_romajiEntry, 1, 2, 3, 4);
            table.Attach(_honyakuEntry, 1, 2, 4, 5);

            _tangoEntry.Changed += EntryOnChanged;
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
            _tangoEntry.GrabFocus();
            
            ShowAll();
            EntryOnChanged(null, null);
        }

        private void CloseButtonOnClicked(object sender, EventArgs eventArgs)
        {
            Destroy();
        }

        private void EntryOnChanged(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(_tangoEntry.Text.Trim()) ||
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
                _tangoService.Insert
                (
                    _tangoEntry.Text.Trim(),
                    _furiganaEntry.Text.Trim(),
                    _romajiEntry.Text.Trim(),
                    _honyakuEntry.Text.Trim()
                );
            else
            {
                _tango.Namae = _tangoEntry.Text.Trim();
                _tango.Furigana = _furiganaEntry.Text.Trim();
                _tango.Romaji = _romajiEntry.Text.Trim();
                _tango.Honyaku = _honyakuEntry.Text.Trim();
                _tangoService.Update(_tango);
            }

            if (_parentWindow.GetType() == typeof(TangoList))
            {
                var list = _parentWindow as TangoList;
                list.RefreshList();
            }
            Destroy();
        }
    }
}