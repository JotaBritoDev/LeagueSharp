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
        public AnnieOrbwalkComplementation AnnieOrbwalker { get; set; }
        public AnnieDrawings Draws { get; set; }
        public CommonDamageDrawing DrawDamage { get; set; }
        public CommonForceUltimate ForceUltimate { get; set; }
        public Orbwalking.Orbwalker Orbwalker { get; set; }
        public Obj_AI_Hero Player { get; set; }
        public CommonDisableAA DisableAA { get; set; }

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
            AnnieOrbwalker = new AnnieOrbwalkComplementation(this);

            Draws = new AnnieDrawings(this);
            DrawDamage = new CommonDamageDrawing(this);
            ForceUltimate = new CommonForceUltimate(this);
            UltimateRange = Spells.R.Range;

            ForceUltimate.ForceUltimate = AnnieOrbwalker.Ultimate;
            DrawDamage.AmountOfDamage = Spells.GetMaxDamage;
            DrawDamage.Active = true;

            DisableAA = new CommonDisableAA(this);

            Obj_AI_Base.OnProcessSpellCast += EAgainstEnemyAA;
            Interrupter2.OnInterruptableTarget += InterruptDangerousSpells;
            AntiGapcloser.OnEnemyGapcloser += StunGapCloser;
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

    }
}
