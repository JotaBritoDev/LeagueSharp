namespace KoreanOrbwalker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Orbwalker
    {
        #region Fields

        public ExtraDamage ExtraDamage;

        private readonly Queue AA;

        private readonly Obj_AI_Hero player;

        private bool attack;

        private float nextAttack;

        private float windUpTime;

        #endregion

        #region Constructors and Destructors

        public Orbwalker()
        {
            player = ObjectManager.Player;
            windUpTime = 0;
            nextAttack = 0;
            attack = false;

            CustomMenu = new CustomMenu();
            AA = new Queue();
            ExtraDamage = new ExtraDamage(this);
            var resetableAA = new ResetableAA(this);
            var range = new Range(this);

            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Obj_AI_Base.OnTarget += Obj_AI_Turret_OnTarget;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        #endregion

        #region Public Properties

        public CustomMenu CustomMenu { get; set; }

        #endregion

        #region Public Methods and Operators

        public float AutoAttackDamage(Obj_AI_Base target)
        {
            var result = (float)player.GetAutoAttackDamage(target) + ExtraDamage.Get(target);

            return result;
        }

        public GameObjectCombatType CombatType()
        {
            if (player.SkinName.ToLowerInvariant() == "jayce") //Checking here cuz common is bugged... jayce swaps between melee / ranged and the Player.CombatType is always "Melee"
            {
                return player.HasBuff("jaycestancegun") ? GameObjectCombatType.Ranged : GameObjectCombatType.Melee;
            }
            else
            {
                return player.CombatType;
            }
        }

        public bool IsAutoAttackReady()
        {
            return !IsWindingUp() && Game.Time >= nextAttack;
        }

        public bool IsWindingUp()
        {
            return attack && Game.Time <= windUpTime;
        }

        public OrbwalkMode Mode()
        {
            OrbwalkMode result;
            if (CustomMenu.ComboActive)
            {
                result = OrbwalkMode.Combo;
            }
            else if (CustomMenu.LaneClearActive)
            {
                result = OrbwalkMode.LaneClear;
            }
            else if (CustomMenu.MixedActive)
            {
                result = OrbwalkMode.Mixed;
            }
            else if (CustomMenu.LastHitActive)
            {
                result = OrbwalkMode.LastHit;
            }
            else
            {
                result = OrbwalkMode.Inactive;
            }
            return result;
        }

        public void ResetAA()
        {
            nextAttack = Game.Time;
        }

        #endregion

        #region Methods

        private void Drawing_OnDraw(EventArgs args)
        {
        }

        private bool EnemyInRange(Obj_AI_Base target)
        {
            return target.IsEnemy && !target.IsDead && target.IsValid && target.IsTargetable && !target.IsInvulnerable
                   && target.IsVisible
                   && player.Distance(target) <= player.AttackRange + player.BoundingRadius + target.BoundingRadius;
        }

        private void EnqueueAA(AttackableUnit target)
        {
            AA.Enqueue(
                () => player.CanAttack && target.IsValid && !attack && target.IsTargetable && !target.IsInvulnerable,
                () => player.IssueOrder(GameObjectOrder.AttackUnit, target),
                delegate
                    {
                        var result = !target.IsValid || target.IsDead || !target.IsTargetable || target.IsInvulnerable
                                     || (attack && Game.Time >= windUpTime)
                                     || player.Distance(target)
                                     > player.AttackRange + player.BoundingRadius + target.BoundingRadius;
                        if (result)
                        {
                            attack = false;
                        }
                        return result;
                    });
        }

        private void Game_OnUpdate(EventArgs args)
        {
            //Console.Clear();
            //foreach (var buffInstance in player.Buffs)
            //{
            //    Console.WriteLine(buffInstance.Name);
            //}

            if (Mode() == OrbwalkMode.Inactive)
            {
                AA.Clear();
                return;
            }

            if (AA.ExecuteNext())
            {
                return;
            }

            player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (IsAutoAttackReady())
            {
                AttackableUnit target;

                switch (Mode())
                {
                    case OrbwalkMode.Combo:
                        target = GetComboTarget() ?? GetTargetStructure();
                        break;
                    case OrbwalkMode.LaneClear:
                        target = GetLastHitTarget() ?? GetLaneclearTarget() ?? GetTargetStructure() ?? GetComboTarget();
                        break;
                    case OrbwalkMode.Mixed:
                        target = GetLastHitTarget() ?? GetComboTarget() ?? GetTargetStructure();
                        break;
                    case OrbwalkMode.LastHit:
                        target = GetLastHitTarget() ?? GetTargetStructure();
                        break;
                    default:
                        target = null;
                        break;
                }

                if (target != null && target.IsValid)
                {
                    EnqueueAA(target);
                }
            }
        }

        private float GetAATime(Obj_AI_Minion minion)
        {
            var result = player.AttackCastDelay * 1000;

            if (CombatType() == GameObjectCombatType.Ranged)
            {
                var speed = player.BasicAttack.MissileSpeed;
                speed *= player.SkinName.ToLowerInvariant() == "jayce" ? (float)Math.PI * 2 : 1; //Checking here cuz common is bugged... jayce swaps between melee / ranged and the Player.CombatType is always "Melee"

                result += (int)((player.Distance(minion) / speed) * 1000);
            }

            return result;
        }

        private Obj_AI_Base GetComboTarget()
        {
            return ObjectManager.Get<Obj_AI_Hero>().Where(EnemyInRange).FirstOrDefault();
        }

        private Obj_AI_Base GetLaneclearTarget()
        {
            var minionList =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(EnemyInRange)
                    .OrderByDescending(target => target.MaxHealth)
                    .ToList();

            return !HoldBitch(minionList) ? minionList.FirstOrDefault() : null;
        }

        private Obj_AI_Base GetLastHitTarget()
        {
            var result =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(EnemyInRange)
                    .OrderByDescending(target => target.MaxHealth)
                    .FirstOrDefault(
                        minion =>
                        AutoAttackDamage(minion)
                        > HealthPrediction.GetHealthPrediction(
                            minion,
                            (int)GetAATime(minion),
                            RandomDelay.Get(CustomMenu.FeelingLike)));

            return result;
        }

        private AttackableUnit GetTargetStructure()
        {
            AttackableUnit result;

            if (!HoldBitch(null))
            {
                result =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .FirstOrDefault(
                            target =>
                            target.IsTargetable && !target.IsAlly
                            && target.Distance(player)
                            <= player.AttackRange + player.BoundingRadius + target.BoundingRadius);
                if (result == null)
                {
                    result =
                       ObjectManager.Get<Obj_BarracksDampener>()
                            .FirstOrDefault(
                                target =>
                                target.IsTargetable && !target.IsAlly
                                && player.Distance(target.Position)
                                <= player.AttackRange + player.BoundingRadius + target.BoundingRadius);
                }

                if (result == null)
                {
                    result =
                    ObjectManager.Get<Obj_HQ>()
                        .FirstOrDefault(
                            target =>
                            target.IsTargetable && !target.IsAlly
                            && player.Distance(target)
                            <= player.AttackRange + player.BoundingRadius + target.BoundingRadius);
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        private bool HoldBitch(List<Obj_AI_Minion> minionList)
        {
            var minions = minionList
                          ?? ObjectManager.Get<Obj_AI_Minion>()
                                 .Where(EnemyInRange)
                                 .OrderByDescending(target => target.MaxHealth)
                                 .ToList();

            return
                minions.Where(
                    minion =>
                    minion.Health
                    > HealthPrediction.GetHealthPrediction(
                        minion,
                        (int)(GetAATime(minion) + player.AttackDelay * 10000),
                        0))
                    .Any(
                        minion =>
                        AutoAttackDamage(minion) * 2
                        > HealthPrediction.GetHealthPrediction(
                            minion,
                            (int)(GetAATime(minion) + player.AttackDelay * 2000),
                            0));
        }

        private void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.IsAutoAttack())
            {
                attack = true;
                windUpTime = Game.Time + player.AttackCastDelay + (RandomWindup.Get(CustomMenu.FeelingLike) / 1000f);
                nextAttack = Game.Time + player.AttackDelay;
            }
        }

        private void Obj_AI_Turret_OnTarget(Obj_AI_Base sender, Obj_AI_BaseTargetEventArgs args)
        {
            if (sender == null || args.Target == null || !sender.IsAlly || !args.Target.IsEnemy
                || !sender.SkinName.ToLowerInvariant().Contains("turret")
                || !args.Target.Name.ToLowerInvariant().Contains("minion"))
            {
                return;
            }

            if (player.Distance(args.Target) <= player.AttackRange + player.BoundingRadius + args.Target.BoundingRadius)
            {
                var target = (Obj_AI_Minion)args.Target;
                var predictedHealth = HealthPrediction.GetHealthPrediction(target, (int)GetAATime(target), 500);

                if (predictedHealth > sender.GetAutoAttackDamage(target) + AutoAttackDamage(target)
                    && predictedHealth < sender.GetAutoAttackDamage(target) * 2)
                {
                    EnqueueAA(target);
                }
            }
        }

        #endregion
    }
}