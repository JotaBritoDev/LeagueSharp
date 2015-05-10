using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;
using Color = System.Drawing.Color;

namespace KoreanAnnie
{
    class AnnieDrawings
    {
        private Annie annie;

        public AnnieDrawings(Annie annie)
        {
            this.annie = annie;

            Drawing.OnDraw += DrawAvailableRange;
        }

        private void DrawAvailableRange(EventArgs args)
        {
            if (annie.GetParamBool("drawskillranges"))
            {
                if (annie.GetParamKeyBind("flashtibbers") && (annie.Spells.R.IsReady()) && (FlashSpell.Flash(annie.Player).IsReady) && (annie.CheckStun()))
                {
                    Render.Circle.DrawCircle(annie.Player.Position, annie.Spells.RFlash.Range, System.Drawing.Color.DarkGreen, 3);
                }
                else
                {
                    Render.Circle.DrawCircle(annie.Player.Position, annie.Spells.MaxRangeCombo, System.Drawing.Color.DarkGreen, 3);
                }

            }
        }

    }
}
