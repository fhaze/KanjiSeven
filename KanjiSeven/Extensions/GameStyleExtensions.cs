using System;
using KanjiSeven.Models;

namespace KanjiSeven.Extensions
{
    public static class GameStyleExtensions
    {
        public static string Label(this GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Simple:
                    return "なじ";
                case GameMode.GuessMode:
                    return "推測";
                case GameMode.InputMode:
                    return "入力";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }
        
    }
}