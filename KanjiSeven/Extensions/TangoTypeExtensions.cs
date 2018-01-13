using System;
using KanjiSeven.Models;

namespace KanjiSeven.Extensions
{
    public static class TangoTypeExtensions
    {
        public static string ToLabel(this TangoType tangoType)
        {
            switch (tangoType)
            {
                case TangoType.Kanji:
                    return "漢字";
                case TangoType.Hiragana:
                    return "ひらがな";
                case TangoType.Romaji:
                    return "ローマ字";
                case TangoType.Honyaku:
                    return "翻訳";
                default:
                    throw new ArgumentOutOfRangeException(nameof(tangoType), tangoType, null);
            }
        }
        
        public static int Index(this TangoType tangoType)
        {
            var index = 0;
            
            foreach (var value in Enum.GetNames(typeof(TangoType)))
            {
                if (Enum.TryParse(value, out TangoType parsedTangoType))
                {
                    if (tangoType == parsedTangoType)
                        return index;

                    index++;
                }
            }
            
            throw new ArgumentOutOfRangeException(nameof(tangoType), tangoType, null);
        }
    }
}