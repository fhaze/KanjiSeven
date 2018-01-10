using System;
using System.IO;
using Gdk;
using Gtk;
using KanjiSeven.Data;
using KanjiSeven.Widgets;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class ConfigForm : BaseWindow
    {
        private readonly VBox   _mainVerticalBox   = new VBox { BorderWidth = 10 };
        private readonly Entry  _dbDirectoryEntry  = new Entry { WidthRequest = 300 };
        private readonly Button _dbDirectoryBrowse = new Button { Label = "ブラウズ" };
        private readonly Button _confirmButton     = new Button { Label = "OK" };
        private readonly Button _cancelButton      = new Button { Label = "キャンセル" };
        private readonly HScale _hintScale         = new HScale(0, 10, 5);

        private readonly Configuration _configuration = ConfigManager.Current; 
        
        public ConfigForm(Window parent) : base("設定")
        {
            Modal = true;
            TransientFor = parent;
            SetPosition(WindowPosition.CenterOnParent);
            
            var table = new Table(3, 2, false);
            table.Attach(new Label("DBファイラー"), 0, 1, 0, 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("ヒントスピード"), 0, 1, 1, 2, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            
            var hbox = new HBox();
            hbox.PackStart(_dbDirectoryEntry, true, true, 0);
            hbox.PackStart(_dbDirectoryBrowse, false, false, 0);
            table.Attach(hbox, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            _dbDirectoryEntry.Text = _configuration.StorageDir;
            _dbDirectoryBrowse.Clicked += DbDirectoryBrowseOnClicked;

            table.Attach(_hintScale, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            
            _mainVerticalBox.PackStart(table, false, false, 0);
            _mainVerticalBox.PackStart(new HSeparator(), false, true, 5);

            var hbbox = new HButtonBox {Layout = ButtonBoxStyle.End, Spacing = 10};
            hbbox.PackStart(_confirmButton);
            hbbox.PackStart(_cancelButton);
            _confirmButton.Clicked += ConfirmButtonOnClicked;
            _cancelButton.Clicked += CancelButtonOnClicked;
            _mainVerticalBox.PackStart(hbbox);
            
            Add(_mainVerticalBox);
            ShowAll();
        }

        private void CancelButtonOnClicked(object sender, EventArgs eventArgs)
        {
            Destroy();
        }

        private void ConfirmButtonOnClicked(object sender, EventArgs eventArgs)
        {
            _configuration.StorageDir = _dbDirectoryEntry.Text.Trim();
            ConfigManager.Save(_configuration, false);
            LocalContext.Current.Reload();
            Destroy();
        }

        private void DbDirectoryBrowseOnClicked(object sender, EventArgs eventArgs)
        {
            var fc = new FileChooserDialog("", this, FileChooserAction.Open,
                "キャンセル", ResponseType.Cancel,
                "開く", ResponseType.Accept);

            if (fc.Run() == (int) ResponseType.Accept)
            {
                _dbDirectoryEntry.Text = fc.Filename;
            }
            fc.Destroy();
        }
    }
}