using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using GLib;
using Gtk;
using KanjiSeven.Data;
using KanjiSeven.Extensions;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class MainForm : Window
    {
        private readonly Table       _table           = new Table(1, 3, true);
        private readonly VBox        _mainVerticalBox = new VBox();
        private readonly Statusbar   _statusbar       = new Statusbar();
        private readonly VButtonBox  _mainVButtonBox  = new VButtonBox
            { Layout = ButtonBoxStyle.Center, Spacing = 10 };
        
        private readonly Button _startButton  = new Button { Label = "スタート"};
        private readonly Button _kotobaButton = new Button { Label = "言葉を登録" };
        private readonly Button _configButton = new Button { Label = "設定" };
        private readonly Button _exitButton   = new Button { Label = "終了"};
        private readonly Bitmap _background;
        private readonly string _version;
        
        public MainForm() : base("漢字七")
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            _version = $"{version[0]}.{version[1]}";
            
            _background = Resources.ResourceManager.GetObject("background") as Bitmap;
            if (_background != null)
                SetSizeRequest(_background.Width, _background.Height);
            else
                SetSizeRequest(520, 400);
            
            Resizable = false;
            
            _mainVButtonBox.Add(_startButton);
            _mainVButtonBox.Add(_kotobaButton);
            _mainVButtonBox.Add(_configButton);
            _mainVButtonBox.Add(_exitButton);
           
            var vbox = new VBox { Spacing = 10 };
            vbox.PackStart(new Label() { HeightRequest = 120 }, false, true, 0);
            vbox.PackStart(new Label("漢字七").SetFontSize(50).SetForegroundColor(255, 255, 255), false, false, 0);
            vbox.PackStart(new Label($"version: {_version}").SetForegroundColor(255, 120, 120), false, false, 0);
            
            _table.Attach(vbox, 0, 1, 0, 1);
            _table.Attach(_mainVButtonBox, 2, 3, 0, 1);
            _mainVerticalBox.PackStart(_table);
            _mainVerticalBox.PackStart(_statusbar, false, true, 0);
            Add(_mainVerticalBox);
            
            _startButton.Clicked += StartButtonOnClicked;
            _kotobaButton.Clicked += KanjiButtonOnClicked;
            _configButton.Clicked += ConfigButtonOnClicked;
            _exitButton.Clicked += ExitButtonOnClicked;

            _mainVerticalBox.ExposeEvent += OnExposeEvent;
            
            ShowAll();
            GrabFocus();
        }

        [ConnectBefore]
        private void OnExposeEvent(object sender, ExposeEventArgs args)
        {
            if (_background != null && sender is Widget w)
                w.DrawResource(_background);
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