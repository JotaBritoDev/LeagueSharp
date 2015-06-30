using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanVladimir
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using KoreanCommon;
    using Color = System.Drawing.Color;

    class Draws
    {
        private CommonChampion champion { get; set; }
        private CommonSpells spells { get; set; }

        public Draws(CommonChampion champion)
        {
            this.champion = champion;
            spells = champion.Spells;

            LeagueSharp.Drawing.OnDraw += DrawRanges;
        }

        private void DrawRanges(EventArgs args)
        {
            if (!KoreanUtils.GetParamBool(champion.MainMenu, "drawskillranges"))
            {
                return;
            }

            if (spells.R.IsReady() && spells.R.CanCast())
            {
                Render.Circle.DrawCircle(champion.Player.Position, spells.R.Range, Color.DarkGreen, 5);
            }
            else if (spells.Q.IsReady() && spells.Q.CanCast())
            {
                Render.Circle.DrawCircle(champion.Player.Position, spells.Q.Range, Color.DarkGreen, 5);
            }
            else if (spells.E.IsReady() && spells.E.CanCast())
            {
                Render.Circle.DrawCircle(champion.Player.Position, spells.E.Range, Color.DarkGreen, 5);
            }
        }
    }
}
