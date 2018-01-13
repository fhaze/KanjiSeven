using System;
using System.Collections.Generic;
using KanjiSeven.Extensions;
using KanjiSeven.Models;

namespace KanjiSeven.Utils
{
    public static class TangoTypeUtil
    {
        public static string[] LabelList()
        {
            var list = new List<string>();
        
            foreach (var value in Enum.GetNames(typeof(TangoType)))
            {
                if (Enum.TryParse(value, out TangoType tangoType))
                    list.Add(tangoType.ToLabel());
            }

            return list.ToArray();
        }
        
        public static TangoType ByIndex(int index)
        {
            if (Enum.TryParse(Enum.GetNames(typeof(TangoType))[index], out TangoType tangoType))
                return tangoType;
            
            throw new ArgumentOutOfRangeException(nameof(tangoType), tangoType, null);
        }
    }
}