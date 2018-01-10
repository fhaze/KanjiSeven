using System;
using Gtk;
using KanjiSeven.Data.Entities;
using KanjiSeven.Services;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class KotobaEditor : Window
    {
        private readonly VBox   _mainVerticalBox = new VBox { BorderWidth = 10};
        private readonly Entry  _idEntry         = new Entry{ WidthRequest = 50, IsEditable = false };
        private readonly Entry  _kotobaEntry     = new Entry();
        private readonly Entry  _hiraganaEntry   = new Entry();
        private readonly Entry  _honyakuEntry    = new Entry();
        private readonly Button _confirmButton   = new Button {Label = "OK"};
        private readonly Button _cancelButton    = new Button {Label = "キャンセル"};

        private readonly KotobaService _kotobaService = KotobaService.Current;
        private readonly Kotoba        _kotoba;

        public KotobaEditor(Window parent, int id) : this(parent)
        {
            _kotoba = _kotobaService.Get(id);
        }
        
        public KotobaEditor(Window parent) : base("言葉を編集")
        {
            WidthRequest = 400;
            Modal = true;
            TransientFor = parent;
            Resizable = true;
            SetPosition(WindowPosition.CenterOnParent);
            
            var table = new Table(3, 2, false)
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
            table.Attach(new Label("翻訳")
                {WidthRequest = 50,  Xalign = 1 }, 0, 1, 3, 4, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            
            var hbox = new HBox();
            hbox.PackStart(_idEntry, false, false, 0);
            table.Attach(hbox, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            table.Attach(_kotobaEntry, 1, 2, 1, 2);
            table.Attach(_hiraganaEntry, 1, 2, 2, 3);
            table.Attach(_honyakuEntry, 1, 2, 3, 4);
            
            _mainVerticalBox.PackStart(table, false, false, 0);
            _mainVerticalBox.PackStart(new HSeparator(), false, true, 5);
            
            var hbbox = new HButtonBox {Layout = ButtonBoxStyle.Spread};
            _confirmButton.Clicked += ConfirmButtonOnClicked;
            hbbox.PackStart(_confirmButton);
            hbbox.PackStart(_cancelButton);

            _mainVerticalBox.PackStart(hbbox, false, false, 0);
            
            Add(_mainVerticalBox);
            _kotobaEntry.GrabFocus();
            
            ShowAll();
        }

        private void ConfirmButtonOnClicked(object sender, EventArgs eventArgs)
        {
            
        }
    }
}