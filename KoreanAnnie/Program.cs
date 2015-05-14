namespace KoreanAnnie
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName.ToLowerInvariant() == "annie")
            {
                Annie annie = new Annie();
            }
        }
    }
}
