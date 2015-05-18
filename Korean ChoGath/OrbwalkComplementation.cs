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
    using SharpDX.Direct3D9;

    class OrbwalkComplementation : CommonOrbwalkComplementation
    {
        public OrbwalkComplementation(CommonChampion champion) : base(champion)
        {

        }

        private void useQ(Obj_AI_Hero target)
        {
            if (!spells.Q.IsReadyToCastOn(target))
            {
                return;
            }

            var pred = Q.GetPrediction(target);

            if (pred.Hitchance >= HitChance.VeryHigh)
            {
                Q.Cast(target);
            }
        }

        private void useW(Obj_AI_Hero target)
        {
            if (!spells.W.IsReadyToCastOn(target))
            {
                return;
            }

            var pred = W.GetPrediction(target);

            if (pred.Hitchance >= HitChance.High && W.IsInRange(pred.CastPosition))
            {
                W.Cast(pred.CastPosition);
            }
        }

        private bool haveManaToHaras()
        {
            return champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "manalimittoharas");
        }

        private bool haveManaToLaneclear()
        {
            return champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "manalimittolaneclear");
        }

        public override void LastHitMode()
        {
            //
        }

        public override void HarasMode()
        {
            if (!haveManaToHaras())
            {
                return;
            }

            Obj_AI_Hero target = TargetSelector.GetTarget(champion.Spells.MaxRangeHaras, TargetSelector.DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (Q.UseOnHaras)
            {
                useQ(target);
            }

            if (!haveManaToHaras())
            {
                return;
            }

            if (W.UseOnHaras)
            {
                useW(target);
            }
        }

        public override void LaneClearMode()
        {
            if (!haveManaToLaneclear())
            {
                return;
            }
            if (spells.Q.UseOnLaneClear && spells.Q.IsReady() && spells.Q.CanCast())
            {
                List<Obj_AI_Base> minions = MinionManager.GetMinions(champion.Player.Position, spells.Q.Range);
                MinionManager.FarmLocation farmLocation = spells.Q.GetCircularFarmLocation(
                    minions,
                    spells.Q.Width);
                if (farmLocation.MinionsHit >= KoreanUtils.GetParamSlider(champion.MainMenu, "minminionstoq"))
                {
                    spells.Q.Cast(farmLocation.Position);
                }
            }

            if (!haveManaToLaneclear())
            {
                return;
            }

            if (spells.W.UseOnLaneClear && spells.W.IsReady() && spells.W.CanCast())
            {
                List<Obj_AI_Base> minions = MinionManager.GetMinions(champion.Player.Position, spells.W.Range);

                MinionManager.FarmLocation wFarmLocation = spells.W.GetCircularFarmLocation(
                    minions,
                    spells.W.Width);

                if (wFarmLocation.MinionsHit >= KoreanUtils.GetParamSlider(champion.MainMenu, "minminionstow"))
                {
                    spells.W.Cast(wFarmLocation.Position);
                }
            }

            if (KoreanUtils.GetParamBool(champion.MainMenu, "harasonlaneclear"))
            {
                HarasMode();
            }
        }



        public override void ComboMode()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(champion.Spells.MaxRangeHaras, TargetSelector.DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (Q.UseOnCombo)
            {
                useQ(target);
            }

            if (W.UseOnCombo)
            {
                useW(target);
            }

            if (R.IsKillable(target))
            {
                Ultimate(target);
            }
        }

        public override void Ultimate(Obj_AI_Hero target = null)
        {
            if (!R.IsReady())
            {
                return;
            }

            Obj_AI_Hero championTargeted;
            if (target != null)
            {
                championTargeted = target;
            }
            else
            {
                championTargeted = TargetSelector.GetTarget(spells.R.Range, TargetSelector.DamageType.Magical);
            }

            if (championTargeted != null)
            {
                R.Cast(championTargeted);
            }
        }
    }
}
