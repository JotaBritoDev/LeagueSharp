namespace IgniteSharp
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class Drawings
    {
        private readonly Spell ignite;

        public Drawings(Ignite obj)
        {
            ignite = obj.SummonerDot;

            if (obj.menu.Get().Item("ignite#range").GetValue<bool>())
            {
                Drawing.OnDraw += IgniteRange;
            }
        }

        public void IgniteRange(EventArgs args)
        {
            if (ignite.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, ignite.Range, Color.FromArgb(40, Color.Red), 10);
            }
        }
    }
}