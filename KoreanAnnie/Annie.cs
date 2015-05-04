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
    class Annie
    {
        private string menuDisplay = "Korean Annie";
        private Orbwalking.Orbwalker Orbwalker;
        private Obj_AI_Hero Player;
        private CommonMenu MainMenu;
        private Func<string, bool> GetParamBool;
        private Action<string, bool> SetValueBool;
        private Func<string, int> GetParamInt;
        private Func<string, bool> GetParamKeyBind;
        private Func<string, string> ParamName;
        private Func<bool> CanFarm;
        private Func<bool> SaveStun;
        private Func<bool> CheckStun;

        AnnieSpells annieSpells;
        AnnieButtons annieButtons;
        AnnieTibbers annieTibbers;

        public Annie()
        {
            Player = ObjectManager.Player;
            MainMenu = new CommonMenu(menuDisplay, true);
            Orbwalker = MainMenu.Orbwalker;

            AnnieCustomMenu.Load(MainMenu);

            annieSpells = new AnnieSpells();
            annieButtons = new AnnieButtons(MainMenu);
            annieTibbers = new AnnieTibbers(Player);

            ParamName = s => KoreanUtils.ParamName(MainMenu, s);
            GetParamBool = s => KoreanUtils.GetParamBool(MainMenu, s);
            SetValueBool = (s, b) => KoreanUtils.SetValueBool(MainMenu, s, b);
            GetParamInt = s => KoreanUtils.GetParamInt(MainMenu, s);
            GetParamKeyBind = s => KoreanUtils.GetParamKeyBind(MainMenu, s);
            CanFarm = () => (!GetParamBool("supportmode")) || ((GetParamBool("supportmode")) && (Player.CountAlliesInRange(1500f) == 1));
            CheckStun = () => Player.HasBuff("pyromania_particle", true);
            SaveStun = () => (CheckStun() && (GetParamBool("savestunforcombo")));

            Obj_AI_Base.OnProcessSpellCast += EAgainstEnemyAA;
            Interrupter2.OnInterruptableTarget += InterruptDangerousSpells;
            AntiGapcloser.OnEnemyGapcloser += StunGapCloser;
            Orbwalking.BeforeAttack += CancelingAAOnSupportMode;
            Game.OnUpdate += StackE;
            Game.OnUpdate += FlashTibbersLogic;
            Game.OnUpdate += OrbwalkerComplementation;
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
                if ((annieSpells.Q.IsReady()) && (gapcloser.Sender.IsValidTarget(annieSpells.Q.Range)))
                {
                    annieSpells.Q.Cast(gapcloser.Sender);
                }
                else if ((annieSpells.W.IsReady()) && (gapcloser.Sender.IsValidTarget(annieSpells.W.Range)))
                {
                    annieSpells.W.Cast(gapcloser.Sender);
                }
            }
        }

        void InterruptDangerousSpells(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if ((GetParamBool("interruptspells")) && (CheckStun()) && (sender.IsEnemy) && 
                (args.DangerLevel > Interrupter2.DangerLevel.Medium))
            {
                if ((annieSpells.Q.IsReady()) && (sender.IsValidTarget(annieSpells.Q.Range)))
                {
                    annieSpells.Q.Cast(sender);
                }
                if ((annieSpells.W.IsReady()) && (sender.IsValidTarget(annieSpells.W.Range)))
                {
                    annieSpells.W.Cast(sender);
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
                annieSpells.E.Cast();
            }
        }

        private void LastHitMode()
        {
            if (!SaveStun())
            {
                QFarmLogic();
            }
        }

        private void MixedMode()
        {
            LastHitMode();
            Haras();
        }

        private void LaneClearMode()
        {
            if ((!SaveStun()) && (CanFarm()))
            {
                bool manaLimitReached = Player.ManaPercent < GetParamInt("manalimittolaneclear");

                if ((GetParamBool("useqtolaneclear")) && (annieSpells.Q.IsReady()))
                {
                    if (GetParamBool("saveqtofarm"))
                    {
                        QFarmLogic();
                    }
                    else if (!manaLimitReached)
                    {
                        List<Obj_AI_Base> minions = MinionManager.GetMinions(Player.Position, annieSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);

                        if ((minions != null) && (minions.Count > 0))
                        {
                            annieSpells.Q.Cast(minions[0]);
                        }
                    }
                }

                if (!manaLimitReached)
                {
                    if ((GetParamBool("usewtolaneclear")) && (annieSpells.W.IsReady()))
                    {
                        List<Obj_AI_Base> Minions = MinionManager.GetMinions(Player.Position, annieSpells.W.Range);

                        MinionManager.FarmLocation WFarmLocation = annieSpells.W.GetCircularFarmLocation(Minions, annieSpells.W.Width);

                        if (WFarmLocation.MinionsHit >= GetParamInt("minminionstow"))
                        {
                            annieSpells.W.Cast(WFarmLocation.Position);
                        }
                    }
                }
            }

            if (GetParamBool("harasonlaneclear"))
            {
                Haras();
            }
        }

        private void ComboMode()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(annieSpells.MaxRangeForCombo(), TargetSelector.DamageType.Magical);

            if ((annieSpells.R.IsReady()) && (GetParamBool("usertocombo")) && (target.IsValidTarget(annieSpells.R.Range)) && (!annieSpells.CheckOverkill(target)))
            {
                int minEnemiesToR = GetParamInt("minenemiestor");

                if (minEnemiesToR == 1)
                {
                    annieSpells.R.Cast(target.Position);
                }
                else
                {
                    foreach (PredictionOutput pred in ObjectManager.Get<Obj_AI_Hero>().
                        Where(x => x.IsValidTarget(annieSpells.R.Range)).
                        Select(x => annieSpells.R.GetPrediction(x, true)).
                            Where(pred => pred.Hitchance >= HitChance.High && pred.AoeTargetsHitCount >= minEnemiesToR))
                    {
                        annieSpells.R.Cast(pred.CastPosition);
                    }
                }
            }
            if ((annieSpells.W.IsReady()) && (GetParamBool("usewtocombo")) && (target.IsValidTarget(annieSpells.W.Range)))
            {
                annieSpells.W.Cast(target.Position);
            }
            if ((annieSpells.Q.IsReady()) && (GetParamBool("useqtocombo")) && (target.IsValidTarget(annieSpells.Q.Range)))
            {
                annieSpells.Q.Cast(target);
            }
        }

        private void QFarmLogic()
        {
            if ((!SaveStun()) && (CanFarm()))
            {
                if ((annieSpells.Q.IsReady()) && (GetParamBool("useqtofarm")))
                {
                    List<Obj_AI_Base> Minions = MinionManager.GetMinions(Player.Position, annieSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth)
                        .Where(x => annieSpells.Q.IsKillable(x)).ToList();

                    if ((Minions != null) && (Minions.Count > 0))
                    {
                        annieSpells.Q.Cast(Minions[0]);
                    }
                }
            }
        }

        private void Haras()
        {
            bool manaLimitReached = Player.ManaPercent < GetParamInt("manalimittoharas");
            if (!manaLimitReached)
            {
                Obj_AI_Hero target = TargetSelector.GetTarget(annieSpells.MaxRangeForHaras(), TargetSelector.DamageType.Magical);

                if ((annieSpells.Q.IsReady()) && (GetParamBool("useqtoharas")) && (target.IsValidTarget(annieSpells.Q.Range)))
                {
                    annieSpells.Q.Cast(target);
                }

                if ((annieSpells.W.IsReady()) && (GetParamBool("usewtoharas")) && (target.IsValidTarget(annieSpells.W.Range)))
                {
                    annieSpells.W.Cast(target.Position);
                }
            }
        }

        private void StackE(EventArgs args)
        {
            if ((!Player.IsRecalling()) && (!CheckStun()) && (GetParamBool("useetostack")) && (Player.ManaPercent > GetParamInt("manalimitforstacking")) && 
                (annieSpells.E.IsReady()))
            {
                annieSpells.E.Cast();
            }
        }

        private void FlashTibbersLogic(EventArgs args)
        {
            if (GetParamKeyBind("flashtibbers"))
            {
                if ((annieSpells.R.IsReady()) && (CommonSpells.Flash(Player).IsReady) && (CheckStun()))
                {

                    foreach (PredictionOutput pred in ObjectManager.Get<Obj_AI_Hero>().
                        Where(x => x.IsValidTarget(annieSpells.RFlash.Range)).
                        Select(x => annieSpells.RFlash.GetPrediction(x, true)).
                            Where(pred => pred.Hitchance >= HitChance.High && pred.AoeTargetsHitCount >= GetParamInt("minenemiestoflashr")))
                    {
                        Player.Spellbook.CastSpell(CommonSpells.Flash(Player).Slot, pred.CastPosition);
                        Utility.DelayAction.Add(50, () => annieSpells.R.Cast(pred.CastPosition));
                    }
                }

                ComboMode();
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
