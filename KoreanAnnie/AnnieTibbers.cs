using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace KoreanAnnie
{
    class AnnieTibbers
    {
        private const float tibbersRange = 1500f;

        private Obj_AI_Base _tibbers;

        public Obj_AI_Base Tibbers
        {
            get { return _tibbers; }
        }

        private Obj_AI_Base CurrentTarget;
        private Annie annie;

        public AnnieTibbers(Annie annie)
        {
            this.annie = annie;

            GameObject.OnCreate += NewTibbers;
            GameObject.OnDelete += DeleteTibbers;
            Game.OnUpdate += ControlTibbers;
            Orbwalking.OnAttack += AttackTurrent;
            Game.OnUpdate += FlashTibbersLogic;
        }

        private void FlashTibbersLogic(EventArgs args)
        {
            if (annie.GetParamKeyBind("flashtibbers"))
            {
                if ((annie.Spells.R.IsReady()) && (CommonSpells.Flash(annie.Player).IsReady) && (annie.CheckStun()))
                {
                    int minToCast = annie.GetParamSlider("minenemiestoflashr");

                    if (minToCast > 1)
                    {
                        foreach (PredictionOutput pred in ObjectManager.Get<Obj_AI_Hero>().
                            Where(x => x.IsValidTarget(annie.Spells.RFlash.Range)).
                            Select(x => annie.Spells.RFlash.GetPrediction(x, true)).
                                Where(pred => pred.Hitchance >= HitChance.High && pred.AoeTargetsHitCount >= minToCast))
                        {
                            annie.Player.Spellbook.CastSpell(CommonSpells.Flash(annie.Player).Slot, pred.CastPosition);
                            Utility.DelayAction.Add(50, () => annie.Spells.R.Cast(pred.CastPosition));
                        }
                    }
                    else
                    {
                        Obj_AI_Hero target = TargetSelector.GetTarget(annie.Spells.RFlash.Range, TargetSelector.DamageType.Magical);
                        annie.Player.Spellbook.CastSpell(CommonSpells.Flash(annie.Player).Slot, target.Position);
                        Utility.DelayAction.Add(50, () => annie.Spells.R.Cast(target.Position));
                    }
                }

                annie.ComboMode();
            }
        }

        private void AttackTurrent(AttackableUnit unit, AttackableUnit target)
        {
            if ((_tibbers != null) && (_tibbers.IsValid) && (unit.IsMe) && (target != null) && (target is Obj_AI_Turret))
            {
                CurrentTarget = (Obj_AI_Base)target;
            }
        }

        private void NewTibbers(GameObject sender, EventArgs args)
        {
            if (IsTibbers(sender))
            {
                _tibbers = (Obj_AI_Base) sender;
            }
        }

        private void DeleteTibbers(GameObject sender, EventArgs args)
        {
            if (IsTibbers(sender))
            {
                _tibbers = null;
            }
        }

        private bool IsTibbers(GameObject sender)
        {
            return ((sender != null) && (sender.IsValid) && (sender.Name.ToLowerInvariant().Equals("tibbers")) && (sender.IsAlly));
        }

        public void ControlTibbers(EventArgs args)
        {
            if ((_tibbers != null) && (_tibbers.IsValid))
            {
                Obj_AI_Base target = FindTarget();

                if ((target != null))
                {
                    annie.Player.IssueOrder(_tibbers.Distance(target.Position) > 200 ? 
                        GameObjectOrder.MovePet : 
                        GameObjectOrder.AutoAttackPet, target);
                }
            }
        }

        private Obj_AI_Base FindTarget()
        {
            Obj_AI_Base target;

            target = GetChampion();

            if (target != null)
                return target;

            target = GetBaronOrDragon();

            if (target != null)
                return target;

            target = GetJungleMob();

            if (target != null)
                return target;

            target = GetMinion();

            if (target != null)
                return target;

            if ((CurrentTarget != null) && (CurrentTarget.IsValidTarget(annie.Player.AttackRange + 200f)))
                return CurrentTarget;
            else
                CurrentTarget = null;

            return annie.Player;
        }

        private Obj_AI_Base GetMinion()
        {
            List<Obj_AI_Base> minion = MinionManager.GetMinions(_tibbers.Position, tibbersRange).
                OrderBy(x => x.Distance(_tibbers.Position)).ToList();
            if ((minion != null) && (minion.Count > 0))
            {
                return minion[0];
            }
            else
            {
                return null;
            }
        }

        private Obj_AI_Base GetJungleMob()
        {
            List<Obj_AI_Base> jungleMob = ObjectManager.Get<Obj_AI_Base>().
                Where(obj => ((obj.SkinName.ToLowerInvariant() == "sru_blue" ||
                               obj.SkinName.ToLowerInvariant() == "sru_gromp" ||
                               obj.SkinName.ToLowerInvariant() == "sru_murkwolf" ||
                               obj.SkinName.ToLowerInvariant() == "sru_razorbeak" ||
                               obj.SkinName.ToLowerInvariant() == "sru_red" ||
                               obj.SkinName.ToLowerInvariant() == "sru_krug") &&
                               (obj.IsVisible && obj.HealthPercent < 100) && (obj.HealthPercent > 0) && (obj.IsVisible))).
                ToList();

            if ((jungleMob != null) && (jungleMob.Count > 0))
            {
                return jungleMob[0];
            }
            else
            {
                return null;
            }
        }

        private Obj_AI_Base GetBaronOrDragon()
        {
            List<Obj_AI_Base> legendaryMonster = ObjectManager.Get<Obj_AI_Base>().
                Where(obj => ((obj.SkinName.ToLowerInvariant() == "sru_dragon" || obj.SkinName.ToLowerInvariant() == "sru_baron") && obj.IsVisible && obj.HealthPercent < 100 && obj.HealthPercent > 0)).
                ToList();

            if ((legendaryMonster != null) && (legendaryMonster.Count > 0))
            {
                return legendaryMonster[0];
            }
            else
            {
                return null;
            }
        }

        private Obj_AI_Base GetChampion()
        {
            Obj_AI_Hero champ = TargetSelector.GetTarget(tibbersRange, TargetSelector.DamageType.Magical);
            if ((champ != null))
            {
                return champ;
            }
            else
            {
                return null;
            }
        }

    }
}
