using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Gdk;

namespace KanjiSeven.Utils
{
    public static class PixbufUtil
    {
        public static bool TryFrom(object obj, out Pixbuf pixbuf)
        {
            if (obj is Bitmap bmp)
            {
                using (var stream = new MemoryStream())
                {
                    bmp.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    pixbuf = new Pixbuf(stream);
                    return true;
                }
            }
            pixbuf = null;
            return false;
        }
    }
}