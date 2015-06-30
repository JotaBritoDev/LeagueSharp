using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D9;

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
            if (KoreanUtils.GetParamBool(champion.MainMenu, "useqtofarm") && Q.IsReady())
            {
                Obj_AI_Base target =
                MinionManager.GetMinions(Q.Range).OrderByDescending(minion => minion.MaxHealth)
                .FirstOrDefault(minion =>
                    Q.GetDamage(minion) >
                    HealthPrediction.GetHealthPrediction(minion,
                        (int)(champion.Player.Distance(minion) / Q.Speed) * 1000, (int)Q.Delay * 1000));

                if (target != null)
                {
                    Q.Cast(target);
                }
            }

            if (E.IsReady() && !Q.IsReady())
            {
                int farmCount = MinionManager.GetMinions(E.Range).Count(minion => E.IsKillable(minion));

                if (farmCount >= 2)
                {
                    E.Cast();
                }
            }
        }

        public override void HarasMode()
        {
            Obj_AI_Hero target;

            if (Q.IsReady() && Q.UseOnHaras)
            {
                target = TargetSelector.GetTarget(Q.Range, Q.DamageType);

                if (target != null)
                {
                    Q.Cast(target);
                }
            }

            if (E.IsReady() && E.UseOnHaras)
            {
                target = TargetSelector.GetTarget(E.Range, E.DamageType);

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

                if (E.IsReady())
                {
                    if (MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral).Count > 0)
                    {
                        E.Cast();
                    }
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

            if (Q.IsReady() && Q.UseOnLaneClear)
            {
                Obj_AI_Base jungleMob =
                    MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                        .FirstOrDefault();

                if (jungleMob != null)
                {
                    Q.Cast(jungleMob);
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

            if (R.IsReady() && R.UseOnCombo)
            {
                int minEnemiesToR = KoreanUtils.GetParamSlider(champion.MainMenu, "minenemiestor");

                if (minEnemiesToR == 1)
                {
                    target = TargetSelector.GetTarget(R.Range, R.DamageType);
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

                if (R.IsReady() && ((Q.UseOnCombo && Q.IsReady()) || !Q.UseOnCombo))
                {
                    target = TargetSelector.GetTarget(R.Range, R.DamageType);

                    if (target != null)
                    {
                        float totalDamage = R.GetDamage(target) + Q.GetDamage(target)*1.12f + E.GetDamage(target) * 1.25f * (E.Instance.Ammo + 1) * 1.12f;

                        if (totalDamage > target.Health && totalDamage*0.7f < target.Health)
                        {
                            R.Cast(target.Position);
                        }
                    }
                }
            }

            if (E.IsReady() && E.UseOnCombo)
            {
                target = TargetSelector.GetTarget(E.Range, E.DamageType);

                if (target != null)
                {
                    E.Cast();
                }
            }

            if (Q.IsReady() && Q.UseOnCombo)
            {
                target = TargetSelector.GetTarget(Q.Range, Q.DamageType);

                if (target != null)
                {
                    Q.Cast(target);
                }
            }

            if (W.IsReady() && W.UseOnCombo)
            {
                if (HeroManager.Enemies.Any(enemy => enemy.Distance(champion.Player) < 180 && !enemy.IsDead))
                {
                    W.Cast();
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
