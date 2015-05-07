using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using KoreanAnnie.Properties;

namespace KoreanAnnie
{
    class Annie : CommonChamp
    {
        public const string menuDisplay = "Korean Annie";
        public float UltimateRange { get; set; }
        public Func<string, bool> GetParamBool;
        public Action<string, bool> SetValueBool;
        public Func<string, int> GetParamSlider;
        public Func<string, bool> GetParamKeyBind;
        public Func<string, string> ParamName;
        public Func<bool> CanFarm;
        public Func<bool> SaveStun;
        public Func<bool> CheckStun;

        public CommonMenu MainMenu { get; set; }
        public AnnieSpells Spells { get; set; }
        public AnnieButtons Buttons { get; set; }
        public AnnieTibbers Tibbers { get; set; }
        public AnnieDrawings Draws { get; set; }
        public CommonDamageDrawing DrawDamage { get; set; }
        public CommonForceUltimate ForceUltimate { get; set; }
        public Orbwalking.Orbwalker Orbwalker { get; set; }
        public Obj_AI_Hero Player { get; set; }

        public Annie() 
        {
            Player = ObjectManager.Player;
            MainMenu = new CommonMenu(menuDisplay, true);
            Orbwalker = MainMenu.Orbwalker;
            AnnieCustomMenu.Load(MainMenu);

            LoadLambdaExpressions();

            Spells = new AnnieSpells(this);
            Buttons = new AnnieButtons(this);
            Tibbers = new AnnieTibbers(this);
            Draws = new AnnieDrawings(this);
            DrawDamage = new CommonDamageDrawing(this);
            ForceUltimate = new CommonForceUltimate(this);
            UltimateRange = Spells.R.Range;
            ForceUltimate.ForceUltimate = Ultimate;

            DrawDamage.AmountOfDamage = Spells.GetMaxDamage;
            DrawDamage.Active = true;

            Obj_AI_Base.OnProcessSpellCast += EAgainstEnemyAA;
            Interrupter2.OnInterruptableTarget += InterruptDangerousSpells;
            AntiGapcloser.OnEnemyGapcloser += StunGapCloser;
            Orbwalking.BeforeAttack += CancelingAAOnSupportMode;
            Game.OnUpdate += OrbwalkerComplementation;
        }

