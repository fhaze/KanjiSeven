using System;
using System.IO;
using Gdk;
using Gtk;
using KanjiSeven.Data;
using KanjiSeven.Extensions;
using KanjiSeven.Models;
using KanjiSeven.Services;
using KanjiSeven.Utils;
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
        private readonly ComboBox    _questionCombobox;
        private readonly ComboBox    _answerCombobox;
        private readonly CheckButton _autoMode          = new CheckButton { Label = "オートモード" };
        private readonly HScale      _autoModeScale     = new HScale(0, 5000, 100);
        private readonly CheckButton _showHint          = new CheckButton { Label = "ヒントを見せて" };
        private readonly HScale      _hintScale         = new HScale(0, 20, 1);

        private readonly Configuration _configuration = ConfigurationService.Current; 
        
        public ConfigForm(Window parent) : base("設定")
        {
            TransientFor = parent;
            SetPosition(WindowPosition.CenterOnParent);

            var table = new Table(6, 2, false)
            {
                ColumnSpacing = 10,
                RowSpacing = 10
            };
            table.Attach(new Label("DBファイラー")
                { Xalign = 1, WidthRequest = 130 }, 0, 1, 0, 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("ヒントスピード（秒）")
                { Xalign = 1, WidthRequest = 130 }, 0, 1, 2, 3, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("オートモードスピード（ミリ秒）")
                { Xalign = 1, WidthRequest = 130 }, 0, 1, 4, 5, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            table.Attach(new Label("ゲームモード")
                { Xalign = 1, WidthRequest = 130 }, 0, 1, 5, 6, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
            
            var hbox = new HBox();
            hbox.PackStart(_dbDirectoryEntry, true, true, 0);
            hbox.PackStart(_dbDirectoryBrowse, false, false, 0);
            table.Attach(hbox, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            _dbDirectoryEntry.Text = _configuration.StorageDir;
            _dbDirectoryBrowse.Clicked += DbDirectoryBrowseOnClicked;
            table.Attach(_showHint, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            _showHint.Active = _configuration.ShowHint;
            _showHint.Toggled += ShowHintOnToggled;
            
            _hintScale.Sensitive = _configuration.ShowHint;
            _hintScale.Value = _configuration.HintSpeed;        
            table.Attach(_hintScale, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            
            table.Attach(_autoMode, 1, 2, 3, 4, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            _autoMode.Active = _configuration.AutoMode;
            _autoMode.Toggled += AutoModeOnToggled;
            
            _autoModeScale.Sensitive = _configuration.AutoMode;
            _autoModeScale.Value = _configuration.AutoModeSpeed;   
            table.Attach(_autoModeScale, 1, 2, 4, 5, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            
            _simpleRadio = new RadioButton(_simpleRadio, GameMode.Simple.Label());
            _guessRadio = new RadioButton(_simpleRadio, GameMode.GuessMode.Label());
            _inputRadio = new RadioButton(_simpleRadio, GameMode.InputMode.Label()) { Sensitive = false };
            
            _simpleRadio.Toggled += SimpleRadioOnToggled;
            _guessRadio.Toggled += SimpleRadioOnToggled;
            _inputRadio.Toggled += SimpleRadioOnToggled;
            
            switch (_configuration.GameMode)
            {
                case GameMode.Simple:
                    _simpleRadio.Active = true;
                    break;
                case GameMode.GuessMode:
                    _guessRadio.Active = true;
                    break;
                case GameMode.InputMode:
                    _inputRadio.Active = true;
                    break;
            }
            
            var gameModeHBox = new HBox(false, 20);
            var gameModeVBox = new VBox();
            gameModeVBox.PackStart(_simpleRadio, false, false, 0);
            gameModeVBox.PackStart(_guessRadio, false, false, 0);
            gameModeVBox.PackStart(_inputRadio, false, false, 0);
            gameModeHBox.PackStart(gameModeVBox, false, false, 0);
            
            var gameModeVBox2 = new VBox();
            _questionCombobox = new ComboBox(TangoTypeUtil.LabelList());
            _questionCombobox.Active = _configuration.QuestionType.Index();
            gameModeVBox2.PackStart(new Label("質問") { Xalign = 0 }, false, false, 0);
            gameModeVBox2.PackStart(_questionCombobox, false, false, 0);
            gameModeHBox.PackStart(gameModeVBox2, false, false, 0);
            
            var gameModeVBox3 = new VBox();
            _answerCombobox = new ComboBox(TangoTypeUtil.LabelList());
            _answerCombobox.Active = _configuration.AnswerType.Index();
            gameModeVBox3.PackStart(new Label("回答") { Xalign = 0 }, false, false, 0);
            gameModeVBox3.PackStart(_answerCombobox, false, false, 0);
            gameModeHBox.PackStart(gameModeVBox3, false, false, 0);
            
            table.Attach(gameModeHBox, 1, 2, 5, 6, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
                
            _mainVerticalBox.PackStart(table, false, false, 0);
            _mainVerticalBox.PackStart(new HSeparator(), false, true, 5);
            
            var hbbox = new HButtonBox {Layout = ButtonBoxStyle.End, Spacing = 10};
            hbbox.PackStart(_confirmButton);
            hbbox.PackStart(_cancelButton);;
            _confirmButton.Clicked += ConfirmButtonOnClicked;
            _cancelButton.Clicked += CancelButtonOnClicked;
            _mainVerticalBox.PackStart(hbbox);
            
            Add(_mainVerticalBox);
            ShowAll();
        }

        private void AutoModeOnToggled(object sender, EventArgs eventArgs)
        {
            if (sender is CheckButton check)
            {
                _autoModeScale.Sensitive = check.Active;
                _configuration.AutoMode = check.Active;
            }
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

            if (radio?.Label == GameMode.Simple.Label())
                _configuration.GameMode = GameMode.Simple;
            else if (radio?.Label == GameMode.GuessMode.Label())
                _configuration.GameMode = GameMode.GuessMode;
            else if (radio?.Label == GameMode.InputMode.Label())
                _configuration.GameMode = GameMode.InputMode;
        }

        private void CancelButtonOnClicked(object sender, EventArgs eventArgs)
        {
            Destroy();
        }

        private void ConfirmButtonOnClicked(object sender, EventArgs eventArgs)
        {
            _configuration.QuestionType = TangoTypeUtil.ByIndex(_questionCombobox.Active);
            _configuration.AnswerType = TangoTypeUtil.ByIndex(_answerCombobox.Active);
            _configuration.StorageDir = _dbDirectoryEntry.Text.Trim();
            _configuration.HintSpeed = Convert.ToInt32(_hintScale.Value);
            _configuration.AutoModeSpeed = Convert.ToInt32(_autoModeScale.Value);
            
            ConfigurationService.Save(_configuration, false);
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