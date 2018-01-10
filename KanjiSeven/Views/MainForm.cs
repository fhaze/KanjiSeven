using System;
using System.Threading.Tasks;
using GLib;
using Gtk;
using KanjiSeven.Data;

namespace KanjiSeven.Views
{
    public class MainForm : Window
    {
        private readonly VBox       _mainVerticalBox = new VBox();
        private readonly Statusbar  _statusbar       = new Statusbar();
        private readonly VButtonBox _mainVButtonBox  = new VButtonBox
            { Layout = ButtonBoxStyle.Center, Spacing = 10 };
        
        private readonly Button _startButton  = new Button { Label = "ゲーム"};
        private readonly Button _kotobaButton = new Button { Label = "言葉を登録" };
        private readonly Button _configButton = new Button { Label = "設定" };
        private readonly Button _exitButton   = new Button { Label = "終了"};
        
        private readonly LocalContext _context = LocalContext.Current;
        
        public MainForm() : base("Kanji Seven")
        {            
            Resize(250, 175);
            
            _mainVButtonBox.Add(_startButton);
            _mainVButtonBox.Add(_kotobaButton);
            _mainVButtonBox.Add(_configButton);
            _mainVButtonBox.Add(_exitButton);
            
            _mainVerticalBox.PackStart(_mainVButtonBox);
            _mainVerticalBox.PackStart(_statusbar, false, true, 0);
            Add(_mainVerticalBox);
            
            _startButton.Clicked += StartButtonOnClicked;
            _kotobaButton.Clicked += KanjiButtonOnClicked;
            _configButton.Clicked += ConfigButtonOnClicked;
            _exitButton.Clicked += ExitButtonOnClicked;
            
            ShowAll();
            GrabFocus();
        }

        private void StartButtonOnClicked(object sender, EventArgs eventArgs)
        {
            new FlashCardForm(this);
        }

        private void ConfigButtonOnClicked(object sender, EventArgs eventArgs)
        {
            new ConfigForm(this);
        }

        private void ExitButtonOnClicked(object sender, EventArgs eventArgs)
        {
            Application.Quit();
        }

        private void KanjiButtonOnClicked(object sender, EventArgs eventArgs)
        {
            new KotobaList(this);
        }

        protected override void OnDestroyed()
        {
            LocalContext.Current.Conn?.Close();
            Application.Quit();
        }
    }
}