using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using KoreanCommon;
using Color = System.Drawing.Color;


namespace KoreanLucian
{
    using SharpDX;
    
    class SemiAutomaticE
    {
        private readonly Lucian lucian;

        private readonly Render.Text Text = new Render.Text(0, 0, "Tap E again to DASH\nTap CTRL to CANCEL", 23, new ColorBGRA(255, 255, 255, 200));

        private CommonSpell E;

        private bool _holding;

        public bool Holding
        {
            get { return _holding; }
        }

        public bool Cast(Obj_AI_Base target)
        {
            if (target.IsDead)
            {
                return false;
            }

            bool result = false;

            if (target.Distance(Game.CursorPos) <= lucian.Player.Distance(target)
                || target.Distance(Game.CursorPos) <= Orbwalking.GetRealAutoAttackRange(lucian.Player)
                || target.Distance(lucian.Player) + E.Range <= Orbwalking.GetRealAutoAttackRange(lucian.Player))
            {
                Process(false);
                result = E.Cast(Game.CursorPos);
            }
            else
            {
                Process(false);
                result = E.Cast(Vector3.SmoothStep(lucian.Player.Position, Game.CursorPos, 0.001f));
            }


            return result;
        }
        
        public SemiAutomaticE(Lucian lucian)
        {
            this.lucian = lucian;
            E = lucian.Spells.E;

            if (KoreanUtils.GetParamBool(lucian.MainMenu, "dashmode"))
            {
                Game.OnWndProc += Game_OnWndProc; 
            }
        }

        private void Process(bool b)
        {
            if (b)
            {
                _holding = true;
                LeagueSharp.Drawing.OnDraw += Drawing_OnDraw;
                Spellbook.OnCastSpell += HoldE;
            }
            else
            {
                _holding = false;
                LeagueSharp.Drawing.OnDraw -= Drawing_OnDraw;
                Spellbook.OnCastSpell -= HoldE;
            }
        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (KoreanUtils.GetParamBool(lucian.MainMenu, "drawingetext"))
            {
                Text.X = (int)lucian.Player.HPBarPosition.X + 5;
                Text.Y = (int)lucian.Player.HPBarPosition.Y + 180;
                Text.OnEndScene();
            }
        }

        public void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_KEYDOWN || E.Instance.Level == 0)
            {
                return;
            }

            if (args.WParam == 69 && !_holding)
            {
                Process(true);
            }
            else if (args.WParam == 69 && _holding || (args.WParam == 17))
            {
                Process(false);
            }
        }

        private void HoldE(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot != E.Slot)
            {
                return;
            }

            if (_holding)
            {
                args.Process = false;
            }
        }
    }
}
