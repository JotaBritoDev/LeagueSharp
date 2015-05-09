using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    abstract class CommonOrbwalkComplementation
    {
        protected CommonChampion champion { get; set; }

        public abstract void LastHitMode();
        public abstract void HarasMode();
        public abstract void LaneClearMode();
        public abstract void ComboMode();
        public abstract void Ultimate();

        public void UseSkills(EventArgs args)
        {
            if (champion != null)
            {
                switch (champion.MainMenu.Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.LastHit:
                        LastHitMode();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        HarasMode();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        LaneClearMode();
                        break;
                    case Orbwalking.OrbwalkingMode.Combo:
                        ComboMode();
                        break;
                }
            }
        }
    }
}
