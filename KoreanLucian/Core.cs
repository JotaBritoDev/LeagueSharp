using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private readonly Spell WFarm;

        private readonly Spell QLaneClear;

        private readonly Spell QHaras;

        private object lockObject;

        public Core(CommonChampion champion)
            : base(champion)
        {
            WFarm = new Spell(W.Slot, W.Range, W.DamageType);
            WFarm.SetSkillshot(W.Delay, W.Width, W.Speed, true, W.Type);

            QLaneClear = new Spell(Q.Slot, 1200, Q.DamageType);
            QLaneClear.SetSkillshot(0.55f, 75f, float.MaxValue, true, SkillshotType.SkillshotLine);

            QHaras = new Spell(Q.Slot, 1100, Q.DamageType);
            QHaras.SetSkillshot(0.55f, 0.5f, float.MaxValue, true, SkillshotType.SkillshotLine);

            lockObject = new object();

            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Spellbook.OnCastSpell += lockR;
            Spellbook.OnCastSpell += passiveControl;
        }

        private void passiveControl(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E)
            {
                hasPassive = true;
            }
        }

        private void lockR(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot != SpellSlot.R)
            {
                return;
            }

            lock (lockObject)
            {
                Spellbook.OnCastSpell -= lockR;

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
                        R.Cast(target.Position);
                        hasPassive = true;
                    }
                }

                Spellbook.OnCastSpell += lockR;
            }
        }

        private bool CheckPassive()
        {
            return hasPassive;
            //champion.Player.HasBuff("LucianPassiveBuff") << It sucks! X__X
        }

        private bool isJungleMob(Obj_AI_Base target)
        {
            return (target.SkinName.ToLowerInvariant() == "sru_blue"
                    || target.SkinName.ToLowerInvariant() == "sru_gromp"
                    || target.SkinName.ToLowerInvariant() == "sru_murkwolf"
                    || target.SkinName.ToLowerInvariant() == "sru_razorbeak"
                    || target.SkinName.ToLowerInvariant() == "sru_baron"
                    || target.SkinName.ToLowerInvariant() == "sru_dragon"
                    || target.SkinName.ToLowerInvariant() == "sru_red"
                    || target.SkinName.ToLowerInvariant() == "sru_crab"
                    || target.SkinName.ToLowerInvariant() == "sru_krug");
        }

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
            if (CheckPassive())
            {
                return;
            }

            lock (lockObject)
            {

                Obj_AI_Hero target;

                if (!CheckPassive() && HaveManaToHaras() && Q.UseOnHaras && Q.IsReady() && Q.CanCast())
                {
                    target = TargetSelector.GetTarget(champion.Player, QHaras.Range, TargetSelector.DamageType.Physical);

                    if (target != null && Q.IsReadyToCastOn(target) && Q.CanCast(target))
                    {
                        Q.CastOnUnit(target);
                        hasPassive = true;
                        Orbwalking.ResetAutoAttackTimer();
                    }
                    else if (KoreanUtils.GetParamBool(champion.MainMenu, "extendedq") && target != null
                             && QHaras.IsReady() && QHaras.CanCast(target))
                    {
                        List<Vector2> targetPos = new List<Vector2>();
                        targetPos.Add(target.Position.To2D());

                        var colision =
                            QHaras.GetCollision(champion.Player.Position.To2D(), targetPos, 10f)
                                .FirstOrDefault(x => Q.IsInRange(x) && x.Distance(target) <= 500f);

                        if (colision != null)
                        {
                            Q.CastOnUnit(colision);
                            hasPassive = true;
                            Orbwalking.ResetAutoAttackTimer();
                        }
                    }
                }

                if (!CheckPassive() && HaveManaToHaras() && W.UseOnHaras && W.IsReady() && W.CanCast())
                {
                    target = TargetSelector.GetTarget(champion.Player, W.Range, TargetSelector.DamageType.Physical);

                    if (target != null && target.Distance(champion.Player) <= Orbwalking.GetRealAutoAttackRange(champion.Player))
                    {
                        W.Cast(target.Position);
                        hasPassive = true;
                        Orbwalking.ResetAutoAttackTimer();
                    }
                    else
                    {
                        PredictionOutput wPrediction = W.GetPrediction(target);

                        if (wPrediction != null && wPrediction.Hitchance >= HitChance.High
                            && wPrediction.CastPosition != Vector3.Zero)
                        {
                            W.Cast(wPrediction.CastPosition);
                            hasPassive = true;
                            Orbwalking.ResetAutoAttackTimer();
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

                    if (target != null && target.Distance(champion.Player) > Orbwalking.GetRealAutoAttackRange(champion.Player))
                    {
                        E.Cast(Game.CursorPos);
                        hasPassive = true;
                        Orbwalking.ResetAutoAttackTimer();
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
                List<Obj_AI_Base> minionsBase = MinionManager.GetMinions(
                        champion.Player.Position,
                        QLaneClear.Range,
                        MinionTypes.All,
                        MinionTeam.NotAlly,
                        MinionOrderTypes.MaxHealth);

                if (HaveManaToLaneclear() && Q.UseOnLaneClear && Q.IsReady() && !CheckPassive())
                {
                    if (minionsBase.Count > 0)
                    {
                        foreach (Obj_AI_Base minion in minionsBase.Where(x => Q.IsInRange(x)))
                        {
                            if (QLaneClear.CountHits(minionsBase, minion.Position) >= KoreanUtils.GetParamSlider(champion.MainMenu, "qcounthit")
                                || isJungleMob(minion))
                            {
                                Q.CastOnUnit(minion);
                                hasPassive = true;
                                Orbwalking.ResetAutoAttackTimer();
                            }
                        }
                    }
                }

                List<Obj_AI_Base> minions = minionsBase.Where(x => W.IsInRange(x)).ToList();

                if (HaveManaToLaneclear() && W.UseOnLaneClear && W.IsReady() && !CheckPassive())
                {
                    if (minions.Count == 1 && isJungleMob(minions[0]))
                    {
                        PredictionOutput wPrediction = W.GetPrediction(minions[0]);
                        if (wPrediction != null && wPrediction.Hitchance >= HitChance.High && wPrediction.CastPosition != Vector3.Zero)
                        {
                            W.Cast(wPrediction.CastPosition);
                            hasPassive = true;
                            Orbwalking.ResetAutoAttackTimer();
                        }
                    }
                    else if (minions.Count > 0)
                    {
                        foreach (Obj_AI_Base minion in minions)
                        {
                            if (W.CountHits(minions, minion.Position) >= KoreanUtils.GetParamSlider(champion.MainMenu, "wcounthit")
                                || isJungleMob(minion))
                            {
                                W.Cast(minion.Position);
                                hasPassive = true;
                                Orbwalking.ResetAutoAttackTimer();
                            }
                        }
                    }
                }

                minions =
                    minionsBase.Where(
                        x => x.Distance(champion.Player) < Orbwalking.GetRealAutoAttackRange(champion.Player) + E.Range * 1.5)
                        .ToList();

                if (((E.Instance.ManaCost > 0 && HaveManaToLaneclear()) || E.Instance.ManaCost.Equals(0f))
                    && E.UseOnLaneClear && E.CanCast() && !CheckPassive())
                {
                    if ((minions.Count > 1 && E.IsReady())
                        || (minions.Count == 1 && isJungleMob(minions[0])))
                    {
                        E.Cast(Game.CursorPos);
                        hasPassive = true;
                        Orbwalking.ResetAutoAttackTimer();
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

            if (!CheckPassive() && Q.UseOnCombo && Q.IsReady() && Q.CanCast())
            {
                target = TargetSelector.GetTarget(champion.Player, Q.Range, TargetSelector.DamageType.Physical);

                if (target != null && Q.IsReadyToCastOn(target) && Q.CanCast(target))
                {
                    Q.CastOnUnit(target);
                    hasPassive = true;
                    Orbwalking.ResetAutoAttackTimer();
                }
            }

            if (!CheckPassive() && HaveManaToHaras() && W.UseOnHaras && W.IsReady() && W.CanCast())
            {
                target = TargetSelector.GetTarget(champion.Player, W.Range, TargetSelector.DamageType.Physical);

                if (target != null && target.Distance(champion.Player) <= Orbwalking.GetRealAutoAttackRange(champion.Player))
                {
                    W.Cast(target.Position);
                    hasPassive = true;
                    Orbwalking.ResetAutoAttackTimer();
                }
                else
                {
                    PredictionOutput wPrediction = W.GetPrediction(target);

                    if (wPrediction != null && wPrediction.Hitchance >= HitChance.Medium
                        && wPrediction.CastPosition != Vector3.Zero)
                    {
                        W.Cast(wPrediction.CastPosition);
                        hasPassive = true;
                        Orbwalking.ResetAutoAttackTimer();
                    }
                }
            }

            if (!CheckPassive() && E.UseOnCombo && E.IsReady() && E.CanCast()
                && champion.Player.CountEnemiesInRange(E.Range + Orbwalking.GetRealAutoAttackRange(champion.Player) - 25f) > 0)
            {
                E.Cast(Game.CursorPos);
                hasPassive = true;
                Orbwalking.ResetAutoAttackTimer();
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
