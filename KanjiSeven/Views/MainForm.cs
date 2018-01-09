using System;
using GLib;
using Gtk;

namespace KanjiSeven.Views
{
    public class MainForm : Window
    {
        private readonly VBox       _mainVerticalBox = new VBox();
        private readonly Statusbar  _statusbar       = new Statusbar();
        private readonly VButtonBox _mainVButtonBox  = new VButtonBox
            { Layout = ButtonBoxStyle.Center, Spacing = 10 };
        
        private readonly Button _startButton = new Button { Label = "スタート"};
        private readonly Button _kanjiButton = new Button { Label = "漢字を登録" };
        private readonly Button _exitButton  = new Button { Label = "終了"};
        
        public MainForm() : base("Kanji Seven")
        {
            Resize(300, 400);
            
            _mainVButtonBox.Add(_startButton);
            _mainVButtonBox.Add(_kanjiButton);
            _mainVButtonBox.Add(_exitButton);
            
            _mainVerticalBox.PackStart(_mainVButtonBox);
            _mainVerticalBox.PackStart(_statusbar, false, true, 0);
            Add(_mainVerticalBox);
            
            CreateListeners();
            ShowAll();
        }

        private void CreateListeners()
        {
            _kanjiButton.Clicked += KanjiButtonOnClicked;
        }

        private void KanjiButtonOnClicked(object sender, EventArgs eventArgs)
        {
            new KanjiEditor(this);
        }
    }
}