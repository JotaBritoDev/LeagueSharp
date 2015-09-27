namespace KoreanZed
{
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    class ZedDrawingSpells
    {
        private readonly ZedMenu zedMenu;

        private readonly ZedSpells zedSpells;

        public ZedDrawingSpells(ZedMenu zedMenu, ZedSpells zedSpells)
        {
            this.zedMenu = zedMenu;
            this.zedSpells = zedSpells;

            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Drawing_OnDraw(System.EventArgs args)
        {
            if (!zedMenu.GetParamBool("koreanzed.drawing.skillranges"))
            {
                return;
            }

            ZedSpell zedSpell = null;

            if (zedSpells.R.UseOnCombo && zedSpells.R.IsReady() && zedSpells.R.Instance.ToggleState == 0)
            {
                zedSpell = zedSpells.R;
            }
            else if (zedSpells.Q.UseOnCombo && zedSpells.Q.IsReady())
            {
                zedSpell = zedSpells.Q;
            }
            else if (zedSpells.E.UseOnCombo && zedSpells.E.IsReady())
            {
                zedSpell = zedSpells.E;
            }

            if (zedSpell != null)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, zedSpell.Range, Color.FromArgb(150, Color.DarkGreen), 5);
            }
        }
    }
}
