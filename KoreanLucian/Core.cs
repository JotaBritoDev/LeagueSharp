using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using KoreanCommon;

namespace KoreanLucian
{
    using SharpDX;

    class Core : CommonOrbwalkComplementation
    {
        private bool hasPassive;

        private readonly object lockObject;

        public Core(CommonChampion champion)
            : base(champion)
        {
            lockObject = new object();

            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Spellbook.OnCastSpell += LockR;
            Spellbook.OnCastSpell += PassiveControl;
        }

        private void PassiveControl(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E)
            {
                ProcessSpell();
            }
        }

        private void ProcessSpell()
        {
            hasPassive = true;
            Orbwalking.ResetAutoAttackTimer();
        }

        private void LockR(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot != SpellSlot.R)
            {
                return;
            }

            lock (lockObject)
            {
                Spellbook.OnCastSpell -= LockR;

                if (KoreanUtils.GetParamBool(champion.MainMenu, "useyoumuu") && ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                {
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
                }

                if (KoreanUtils.GetParamBool(champion.MainMenu, "lockr"))
                {
                    var target = TargetSelector.GetTarget(champion.Player, R.Range, TargetSelector.DamageType.Physical);

                    if (target != null)
                    {
                        args.Process = false;
                        if (R.Cast(target.Position))
                        {
                            ProcessSpell();
                        }
                    }
                }

                Spellbook.OnCastSpell += LockR;
            }
        }

        private bool CheckPassive()
        {
            return hasPassive;
        }

        //Maybe next feature
        //private bool IsJungleMob(Obj_AI_Base target)
        //{
        //    return (target.SkinName.ToLowerInvariant() == "sru_blue"
        //            || target.SkinName.ToLowerInvariant() == "sru_gromp"
        //            || target.SkinName.ToLowerInvariant() == "sru_murkwolf"
        //            || target.SkinName.ToLowerInvariant() == "sru_razorbeak"
        //            || target.SkinName.ToLowerInvariant() == "sru_baron"
        //            || target.SkinName.ToLowerInvariant() == "sru_dragon"
        //            || target.SkinName.ToLowerInvariant() == "sru_red"
        //            || target.SkinName.ToLowerInvariant() == "sru_crab"
        //            || target.SkinName.ToLowerInvariant() == "sru_krug");
        //}

