using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAkali
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName.ToLowerInvariant() == "akali")
            {
                var akali = new Akali();
            }
        }
    }
}