        private void Ultimate()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                Spells.R.Cast(target.Position);
            }
        }

        private void LoadLambdaExpressions()
        {
            ParamName = s => KoreanUtils.ParamName(MainMenu, s);
            GetParamBool = s => KoreanUtils.GetParamBool(MainMenu, s);
            SetValueBool = (s, b) => KoreanUtils.SetValueBool(MainMenu, s, b);
            GetParamSlider = s => KoreanUtils.GetParamSlider(MainMenu, s);
            GetParamKeyBind = s => KoreanUtils.GetParamKeyBind(MainMenu, s);
            CanFarm = () => (!GetParamBool("supportmode")) || ((GetParamBool("supportmode")) && (Player.CountAlliesInRange(1500f) == 1));
            CheckStun = () => Player.HasBuff("pyromania_particle", true);
            SaveStun = () => (CheckStun() && (GetParamBool("savestunforcombo")));
        }

        private void CancelingAAOnSupportMode(Orbwalking.BeforeAttackEventArgs args)
        {
            if ((args.Target is Obj_AI_Base) && (((Obj_AI_Base)args.Target).IsMinion) && (!CanFarm()))
            {
                args.Process = false;
            }
        }

        void StunGapCloser(ActiveGapcloser gapcloser)
        {
            if (GetParamBool("antigapcloser") && (CheckStun()))
            {
                if ((Spells.Q.IsReady()) && (gapcloser.Sender.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.Cast(gapcloser.Sender);
                }
                else if ((Spells.W.IsReady()) && (gapcloser.Sender.IsValidTarget(Spells.W.Range)))
                {
                    Spells.W.Cast(gapcloser.Sender);
                }
            }
        }

        void InterruptDangerousSpells(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if ((GetParamBool("interruptspells")) && (CheckStun()) && (sender.IsEnemy) && 
                (args.DangerLevel > Interrupter2.DangerLevel.Medium))
            {
                if ((Spells.Q.IsReady()) && (sender.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.Cast(sender);
                }
                if ((Spells.W.IsReady()) && (sender.IsValidTarget(Spells.W.Range)))
                {
                    Spells.W.Cast(sender);
                }
            }
        }

        void EAgainstEnemyAA(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if ((GetParamBool("useeagainstaa")) && 
                (!sender.IsMe) && (sender.IsEnemy) && (sender is Obj_AI_Hero) && 
                (args.Target != null) && (args.Target.IsMe) &&
                (Player.Distance(args.End) < 440) &&
                (args.SData.Name.ToLowerInvariant().Contains("attack")))
            {
                Spells.E.Cast();
            }
        }

        public void LastHitMode()
        {
            if (!SaveStun())
            {
                QFarmLogic();
            }
        }

        public void MixedMode()
        {
            LastHitMode();
            Haras();
        }

        public void LaneClearMode()
        {
            if ((!SaveStun()) && (CanFarm()))
            {
                bool manaLimitReached = Player.ManaPercent < GetParamSlider("manalimittolaneclear");

                if ((GetParamBool("useqtolaneclear")) && (Spells.Q.IsReady()))
                {
                    if (GetParamBool("saveqtofarm"))
                    {
                        QFarmLogic();
                    }
                    else if (!manaLimitReached)
                    {
                        List<Obj_AI_Base> minions = MinionManager.GetMinions(Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);

                        if ((minions != null) && (minions.Count > 0))
                        {
                            Spells.Q.Cast(minions[0]);
                        }
                    }
                }

                if (!manaLimitReached)
                {
                    if ((GetParamBool("usewtolaneclear")) && (Spells.W.IsReady()))
                    {
                        List<Obj_AI_Base> Minions = MinionManager.GetMinions(Player.Position, Spells.W.Range);

                        MinionManager.FarmLocation WFarmLocation = Spells.W.GetCircularFarmLocation(Minions, Spells.W.Width);

                        if (WFarmLocation.MinionsHit >= GetParamSlider("minminionstow"))
                        {
                            Spells.W.Cast(WFarmLocation.Position);
                        }
                    }
                }
            }

            if (GetParamBool("harasonlaneclear"))
            {
                Haras();
            }
        }

        public void ComboMode()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Spells.MaxRangeForCombo(), TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if ((Spells.R.IsReady()) && (GetParamBool("usertocombo")) && (target.IsValidTarget(Spells.R.Range)) && (!Spells.CheckOverkill(target)))
            {
                int minEnemiesToR = GetParamSlider("minenemiestor");

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
            if ((Spells.W.IsReady()) && (GetParamBool("usewtocombo")) && (target.IsValidTarget(Spells.W.Range)))
            {
                Spells.W.Cast(target);
            }
            if ((Spells.Q.IsReady()) && (GetParamBool("useqtocombo")) && (target.IsValidTarget(Spells.Q.Range)))
            {
                Spells.Q.Cast(target);
            }
            if ((!GetParamBool("supportmode")) && (Spells.GetMaxDamage(target) > target.Health * 1.02f))
            {
                Ultimate();
            }
        }

        private void QFarmLogic()
        {
            if ((!SaveStun()) && (CanFarm()))
            {
                if ((Spells.Q.IsReady()) && (GetParamBool("useqtofarm")))
                {
                    List<Obj_AI_Base> Minions = MinionManager.GetMinions(Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth)
                        .Where(x => Spells.Q.IsKillable(x)).ToList();

                    if ((Minions != null) && (Minions.Count > 0))
                    {
                        Spells.Q.Cast(Minions[0]);
                    }
                }
            }
        }

        private void Haras()
        {
            bool manaLimitReached = Player.ManaPercent < GetParamSlider("manalimittoharas");
            if (!manaLimitReached)
            {
                Obj_AI_Hero target = TargetSelector.GetTarget(Spells.MaxRangeForHaras(), TargetSelector.DamageType.Magical);

                if ((Spells.Q.IsReady()) && (GetParamBool("useqtoharas")) && (target.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.Cast(target);
                }

                if ((Spells.W.IsReady()) && (GetParamBool("usewtoharas")) && (target.IsValidTarget(Spells.W.Range)))
                {
                    Spells.W.Cast(target.Position);
                }
            }
        }

        void OrbwalkerComplementation(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHitMode();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    MixedMode();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClearMode();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    ComboMode();
                    break;
            }
        }

    }
}
