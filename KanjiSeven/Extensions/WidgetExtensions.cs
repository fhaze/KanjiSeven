﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Gdk;
using Gtk;
using GC = Gdk.GC;

namespace KanjiSeven.Extensions
{
    public static class WidgetExtensions
    {
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

        public static Widget SetFontSize(this Widget widget, int size)
        {
            var fontDescription = widget.PangoContext.FontDescription;
            fontDescription.Size = Convert.ToInt32(size * Pango.Scale.PangoScale);
            widget.ModifyFont(fontDescription);
            return widget;
        }

        public static Widget SetBackgroundColor(this Widget widget, StateType stateType, Gdk.Color color)
        {
            widget.ModifyBg(stateType, color);
            return widget;
        }

        public static Widget SetBackgroundColor(this Widget widget, Gdk.Color color)
        {
            SetBackgroundColor(widget, StateType.Normal, color);
            SetBackgroundColor(widget, StateType.Active, color);
            SetBackgroundColor(widget, StateType.Insensitive, color);
            SetBackgroundColor(widget, StateType.Prelight, color);
            SetBackgroundColor(widget, StateType.Selected, color);
            return widget;
        }
        
        public static Widget SetBackgroundColor(this Widget widget, StateType stateType, byte r, byte g, byte b)
        {
            SetBackgroundColor(widget, stateType, new Gdk.Color(r, g, b));
            return widget;
        }
        
        public static Widget SetBackgroundColor(this Widget widget, byte r, byte g, byte b)
        {
            var color = new Gdk.Color(r, g, b);
            SetBackgroundColor(widget, StateType.Normal, color);
            SetBackgroundColor(widget, StateType.Active, color);
            SetBackgroundColor(widget, StateType.Insensitive, color);
            SetBackgroundColor(widget, StateType.Prelight, color);
            SetBackgroundColor(widget, StateType.Selected, color);
            return widget;
        }
        
        public static Widget SetBaseColor(this Widget widget, StateType stateType, Gdk.Color color)
        {
            widget.ModifyBase(stateType, color);
            return widget;
        }

        public static Widget SetBaseColor(this Widget widget, Gdk.Color color)
        {
            SetBaseColor(widget, StateType.Normal, color);
            SetBaseColor(widget, StateType.Active, color);
            SetBaseColor(widget, StateType.Insensitive, color);
            SetBaseColor(widget, StateType.Prelight, color);
            SetBaseColor(widget, StateType.Selected, color);
            return widget;
        }
        
        public static Widget SetBaseColor(this Widget widget, StateType stateType, byte r, byte g, byte b)
        {
            SetBaseColor(widget, stateType, new Gdk.Color(r, g, b));
            return widget;
        }
        
        public static Widget SetBaseColor(this Widget widget, byte r, byte g, byte b)
        {
            var color = new Gdk.Color(r, g, b);
            SetBaseColor(widget, StateType.Normal, color);
            SetBaseColor(widget, StateType.Active, color);
            SetBaseColor(widget, StateType.Insensitive, color);
            SetBaseColor(widget, StateType.Prelight, color);
            SetBaseColor(widget, StateType.Selected, color);
            return widget;
        }
        
        public static Widget SetForegroundColor(this Widget widget, StateType stateType, Gdk.Color color)
        {
            widget.ModifyFg(stateType, color);
            return widget;
        }

        public static Widget SetForegroundColor(this Widget widget, Gdk.Color color)
        {
            SetForegroundColor(widget, StateType.Normal, color);
            SetForegroundColor(widget, StateType.Active, color);
            SetForegroundColor(widget, StateType.Insensitive, color);
            SetForegroundColor(widget, StateType.Prelight, color);
            SetForegroundColor(widget, StateType.Selected, color);
            return widget;
        }
        
        public static Widget SetForegroundColor(this Widget widget, StateType stateType, byte r, byte g, byte b)
        {
            SetForegroundColor(widget, stateType, new Gdk.Color(r, g, b));
            return widget;
        }
        
        public static Widget SetForegroundColor(this Widget widget, byte r, byte g, byte b)
        {
            var color = new Gdk.Color(r, g, b);
            SetForegroundColor(widget, StateType.Normal, color);
            SetForegroundColor(widget, StateType.Active, color);
            SetForegroundColor(widget, StateType.Insensitive, color);
            SetForegroundColor(widget, StateType.Prelight, color);
            SetForegroundColor(widget, StateType.Selected, color);
            return widget;
        }
    }
}