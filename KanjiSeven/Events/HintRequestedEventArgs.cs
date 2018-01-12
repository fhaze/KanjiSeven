using System;
using KanjiSeven.Data.Entities;

namespace KanjiSeven.Events
{
    public class HintRequestedEventArgs : EventArgs
    {
        public Tango Tango { get; set; }
    }
}