using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using KoreanCommon;
using SharpDX;
using Color = System.Drawing.Color;

namespace KoreanChoGath
{

    class Drawing
    {
        private CommonChampion champion { get; set; }
        private CommonSpells spells { get; set; }

        public Drawing(CommonChampion champion)
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

            if (KoreanUtils.GetParamKeyBind(champion.MainMenu, "flashult") && (spells.R.IsReady())
                && (FlashSpell.Flash(champion.Player).IsReady))
            {
                Render.Circle.DrawCircle(champion.Player.Position, champion.Spells.RFlash.Range, Color.DarkGreen, 5);
            }
            else if (spells.Q.IsReady() && spells.Q.CanCast())
            {
                Render.Circle.DrawCircle(champion.Player.Position, spells.Q.Range, Color.DarkGreen, 5);
            }
            else if (spells.W.IsReady() && spells.W.CanCast())
            {
                Render.Circle.DrawCircle(champion.Player.Position, spells.W.Range, Color.DarkGreen, 5);
            }
            else if (spells.R.IsReady() && spells.R.CanCast())
            {
                Render.Circle.DrawCircle(champion.Player.Position, spells.R.Range, Color.DarkGreen, 5);
            }
        }
    }
}
