using System;
using System.IO;
using Gdk;
using Gtk;
using KanjiSeven.Data;
using KanjiSeven.Extensions;
using KanjiSeven.Models;
using KanjiSeven.Widgets;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class ConfigForm : BaseWindow
    {
        private readonly VBox        _mainVerticalBox   = new VBox { BorderWidth = 10 };
        private readonly Entry       _dbDirectoryEntry  = new Entry { WidthRequest = 300 };
        private readonly Button      _dbDirectoryBrowse = new Button { Label = "ブラウズ", WidthRequest = 60};
        private readonly Button      _confirmButton     = new Button { Label = "OK" };
        private readonly Button      _cancelButton      = new Button { Label = "キャンセル" };
        private readonly RadioButton _simpleRadio;
        private readonly RadioButton _guessRadio;
        private readonly RadioButton _inputRadio;
        private readonly CheckButton _showHint          = new CheckButton { Label = "ヒントを見せて" };
        private readonly HScale      _hintScale         = new HScale(0, 10, 1);

        private readonly Configuration _configuration = ConfigManager.Current; 
        
        public ConfigForm(Window parent) : base("設定")
        {
            Modal = true;
            TransientFor = parent;
            SetPosition(WindowPosition.CenterOnParent);

            var table = new Table(7, 2, false)
            {
                ColumnSpacing = 10,
                RowSpacing = 2
            };
            table.Attach(new Label("DBファイラー")
                { Xalign = 1, WidthRequest = 90 }, 0, 1, 0, 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("ヒントスピード（秒）")
                { Xalign = 1, WidthRequest = 90 }, 0, 1, 2, 3, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("ゲームモード")
                { Xalign = 1, WidthRequest = 90 }, 0, 1, 3, 7, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            
            var hbox = new HBox();
            hbox.PackStart(_dbDirectoryEntry, true, true, 0);
            hbox.PackStart(_dbDirectoryBrowse, false, false, 0);
            table.Attach(hbox, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            _dbDirectoryEntry.Text = _configuration.StorageDir;
            _dbDirectoryBrowse.Clicked += DbDirectoryBrowseOnClicked;
            table.Attach(_showHint, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            _showHint.Active = _configuration.ShowHint;
            _showHint.Toggled += ShowHintOnToggled;
            
            table.Attach(_hintScale, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            
            _simpleRadio = new RadioButton(_simpleRadio, GameStyle.Simple.Label());
            _guessRadio = new RadioButton(_simpleRadio, GameStyle.GuessMode.Label());
            _inputRadio = new RadioButton(_simpleRadio, GameStyle.InputMode.Label());
            
            _simpleRadio.Toggled += SimpleRadioOnToggled;
            _guessRadio.Toggled += SimpleRadioOnToggled;
            _inputRadio.Toggled += SimpleRadioOnToggled;
            
            switch (_configuration.GameStyle)
            {
                case GameStyle.Simple:
                    _simpleRadio.Active = true;
                    break;
                case GameStyle.GuessMode:
                    _guessRadio.Active = true;
                    break;
                case GameStyle.InputMode:
                    _inputRadio.Active = true;
                    break;
            }
            
            table.Attach(_simpleRadio, 1, 2, 3, 4, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            table.Attach(_guessRadio, 1, 2, 4, 5, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            table.Attach(_inputRadio, 1, 2, 6, 7, AttachOptions.Fill, AttachOptions.Fill, 0, 0);

            _hintScale.Sensitive = _configuration.ShowHint;
            _hintScale.Value = _configuration.HintSpeed;            
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

        private void ShowHintOnToggled(object sender, EventArgs eventArgs)
        {
            if (sender is CheckButton check)
            {
                _hintScale.Sensitive = check.Active;
                _configuration.ShowHint = check.Active;
            }
        }

        private void SimpleRadioOnToggled(object sender, EventArgs eventArgs)
        {
            var radio = sender as RadioButton;

            if (radio?.Label == GameStyle.Simple.Label())
                _configuration.GameStyle = GameStyle.Simple;
            else if (radio?.Label == GameStyle.GuessMode.Label())
                _configuration.GameStyle = GameStyle.GuessMode;
            else if (radio?.Label == GameStyle.InputMode.Label())
                _configuration.GameStyle = GameStyle.InputMode;
        }

        private void CancelButtonOnClicked(object sender, EventArgs eventArgs)
        {
            Destroy();
        }

        private void ConfirmButtonOnClicked(object sender, EventArgs eventArgs)
        {
            _configuration.StorageDir = _dbDirectoryEntry.Text.Trim();
            _configuration.HintSpeed = Convert.ToInt32(_hintScale.Value);
            
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