namespace KoreanAnnie
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KoreanAnnie.Common;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class AnnieTibbers
    {
        #region Constants

        private const float TibbersRange = 1500f;

        #endregion

        #region Fields

        private readonly Annie annie;

        private Obj_AI_Base currentTarget;

        #endregion

        #region Constructors and Destructors

        public AnnieTibbers(Annie annie)
        {
            this.annie = annie;

            GameObject.OnCreate += NewTibbers;
            GameObject.OnDelete += DeleteTibbers;
            Game.OnUpdate += ControlTibbers;
            Orbwalking.OnAttack += AttackTurrent;
            Game.OnUpdate += FlashTibbersLogic;
        }

        #endregion

        #region Public Properties

        public Obj_AI_Base Tibbers { get; private set; }

        #endregion

        #region Methods

        private static Obj_AI_Base GetBaronOrDragon()
        {
            List<Obj_AI_Base> legendaryMonster =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        obj =>
                        ((obj.SkinName.ToLowerInvariant() == "sru_dragon"
                          || obj.SkinName.ToLowerInvariant() == "sru_baron") && obj.IsVisible && obj.HealthPercent < 100
                         && obj.HealthPercent > 0))
                    .ToList();

            return (legendaryMonster.Count > 0) ? legendaryMonster[0] : null;
        }

        private Obj_AI_Base GetChampion()
        {
            Obj_AI_Hero champ = TargetSelector.GetTarget(Tibbers, TibbersRange, TargetSelector.DamageType.Magical);

            return champ;
        }

        private static Obj_AI_Base GetJungleMob()
        {
            List<Obj_AI_Base> jungleMob =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        obj =>
                        ((obj.SkinName.ToLowerInvariant() == "sru_blue"
                          || obj.SkinName.ToLowerInvariant() == "sru_gromp"
                          || obj.SkinName.ToLowerInvariant() == "sru_murkwolf"
                          || obj.SkinName.ToLowerInvariant() == "sru_razorbeak"
                          || obj.SkinName.ToLowerInvariant() == "sru_red"
                          || obj.SkinName.ToLowerInvariant() == "sru_krug")
                         && (obj.IsVisible && obj.HealthPercent < 100) && (obj.HealthPercent > 0) && (obj.IsVisible)))
                    .ToList();

            return (jungleMob.Count > 0) ? jungleMob[0] : null;
        }

        private static bool IsTibbers(GameObject sender)
        {
            return ((sender != null) && (sender.IsValid) && (sender.Name.ToLowerInvariant().Equals("tibbers"))
                    && (sender.IsAlly));
        }

        private void AttackTurrent(AttackableUnit unit, AttackableUnit target)
        {
            if ((Tibbers != null) && (Tibbers.IsValid) && (unit.IsMe) && (target is Obj_AI_Turret))
            {
                currentTarget = (Obj_AI_Base)target;
            }
        }

        private void ControlTibbers(EventArgs args)
        {
            if ((Tibbers == null) || (!Tibbers.IsValid) || (!annie.GetParamBool("autotibbers")))
            {
                return;
            }

            Obj_AI_Base target = FindTarget();

            if ((target != null))
            {
                //Method bugged == plz fix the Common
                //annie.Player.IssueOrder(
                //    Tibbers.Distance(target.Position) > 200 ? GameObjectOrder.MovePet : GameObjectOrder.AutoAttackPet,
                //    target);

                if (Tibbers.Distance(target.Position) > 200)
                {
                    annie.Player.IssueOrder(GameObjectOrder.MovePet, target);
                }
                else
                {
                    Tibbers.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
            }
        }

        private void DeleteTibbers(GameObject sender, EventArgs args)
        {
            if (IsTibbers(sender))
            {
                Tibbers = null;
            }
        }

        private Obj_AI_Base FindTarget()
        {
            Obj_AI_Base target = GetChampion();

            if (target != null)
            {
                return target;
            }

            target = GetBaronOrDragon();

            if (target != null)
            {
                return target;
            }

            target = GetJungleMob();

            if (target != null)
            {
                return target;
            }

            target = GetMinion();

            if (target != null)
            {
                return target;
            }

            if ((currentTarget != null) && (currentTarget.IsValidTarget(annie.Player.AttackRange + 200f)))
            {
                return currentTarget;
            }
            currentTarget = null;

            return annie.Player;
        }

        private void FlashTibbersLogic(EventArgs args)
        {
            if (!annie.GetParamKeyBind("flashtibbers"))
            {
                return;
            }

            if ((annie.Spells.R.IsReady()) && (FlashSpell.IsReady(annie.Player)) && (annie.CheckStun()))
            {
                int minToCast = annie.GetParamSlider("minenemiestoflashr");

                if (minToCast > 1)
                {
                    foreach (
                        PredictionOutput pred in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(x => x.IsValidTarget(annie.Spells.RFlash.Range))
                                .Select(x => annie.Spells.RFlash.GetPrediction(x, true))
                                .Where(pred => pred.Hitchance >= HitChance.High && pred.AoeTargetsHitCount >= minToCast)
                        )
                    {
                        PredictionOutput pred1 = pred;
                        annie.Player.Spellbook.CastSpell(FlashSpell.Slot(annie.Player), pred1.CastPosition);
                        Utility.DelayAction.Add(10, () => annie.Spells.R.Cast(pred1.CastPosition));
                    }
                }
                else
                {
                    Obj_AI_Hero target = TargetSelector.GetTarget(
                        annie.Spells.RFlash.Range,
                        TargetSelector.DamageType.Magical);
                    if (target != null)
                    {
                        annie.Player.Spellbook.CastSpell(FlashSpell.Slot(annie.Player), target.Position);
                        Utility.DelayAction.Add(50, () => annie.Spells.R.Cast(target.Position));
                    }
                }
            }

            if (annie.GetParamBool("orbwalktoflashtibbers"))
            {
                annie.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
            annie.AnnieOrbwalker.ComboMode();
        }

        private Obj_AI_Base GetMinion()
        {
            List<Obj_AI_Base> minion =
                MinionManager.GetMinions(Tibbers.Position, TibbersRange)
                    .OrderBy(x => x.Distance(Tibbers.Position))
                    .ToList();

            return (minion.Count > 0) ? minion[0] : null;
        }

        private void NewTibbers(GameObject sender, EventArgs args)
        {
            if (IsTibbers(sender))
            {
                Tibbers = (Obj_AI_Base)sender;
            }
        }

        #endregion
    }
}