using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using KoreanCommon;

namespace KoreanLucian
{
    class KillSteal
    {
        private readonly CommonChampion champion;

        private readonly CommonSpells spells;

        public KillSteal(CommonChampion champion)
        {
            this.champion = champion;
            spells = champion.Spells;

            if (KoreanUtils.GetParamBool(champion.MainMenu, "killsteal"))
            {
                Game.OnUpdate += KS;
            }
        }

        public void KS(EventArgs args)
        {
            if (champion.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)
            {
                Obj_AI_Hero target;

                if (spells.W.IsReady())
                {
                    target = TargetSelector.GetTarget(spells.W.Range, TargetSelector.DamageType.Magical, false);

                    if (target != null && spells.W.IsKillable(target) && spells.W.CanCast(target)
                        && target.Distance(champion.Player) > Orbwalking.GetRealAutoAttackRange(champion.Player))
                    {
                        spells.W.Cast(target.Position);
                    }
                }
                if (spells.E.IsReady())
                {
                    target =
                        TargetSelector.GetTarget(
                            spells.E.Range + Orbwalking.GetRealAutoAttackRange(champion.Player),
                            TargetSelector.DamageType.Physical,
                            false);

                    if (target != null && champion.Player.GetAutoAttackDamage(target) * 1.5f > target.Health
                        && target.Distance(champion.Player) > Orbwalking.GetRealAutoAttackRange(champion.Player))
                    {
                        spells.E.Cast(target.Position);
                        champion.Orbwalker.ForceTarget(target);
                    }
                }
            }
        }
    }
}
