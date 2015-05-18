using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using KoreanCommon;

namespace KoreanChoGath
{
    class CancelAA
    {
        private readonly CommonChampion champion;
        private readonly Spell R;

        public CancelAA(CommonChampion champion)
        {
            R = champion.Spells.R;
            this.champion = champion;

            Orbwalking.BeforeAttack += CancelingAA;
        }

        private void CancelingAA(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target is Obj_AI_Hero) 
            {
                if (champion.Player.GetAutoAttackDamage((Obj_AI_Base)args.Target) > args.Target.Health + 20f)
                {
                    args.Process = true;
                }
                else if (R.IsReady() && R.IsKillable((Obj_AI_Hero)args.Target))
                {
                    args.Process = false;
                }
            }
        }
    }
}
