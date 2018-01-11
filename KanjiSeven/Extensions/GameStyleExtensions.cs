using System;
using KanjiSeven.Models;

namespace KanjiSeven.Extensions
{
    public static class GameStyleExtensions
    {
        public static string Label(this GameStyle gameStyle)
        {
            switch (gameStyle)
            {
                case GameStyle.Simple:
                    return "なじ";
                case GameStyle.GuessMode:
                    return "推測";
                case GameStyle.InputMode:
                    return "入力";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameStyle), gameStyle, null);
            }
        }
        
    }
}