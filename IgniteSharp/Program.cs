namespace IgniteSharp
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += eventArgs => { Ignite inite = new Ignite(); };
        }
    }
}
