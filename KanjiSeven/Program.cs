using System;
using Gtk;
using IniParser;
using KanjiSeven.Services;
using KanjiSeven.Views;

namespace KanjiSeven
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Application.Init();
            ConfigurationService.Init();
            
            var mf = new MainForm();
            
            Application.Run();
        }
    }
}