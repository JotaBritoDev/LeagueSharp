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

namespace KoreanLucian
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

            Render.Circle.DrawCircle(champion.Player.Position, spells.MaxRangeCombo, Color.FromArgb(150, Color.DarkGreen), 10);
        }
    }
}

