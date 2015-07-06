namespace KoreanOrbwalker
{
    using System;

    using LeagueSharp.Common;

    internal class Program
    {
        #region Methods

        private static void Game_OnGameLoad(EventArgs args)
        {
            new Orbwalker();
        }

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        #endregion
    }
}