        private bool HaveManaToHaras()
        {
            return champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "manalimittoharas");
        }

        private bool HaveManaToLaneclear()
        {
            return champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "manalimittolaneclear");
        }

        void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
            {
                hasPassive = false;
            }
        }

        public override void LastHitMode()
        {
            //no needed
        }

        public override void HarasMode()
        {
            lock (lockObject)
            {
                if (champion.CastExtendedQ())
                {
                    ProcessSpell();
                }

                Obj_AI_Hero target;

                if (!CheckPassive() && HaveManaToHaras() && Q.UseOnHaras && Q.IsReady() && Q.CanCast())
                {
                    target = TargetSelector.GetTarget(champion.Player, Q.Range, TargetSelector.DamageType.Physical);

                    if (target != null && Q.IsReadyToCastOn(target) && Q.CanCast(target))
                    {
                        if (Q.CastOnUnit(target))
                        {
                            ProcessSpell();
                        }
                    }
                }

                if (W.UseOnHaras && !CheckPassive() && HaveManaToHaras() && W.IsReady() && W.CanCast())
                {
                    target = TargetSelector.GetTarget(champion.Player, W.Range, TargetSelector.DamageType.Physical);

                    if (target != null && target.Distance(champion.Player) <= Orbwalking.GetRealAutoAttackRange(champion.Player))
                    {
                        if (W.Cast(target.Position))
                        {
                            ProcessSpell();
                        }
                    }
                    else
                    {
                        PredictionOutput wPrediction = W.GetPrediction(target);

                        if (wPrediction != null && wPrediction.Hitchance >= HitChance.High
                            && wPrediction.CastPosition != Vector3.Zero)
                        {
                            if (W.Cast(wPrediction.CastPosition))
                            {
                                ProcessSpell();
                            }
                        }
                    }
                }

                if (((E.Instance.ManaCost > 0 && HaveManaToHaras()) || E.Instance.ManaCost.Equals(0f))
                    && (!CheckPassive() && E.UseOnHaras && E.IsReady() && E.CanCast()))
                {
                    target = TargetSelector.GetTarget(
                        champion.Player,
                        E.Range + Orbwalking.GetRealAutoAttackRange(champion.Player),
                        TargetSelector.DamageType.Physical);

                    if (target != null && !target.IsDead && target.Distance(champion.Player) > Orbwalking.GetRealAutoAttackRange(champion.Player))
                    {
                        if (E.Cast(Game.CursorPos))
                        {
                            ProcessSpell();
                        }
                    }
                }
            }
        }

        public override void LaneClearMode()
        {
            if (CheckPassive())
            {
                return;
            }

            lock (lockObject)
            {
                if (Q.UseOnLaneClear && !CheckPassive() && HaveManaToLaneclear() && champion.CastExtendedQToLaneClear())
                {
                    ProcessSpell();
                }

                if (W.UseOnLaneClear && !CheckPassive() && HaveManaToHaras())
                {
                    List<Obj_AI_Base> minions = MinionManager.GetMinions(W.Range).ToList();
                    MinionManager.FarmLocation farmLocation = W.GetCircularFarmLocation(minions);

                    int minMinions = KoreanUtils.GetParamSlider(champion.MainMenu, "wcounthit");

                    if (farmLocation.MinionsHit >= minMinions && minions.Count >= minMinions)
                    {
                        if (W.Cast(farmLocation.Position))
                        {
                            ProcessSpell();
                        }
                    }
                }

                if (E.UseOnLaneClear && !CheckPassive()
                    && (E.Instance.ManaCost.Equals(0) || (E.Instance.ManaCost > 0 && HaveManaToHaras())))
                {
                    Obj_AI_Base target =
                        MinionManager
                            .GetMinions(
                                E.Range + Orbwalking.GetRealAutoAttackRange(champion.Player))
                            .FirstOrDefault(minion => champion.Player.Distance(minion) > Orbwalking.GetRealAutoAttackRange(champion.Player)
                                && Game.CursorPos.Distance(minion.Position) < champion.Player.Distance(minion));

                    if (target != null)
                    {
                        if (E.Cast(Game.CursorPos))
                        {
                            ProcessSpell();
                        }
                    }
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

            if (Q.UseOnCombo && !CheckPassive() && Q.IsReady() && Q.CanCast())
            {
                target = TargetSelector.GetTarget(champion.Player, Q.Range, TargetSelector.DamageType.Physical);

                if (target != null && Q.IsReadyToCastOn(target) && Q.CanCast(target))
                {
                    if (Q.CastOnUnit(target))
                    {
                        ProcessSpell();
                    }
                }
            }

            if (W.UseOnCombo && !CheckPassive() && W.IsReady() && W.CanCast())
            {
                target = TargetSelector.GetTarget(champion.Player, W.Range, TargetSelector.DamageType.Physical);

                if (target != null && target.Distance(champion.Player) <= Orbwalking.GetRealAutoAttackRange(champion.Player))
                {
                    if (W.Cast(target.Position))
                    {
                        ProcessSpell();
                    }
                }
                else
                {
                    PredictionOutput wPrediction = W.GetPrediction(target);

                    if (wPrediction != null && wPrediction.Hitchance >= HitChance.Medium
                        && wPrediction.CastPosition != Vector3.Zero)
                    {
                        if (W.Cast(wPrediction.CastPosition))
                        {
                            ProcessSpell();
                        }
                    }
                }
            }

            if (E.UseOnCombo && !CheckPassive() && E.IsReady() && E.CanCast()
                && champion.Player.CountEnemiesInRange(E.Range + Orbwalking.GetRealAutoAttackRange(champion.Player) - 25f) > 0)
            {
                if (E.Cast(Game.CursorPos))
                {
                    ProcessSpell();
                }
            }
        }

        public override void Ultimate(Obj_AI_Hero target)
        {
            if (target == null)
            {
                R.Cast(Game.CursorPos);
            }
            else if (KoreanUtils.GetParamBool(champion.MainMenu, "lockr"))
            {
                R.Cast(target.Position);
            }
        }
    }
}
