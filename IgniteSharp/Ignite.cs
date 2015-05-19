namespace IgniteSharp
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class Ignite
    {
        public readonly Spell SummonerDot;

        public readonly CustomMenu menu;

        public readonly AutoKill AutoKill;

        private CastOnBestTarget castOnBestTarget;

        public Ignite()
        {
            SummonerDot = new Spell(ObjectManager.Player.GetSpellSlot("SummonerDot"), 600);
            SummonerDot.SetTargetted(0.1f, float.MaxValue);

            menu = new CustomMenu(this);
            castOnBestTarget = new CastOnBestTarget(this);
            AutoKill = new AutoKill(this);
        }
    }
}