namespace KoreanZed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using KoreanZed.QueueActions;
    using KoreanZed.Enumerators;

    using SharpDX;
    using SharpDX.Direct3D9;

    class ZedCore
    {
        private Orbwalking.Orbwalker ZedOrbwalker { get; set; }

        private readonly ZedMenu zedMenu;

        private readonly ZedSpell q;

        private readonly ZedSpell w;

        private readonly ZedSpell e;

        private readonly ZedSpell r;

        private readonly Obj_AI_Hero player;

        private readonly ZedOffensiveItems zedItems;

        private readonly ActionQueueList harasQueue;

        private readonly ActionQueueList comboQueue;

        private readonly ZedShadows shadows;

        private readonly ZedEnergyChecker energy;

        private readonly ActionQueue actionQueue;

        public ZedCore(ZedSpells zedSpells, Orbwalking.Orbwalker zedOrbwalker, ZedMenu zedMenu, ZedShadows zedShadows)
        {
            q = zedSpells.Q;
            w = zedSpells.W;
            e = zedSpells.E;
            r = zedSpells.R;

            player = ObjectManager.Player;
            ZedOrbwalker = zedOrbwalker;
            this.zedMenu = zedMenu;

            actionQueue = new ActionQueue();
            harasQueue = new ActionQueueList();
            comboQueue = new ActionQueueList();
            zedItems = new ZedOffensiveItems(zedMenu);
            energy = new ZedEnergyChecker(zedMenu);
            shadows = zedShadows;

            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            switch (ZedOrbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                default:
                    return;
            }
        }

        private void Combo()
        {
            if (actionQueue.ExecuteNextAction(comboQueue))
            {
                return;
            }

            if (w.UseOnCombo && w.IsReady() && player.HasBuff("zedr2") && shadows.CanCast)
            {
                Obj_AI_Hero target = TargetSelector.GetTarget(w.Range + e.Range, TargetSelector.DamageType.Physical);

                if (target != null)
                {
                    actionQueue.EnqueueAction(comboQueue,
                        () => true,
                        () => shadows.Cast(target),
                        () => true);
                    return;
                }
            }

            float maxRange = float.MaxValue;

            if (r.UseOnCombo && r.IsReady() && r.Instance.ToggleState == 0)
            {
                Obj_AI_Hero target = null;

                maxRange = Math.Min(maxRange, r.Range);

                if (zedMenu.GetParamBool("koreanzed.combo.ronselected"))
                {
                    if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget.IsValidTarget(maxRange))
                    {
                        target = TargetSelector.SelectedTarget;
                    }
                }
                else
                {
                    List<Obj_AI_Hero> ignoredChamps = zedMenu.GetBlockList(BlockListType.Ultimate);
                    target = TargetSelector.GetTarget(maxRange, r.DamageType, true, ignoredChamps);
                }

                if (target != null)
                {
                    actionQueue.EnqueueAction(
                        comboQueue,
                        () => r.IsReady() && r.Instance.ToggleState == 0,
                        () => r.Cast(target),
                        () => r.Instance.ToggleState != 0);

                    actionQueue.EnqueueAction(comboQueue, () => true, () => zedItems.UseItems(target), () => true);

                    return;
                }
            }

            if (q.UseOnCombo)
            {
                maxRange = Math.Min(maxRange, q.Range);
                Obj_AI_Hero target = TargetSelector.GetTarget(maxRange, q.DamageType);

                PredictionOutput predictionOutput = q.GetPrediction(target);

                if (predictionOutput.Hitchance >= HitChance.VeryHigh)
                {
                    q.Cast(predictionOutput.CastPosition);
                }
            }

            if (e.UseOnCombo)
            {
                maxRange = Math.Min(maxRange, e.Range);
                Obj_AI_Hero target = TargetSelector.GetTarget(maxRange, e.DamageType);
                if (target != null)
                {
                    actionQueue.EnqueueAction(comboQueue,
                        () => true,
                        () => zedItems.UseItems(target),
                        () => true);

                    actionQueue.EnqueueAction(comboQueue,
                        () => e.IsReady(),
                        () => e.Cast(),
                        () => true);
                }
            }

            if (w.UseOnCombo && w.IsReady() && w.Instance.ToggleState != 0)
            {
                List<Obj_AI_Base> shadowList = shadows.GetShadows();

                foreach (Obj_AI_Base objAiBase in shadowList)
                {
                    Obj_AI_Hero target = TargetSelector.GetTarget(2000F, TargetSelector.DamageType.Physical);

                    if (target != null && player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target) + 50F &&
                        objAiBase.Distance(target) < player.Distance(target))
                    {
                        w.Cast();
                    }
                }
            }
        }

        private void Harass()
        {
            actionQueue.ExecuteNextAction(harasQueue);

            shadows.Harass();

            float maxRange = float.MaxValue;

            if (!energy.ReadyToHaras)
            {
                return;
            }

            List<Obj_AI_Hero> blackList = zedMenu.GetBlockList(BlockListType.Harass);

            if ((w.UseOnHarass && w.IsReady() && w.Instance.ToggleState == 0)
                && (player.HealthPercent
                    > zedMenu.GetParamSlider("koreanzed.harasmenu.wusage.dontuselowlife")
                    && HeroManager.Enemies.Count(
                        hero => !hero.IsDead && !hero.IsZombie && player.Distance(hero) < 2000F)
                    < zedMenu.GetParamSlider("koreanzed.harasmenu.wusage.dontuseagainst")))
            {
                if (q.UseOnHarass)
                    if (q.UseOnHarass && q.IsReady() && (player.Mana > q.ManaCost + w.ManaCost))
                    {
                        switch ((ShadowHarassTrigger)zedMenu.GetParamStringList("koreanzed.harasmenu.wusage.trigger"))
                        {
                            case ShadowHarassTrigger.MaxRange:
                                maxRange = Math.Min(maxRange, q.Range + w.Range);
                                break;

                            case ShadowHarassTrigger.MaxDamage:
                                maxRange = Math.Min(maxRange, w.Range + e.Range);
                                break;
                        }

                        Obj_AI_Hero target = TargetSelector.GetTarget(maxRange, q.DamageType, true, blackList);

                        if (target != null)
                        {
                            shadows.Cast(target);
                            actionQueue.EnqueueAction(harasQueue, () => true, () => shadows.Harass(), () => false);
                        }
                    }

                if (e.UseOnHarass && e.IsReady() && (player.Mana > e.ManaCost + w.ManaCost))
                {
                    maxRange = Math.Min(maxRange, e.Range + w.Range);
                    Obj_AI_Hero target = TargetSelector.GetTarget(maxRange, e.DamageType, true, blackList);

                    if (target != null)
                    {
                        shadows.Cast(target);
                        actionQueue.EnqueueAction(harasQueue,
                            () => true,
                            () => shadows.Harass(),
                            () => false);
                    }
                }
            }

            if (q.UseOnHarass && energy.ReadyToHaras)
            {
                maxRange = Math.Min(maxRange, q.Range);
                Obj_AI_Hero target = TargetSelector.GetTarget(maxRange, q.DamageType, true, blackList);

                if (target != null)
                {
                    PredictionOutput predictionOutput = q.GetPrediction(target);

                    bool checkColision = zedMenu.GetParamBool("koreanzed.harasmenu.checkcollisiononq");

                    if (predictionOutput.Hitchance >= HitChance.High
                        && (!checkColision
                            || !q.GetCollision(
                                player.Position.To2D(),
                                new List<Vector2>() { predictionOutput.CastPosition.To2D() }).Any()))
                    {
                        q.Cast(predictionOutput.CastPosition);
                    }
                }
            }

            if (e.UseOnHarass && energy.ReadyToHaras)
            {
                if (TargetSelector.GetTarget(e.Range, e.DamageType) != null)
                {
                    e.Cast();
                }
            }

            LastHit();
        }

        private void LaneClear()
        {
            LastHit();

            if (q.UseOnLaneClear && energy.ReadyToLaneClear)
            {
                MinionManager.FarmLocation farmLocation = q.GetLineFarmLocation(MinionManager.GetMinions(q.Range));

                if (farmLocation.MinionsHit >= zedMenu.GetParamSlider("koreanzed.laneclearmenu.useqif"))
                {
                    q.Cast(farmLocation.Position);
                }
                else
                {
                    Obj_AI_Base jungleMob =
                        MinionManager.GetMinions(q.Range / 1.5F, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                            .FirstOrDefault();

                    if (jungleMob != null)
                    {
                        q.Cast(q.GetPrediction(jungleMob).CastPosition);
                    }
                }
            }

            if (e.UseOnLaneClear && energy.ReadyToLaneClear)
            {
                if (MinionManager.GetMinions(e.Range).Count()
                    >= zedMenu.GetParamSlider("koreanzed.laneclearmenu.useeif"))
                {
                    e.Cast();
                }
                else
                {
                    if (MinionManager.GetMinions(e.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                            .Any())
                    {
                        e.Cast();
                    }
                }
            }           
        }

        private void LastHit()
        {
            if (q.UseOnLastHit)
            {
                Obj_AI_Base target =
                    MinionManager.GetMinions(q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth)
                        .FirstOrDefault(
                            minion =>
                            minion.Distance(player) > Orbwalking.GetRealAutoAttackRange(minion) + 10F 
                            && !minion.IsDead
                            && q.GetDamage(minion) / 2 > minion.Health);

                if (target != null && q.IsKillable(target))
                {
                    PredictionOutput predictionOutput = q.GetPrediction(target);

                    q.Cast(predictionOutput.CastPosition);
                }
            }

            if (e.UseOnLastHit)
            {
                if (MinionManager.GetMinions(e.Range).Count(minion => e.IsKillable(minion))
                    >= zedMenu.GetParamSlider("koreanzed.lasthitmenu.useeif"))
                {
                    e.Cast();
                }
            }
        }

        public float ComboDamage(Obj_AI_Hero target)
        {
            float result = q.UseOnCombo && q.IsReady()
                               ? (q.GetCollision(player.Position.To2D(), new List<Vector2>() { target.Position.To2D() })
                                      .Any()
                                      ? q.GetDamage(target) / 2
                                      : q.GetDamage(target))
                               : 0F;

            result += e.UseOnCombo && e.IsReady() ? e.GetDamage(target) : 0F;

            result += w.UseOnCombo && w.IsReady() && player.Distance(target) < w.Range + Orbwalking.GetRealAutoAttackRange(target)
                          ? (float) player.GetAutoAttackDamage(target, true)
                          : 0F;

            result += r.UseOnCombo && r.IsReady()
                          ? (float)(r.GetDamage(target) + player.GetAutoAttackDamage(target, true))
                          : 0F;

            return result;
        }

        public void ForceUltimate(Obj_AI_Hero target = null)
        {
            if (target != null && r.CanCast(target))
            {
                r.Cast(target);
            }
            else
            {
                r.Cast(TargetSelector.GetTarget(r.Range, r.DamageType));
            }
        }
    }
}
