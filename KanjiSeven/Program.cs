using System;
using Gtk;
using IniParser;
using KanjiSeven.Views;

namespace KanjiSeven
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Application.Init();
            ConfigManager.Init();
            
            var mf = new MainForm();
            
            Application.Run();
        }
    }
}