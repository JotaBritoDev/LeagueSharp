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
    class SmartE
    {
        private readonly Spell E;
        private readonly CommonChampion champion;

        public SmartE(CommonChampion champion)
        {
            E = champion.Spells.E;
            this.champion = champion;

            Orbwalking.BeforeAttack += SmartUse;
        }

        private void SmartUse(Orbwalking.BeforeAttackEventArgs args)
        {
            if (E.Instance.ToggleState == 2)
            {
                if ((args.Target is Obj_AI_Turret && champion.Player.CountEnemiesInRange(1000f) > 0)
                    || (champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit
                        || champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    || (champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                        && !KoreanUtils.GetParamBool(champion.MainMenu, "useetolaneclear")))
                {
                    E.Cast();
                }
            }
            else if (E.Instance.ToggleState < 2)
            {
                if ((args.Target is Obj_AI_Turret && champion.Player.CountEnemiesInRange(1000f) == 0)
                    || (champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                        && KoreanUtils.GetParamBool(champion.MainMenu, "useetolaneclear"))
                    || (champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo))
                {
                    E.Cast();
                }
            }
        }
    }
}
