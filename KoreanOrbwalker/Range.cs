namespace KoreanOrbwalker
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Range
    {
        #region Fields

        private readonly Orbwalker orbwalker;

        #endregion

        #region Constructors and Destructors

        public Range(Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        #endregion

        #region Methods

        private void Drawing_OnDraw(EventArgs args)
        {
            if (orbwalker.CustomMenu.RangesActive)
            {
                Obj_AI_Base player = ObjectManager.Player;
                var range = player.BoundingRadius + player.AttackRange;
                Render.Circle.DrawCircle(player.Position, range, Color.Black, 5);
            }
        }

        #endregion
    }
}