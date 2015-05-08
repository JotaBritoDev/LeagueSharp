using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    class AnnieOrbwalkComplementation : CommonOrbwalkComplementation
    {
        private Annie annie;
        private AnnieSpells Spells;

        public AnnieOrbwalkComplementation(Annie annie)
        {
            champion = annie;
            this.annie = annie;
            Spells = annie.Spells;

            Orbwalking.BeforeAttack += CancelingAAOnSupportMode;
            Game.OnUpdate += UseSkills;
        }

        public override void LastHitMode()
        {
            if (!annie.SaveStun())
            {
                QFarmLogic();
            }
        }

        public override void HarasMode()
        {
            LastHitMode();
            Haras();
        }

        public override void LaneClearMode()
        {
            if ((!annie.SaveStun()) && (annie.CanFarm()))
            {
                bool manaLimitReached = annie.Player.ManaPercent < annie.GetParamSlider("manalimittolaneclear");

                if ((annie.GetParamBool("useqtolaneclear")) && (Spells.Q.IsReady()))
                {
                    if (annie.GetParamBool("saveqtofarm"))
                    {
                        QFarmLogic();
                    }
                    else if (!manaLimitReached)
                    {
                        List<Obj_AI_Base> minions = MinionManager.GetMinions(annie.Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);

                        if ((minions != null) && (minions.Count > 0))
                        {
                            Spells.Q.Cast(minions[0]);
                        }
                    }
                }

                if (!manaLimitReached)
                {
                    if ((annie.GetParamBool("usewtolaneclear")) && (Spells.W.IsReady()))
                    {
                        List<Obj_AI_Base> Minions = MinionManager.GetMinions(annie.Player.Position, Spells.W.Range);

                        MinionManager.FarmLocation WFarmLocation = Spells.W.GetCircularFarmLocation(Minions, Spells.W.Width);

                        if (WFarmLocation.MinionsHit >= annie.GetParamSlider("minminionstow"))
                        {
                            Spells.W.Cast(WFarmLocation.Position);
                        }
                    }
                }
            }

            if (annie.GetParamBool("harasonlaneclear"))
            {
                Haras();
            }
        }

        public override void ComboMode()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Spells.MaxRangeForCombo(), TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if ((Spells.R.IsReady()) && (annie.GetParamBool("usertocombo")) && (target.IsValidTarget(Spells.R.Range)) && (!Spells.CheckOverkill(target)))
            {
                int minEnemiesToR = annie.GetParamSlider("minenemiestor");

                if (minEnemiesToR == 1)
                {
                    Spells.R.Cast(target.Position);
                }
                else
                {
                    foreach (PredictionOutput pred in ObjectManager.Get<Obj_AI_Hero>().
                        Where(x => x.IsValidTarget(Spells.R.Range)).
                        Select(x => Spells.R.GetPrediction(x, true)).
                            Where(pred => pred.Hitchance >= HitChance.High && pred.AoeTargetsHitCount >= minEnemiesToR))
                    {
                        Spells.R.Cast(pred.CastPosition);
                    }
                }
            }
            if ((Spells.W.IsReady()) && (annie.GetParamBool("usewtocombo")) && (target.IsValidTarget(Spells.W.Range)))
            {
                Spells.W.Cast(target.Position);
            }
            if ((Spells.Q.IsReady()) && (annie.GetParamBool("useqtocombo")) && (target.IsValidTarget(Spells.Q.Range)))
            {
                Spells.Q.Cast(target);
            }
            if ((!annie.GetParamBool("supportmode")) && (Spells.R.GetDamage(target) > target.Health * 1.02f) && (!Spells.CheckOverkill(target)))
            {
                Ultimate();
            }
        }

        public override void Ultimate()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                Spells.R.Cast(target.Position);
            }
        }

        private void Haras()
        {
            bool manaLimitReached = annie.Player.ManaPercent < annie.GetParamSlider("manalimittoharas");
            if (!manaLimitReached)
            {
                Obj_AI_Hero target = TargetSelector.GetTarget(Spells.MaxRangeForHaras(), TargetSelector.DamageType.Magical);

                if ((Spells.Q.IsReady()) && (annie.GetParamBool("useqtoharas")) && (target.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.Cast(target);
                }

                if ((Spells.W.IsReady()) && (annie.GetParamBool("usewtoharas")) && (target.IsValidTarget(Spells.W.Range)))
                {
                    Spells.W.Cast(target.Position);
                }
            }
        }

        private void QFarmLogic()
        {
            if ((!annie.SaveStun()) && (annie.CanFarm()))
            {
                if ((Spells.Q.IsReady()) && (annie.GetParamBool("useqtofarm")))
                {
                    List<Obj_AI_Base> Minions = MinionManager.GetMinions(annie.Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth)
                        .Where(x => Spells.Q.IsKillable(x)).ToList();

                    if ((Minions != null) && (Minions.Count > 0))
                    {
                        Spells.Q.Cast(Minions[0]);
                    }
                }
            }
        }

        private void CancelingAAOnSupportMode(Orbwalking.BeforeAttackEventArgs args)
        {
            if ((args.Target is Obj_AI_Base) && (((Obj_AI_Base)args.Target).IsMinion) && (!annie.CanFarm()))
            {
                args.Process = false;
            }
        }
    }
}
