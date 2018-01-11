using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Gdk;
using Gtk;
using Window = Gtk.Window;

namespace KanjiSeven.Extensions
{
    public static class WidgetExtensions
    {
        public static void DrawResource(this Window window, object obj)
        {
            if (obj is Bitmap bmp)
            {
                using (var stream = new MemoryStream())
                {
                    bmp.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    window.GdkWindow.DrawPixbuf(new GC(window.GdkWindow), new Pixbuf(stream), 0, 0, 0, 0, bmp.Width,
                        bmp.Height,
                        RgbDither.None, 0, 0);   

                }
            }
        }
        
        public static void DrawResource(this Widget widget, object obj)
        {
            if (obj is Bitmap bmp)
            {
                using (var stream = new MemoryStream())
                {
                    bmp.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    widget.GdkWindow.DrawPixbuf(new GC(widget.GdkWindow), new Pixbuf(stream), 0, 0, 0, 0, bmp.Width,
                        bmp.Height,
                        RgbDither.None, 0, 0);
                }
            }
        }
    }
}