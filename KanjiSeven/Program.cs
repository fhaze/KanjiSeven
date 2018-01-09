using Gtk;
using KanjiSeven.Views;

namespace KanjiSeven
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Application.Init();
            
            var mf = new MainForm();
            
            Application.Run();
        }
    }
}