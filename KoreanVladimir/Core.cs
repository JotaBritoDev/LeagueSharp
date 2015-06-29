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

    class Core : CommonCore
    {
        public Core(CommonChampion champion) : base(champion)
        {
        }

        public override void LastHitMode()
        {
            if (!KoreanUtils.GetParamBool(champion.MainMenu, "useqtofarm"))
            {
                return;
            }

            if (!Q.IsReady())
            {
                return;
            }

            Obj_AI_Base target =
            MinionManager.GetMinions(Q.Range).OrderByDescending(minion => minion.MaxHealth)
            .FirstOrDefault(minion =>
                Q.GetDamage(minion) >
                HealthPrediction.GetHealthPrediction(minion,
                    (int) (champion.Player.Distance(minion)/Q.Speed)*1000, (int) Q.Delay*1000));

            if (target != null)
            {
                Q.Cast(target);
            }
        }

        public override void HarasMode()
        {
            Obj_AI_Hero target;

            if (Q.IsReady() && Q.UseOnHaras)
            {
                target = TargetSelector.GetTarget(Q.Range, Q.DamageType, false);

                if (target != null)
                {
                    Q.Cast(target);
                }
            }

            if (E.IsReady() && E.UseOnHaras)
            {
                target = TargetSelector.GetTarget(E.Range, E.DamageType, false);

                if (target != null)
                {
                    E.Cast();
                }
            }

            LastHitMode();
        }

        public override void LaneClearMode()
        {
            if (E.IsReady() && E.UseOnLaneClear)
            {
                if (MinionManager.GetMinions(E.Range).Count >= KoreanUtils.GetParamSlider(champion.MainMenu, "minminionstoe"))
                {
                    E.Cast();
                }
            }

            if (KoreanUtils.GetParamBool(champion.MainMenu, "saveqtofarm") &&
                KoreanUtils.GetParamBool(champion.MainMenu, "useqtofarm"))
            {
                LastHitMode();
            }
            else if (Q.IsReady() && Q.UseOnLaneClear)
            {
                Obj_AI_Base minion =
                    MinionManager.GetMinions(Q.Range).OrderByDescending(x => x.MaxHealth).FirstOrDefault();

                if (minion != null)
                {
                    Q.Cast(minion);
                }
            }

            if (KoreanUtils.GetParamBool(champion.MainMenu, "harasonlaneclear"))
            {
                HarasMode();
            }
        }

        public override void ComboMode()
        {
            Obj_AI_Hero target;

            if (E.IsReady() && E.UseOnCombo)
            {
                target = TargetSelector.GetTarget(E.Range, E.DamageType, false);

                if (target != null)
                {
                    E.Cast();
                }
            }

            if (Q.IsReady() && Q.UseOnCombo)
            {
                target = TargetSelector.GetTarget(Q.Range, Q.DamageType, false);

                if (target != null)
                {
                    Q.Cast(target);
                }
            }

            if (R.IsReady() && R.UseOnCombo)
            {
                int minEnemiesToR = KoreanUtils.GetParamSlider(champion.MainMenu, "minenemiestor");

                if (minEnemiesToR == 1)
                {
                    target = TargetSelector.GetTarget(R.Range, R.DamageType, false);
                    if (target != null)
                    {
                        spells.R.Cast(target.Position);
                    }
                }
                else
                {
                    foreach (PredictionOutput pred in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(x => x.IsValidTarget(spells.R.Range))
                            .Select(x => spells.R.GetPrediction(x, true))
                            .Where(pred => pred.Hitchance >= HitChance.High && pred.AoeTargetsHitCount >= minEnemiesToR)
                        )
                    {
                        spells.R.Cast(pred.CastPosition);
                    }
                }
            }
        }

        public override void Ultimate(Obj_AI_Hero target)
        {
            if (target != null)
            {
                R.Cast(target.Position);
            }
            else
            {
                R.CastOnBestTarget();
            }
        }
    }
}
