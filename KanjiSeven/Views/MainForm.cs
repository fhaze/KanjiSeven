using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Gdk;
using GLib;
using Gtk;
using KanjiSeven.Data;
using KanjiSeven.Extensions;
using KanjiSeven.Utils;
using KanjiSeven.Widgets;
using Key = Gdk.Key;
using Window = Gtk.Window;

namespace KanjiSeven.Views
{
    public class MainForm : Window
    {
        private readonly VBox        _mainVerticalBox = new VBox();
        private readonly Statusbar   _statusbar       = new Statusbar();
        private readonly Gtk.Image   _logo            = new Gtk.Image();
        
        private readonly Bitmap _background;
        private readonly string _version;

        private readonly AccelGroup _accelGroup = new AccelGroup();
        private readonly MenuBar    _menuBar = new MenuBar();
        private readonly Toolbar    _toolbar = new Toolbar { ToolbarStyle = ToolbarStyle.Icons };
        
        private TangoList     _tangoList;
        private ConfigForm    _configForm;
        private FlashCardForm _flashCardForm;
        private AboutDialog   _aboutDialog;
        
        public MainForm() : base("漢字七")
        {
            _background = Resources.ResourceManager.GetObject("background") as Bitmap;
            SetSizeRequest(900, 700);
            
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            _version = $"{version[0]}.{version[1]}";
           
            var vbox = new VBox { Spacing = 10 };
            
            if (PixbufUtil.TryFrom(Resources.ResourceManager.GetObject("logo"), out Pixbuf pixbuf))
                _logo.Pixbuf = pixbuf;
            
            AddAccelGroup(_accelGroup);
            
            //ファイル
            var fileMenu = new Menu();
            var file = new MenuItem("ファイル(_F)") { Submenu = fileMenu　};

            var game = new MenuItem("新しいゲーム…");
            game.Activated += OpenFlashCardGame;
            game.AddAccelerator("activate", _accelGroup, new AccelKey(Key.n, ModifierType.ControlMask, AccelFlags.Visible));
            fileMenu.Add(game);
            
            var settings = new MenuItem("設定…");
            settings.Activated += OpenConfiguration;
            settings.AddAccelerator("activate", _accelGroup, new AccelKey(Key.s, ModifierType.ControlMask | ModifierType.Mod1Mask, AccelFlags.Visible));
            fileMenu.Add(settings);
            
            fileMenu.Add(new SeparatorMenuItem());
            
            var exit = new MenuItem("終了");
            exit.Activated += Exit;
            exit.AddAccelerator("activate", _accelGroup, new AccelKey(Key.q, ModifierType.ControlMask, AccelFlags.Visible));
            fileMenu.Add(exit);

            _menuBar.Append(file);
            
            //単語
            var tangoMenu = new Menu();
            var tango = new MenuItem("単語(_T)") { Submenu = tangoMenu　};
            
            var registration = new MenuItem("登録…");
            registration.Activated += OpenTangoEditor;
            registration.AddAccelerator("activate", _accelGroup, new AccelKey(Key.r, ModifierType.ControlMask, AccelFlags.Visible));
            tangoMenu.Add(registration);
            
            _menuBar.Append(tango);
            
            //ヘルプ
            var helpMenu = new Menu();
            var help = new MenuItem("ヘルプ(_H)") {　Submenu = helpMenu　};
            
            var info = new MenuItem("バージョン情報…");
            info.Activated += About;
            info.AddAccelerator("activate", _accelGroup, new AccelKey(Key.F1, ModifierType.None, AccelFlags.Visible));
            helpMenu.Add(info);
            
            _menuBar.Append(help);
            _mainVerticalBox.PackStart(_menuBar, false, false, 0);

            var play = new ToolButton(Stock.MediaPlay);
            play.Clicked += OpenFlashCardGame;
            _toolbar.Insert(play, 0);
            
            var edit = new ToolButton(Stock.Edit);
            edit.Clicked += OpenTangoEditor;
            _toolbar.Insert(edit, 1);
            
            var preferences = new ToolButton(Stock.Preferences);
            preferences.Clicked += OpenConfiguration;
            _toolbar.Insert(preferences, 2);
            
            _toolbar.Insert(new SeparatorToolItem(), 3);
            
            var quit = new ToolButton(Stock.Quit);
            quit.Clicked += Exit;
            _toolbar.Insert(quit, 4);
            
            _mainVerticalBox.PackStart(_toolbar, false, true, 0);

            vbox.PackStart(new Label(), true, false, 0);
            vbox.PackStart(_logo, false, true, 0);
            vbox.PackStart(new Label("漢字七").SetFontSize(40).SetForegroundColor(0, 0, 0), false, false, 0);
            vbox.PackStart(new Label($"バージョン: {_version}").SetForegroundColor(255, 50, 50), false, false, 0);
            vbox.PackStart(new Label(), true, false, 0);

            _mainVerticalBox.PackStart(vbox, true, true, 0);
            _mainVerticalBox.PackStart(_statusbar, false, true, 0);
            Add(_mainVerticalBox);

            ExposeEvent += OnExposeEvent;
            
            ShowAll();
            GrabFocus();
        }

        [ConnectBefore]
        private void OnExposeEvent(object sender, ExposeEventArgs args)
        {
            if (_background != null && sender is Widget w)
                w.DrawResource(_background); 
        }


        private void OpenFlashCardGame(object sender, EventArgs eventArgs)
        {
            if (_flashCardForm == null || !_flashCardForm.Visible)
                _flashCardForm = new FlashCardForm(this);
        }

        private void OpenConfiguration(object sender, EventArgs eventArgs)
        {
            if (_configForm == null || !_configForm.Visible)
                _configForm = new ConfigForm(this);
        }

        private void OpenTangoEditor(object sender, EventArgs eventArgs)
        {
            if (_tangoList == null || !_tangoList.Visible)
                _tangoList = new TangoList(this);
        }

        private void Exit(object sender, EventArgs eventArgs)
        {
            Application.Quit();
        }

        private void About(object sender, EventArgs eventArgs)
        {
            if (_aboutDialog == null || !_aboutDialog.Visible)
            {
                var ab = new AboutDialog
                {
                    Website = "https://github.com/fhaze",
                    ProgramName = "漢字七",
                    Authors = new[] {"FHaze aka 松本エデル"},
                    Version = _version
                };
                ab.Run();
                ab.Destroy();

                _aboutDialog = ab;
            }
        }
        
        protected override void OnDestroyed()
        {
            LocalContext.Current.Conn?.Close();
            Application.Quit();
        }
    }
}