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
    class AnnieButtons
    {
        public Render.Sprite stunButtonON { get; set; }
        public Render.Sprite stunButtonOFF { get; set; }
        public bool ShowEasyButton { get; set; }

        private readonly CommonMenu MainMenu;

        public AnnieButtons(CommonMenu MainMenu)
        {
            this.MainMenu = MainMenu;

            ShowEasyButton = (KoreanUtils.GetParamBool(MainMenu, "showeeasybutton"));

            stunButtonON = new Render.Sprite(Properties.Resources.StunON, new Vector2(Drawing.Width * 0.03f, Drawing.Height * 0.80f));
            stunButtonON.Scale = new Vector2(0.90f, 0.90f);
            stunButtonON.Add();

            stunButtonOFF = new Render.Sprite(Properties.Resources.StunOFF, new Vector2(Drawing.Width * 0.03f, Drawing.Height * 0.80f));
            stunButtonOFF.Scale = new Vector2(0.90f, 0.90f);
            stunButtonOFF.Add();

            if (ShowEasyButton) 
            {
                stunButtonON.Visible = KoreanUtils.GetParamBool(MainMenu, "savestunforcombo");
                stunButtonOFF.Visible = !stunButtonON.Visible;
            }

            KoreanUtils.GetParam(MainMenu, "savestunforcombo").ValueChanged += SaveStunForCombo_ValueChanged;
            Game.OnWndProc += ButtonControl;
        }

        public bool MouseOnButton(Render.Sprite button)
        {
            Vector2 pos = Utils.GetCursorPos();

            return ((pos.X >= button.Position.X) && pos.X <= (button.Position.X + button.Width) &&
                    pos.Y >= button.Position.Y && pos.Y <= (button.Position.Y + button.Height));

        }

        public void SaveStunForCombo_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (ShowEasyButton)
            {
                stunButtonON.Visible = e.GetNewValue<bool>();
                stunButtonOFF.Visible = !stunButtonON.Visible;
            }
        }

        public void ButtonControl(WndEventArgs args)
        {
            ShowEasyButton = (KoreanUtils.GetParamBool(MainMenu, "showeeasybutton"));

            if (ShowEasyButton)
            {
                if ((args.Msg == (uint)WindowsMessages.WM_LBUTTONUP) && (MouseOnButton(stunButtonON) || MouseOnButton(stunButtonOFF)))
                {
                    if (stunButtonON.Visible)
                    {
                        KoreanUtils.SetValueBool(MainMenu, "savestunforcombo", false);
                    }
                    else if (stunButtonOFF.Visible)
                    {
                        KoreanUtils.SetValueBool(MainMenu, "savestunforcombo", true);
                    }
                }
                else if ((args.Msg == (uint)WindowsMessages.WM_MOUSEMOVE) && (args.WParam == 5) && (MouseOnButton(stunButtonON) || MouseOnButton(stunButtonOFF)))
                {
                    Vector2 novaPosicao = new Vector2(Utils.GetCursorPos().X - (stunButtonON.Width / 2), Utils.GetCursorPos().Y - 10);

                    stunButtonON.Position = novaPosicao;
                    stunButtonON.PositionUpdate = () => novaPosicao;
                    stunButtonON.PositionUpdate = null;

                    stunButtonOFF.Position = novaPosicao;
                    stunButtonOFF.PositionUpdate = () => novaPosicao;
                    stunButtonOFF.PositionUpdate = null;
                }

                stunButtonON.Visible = KoreanUtils.GetParamBool(MainMenu, "savestunforcombo");
                stunButtonOFF.Visible = !stunButtonON.Visible;
            }
            else
            {
                stunButtonOFF.Visible = false;
                stunButtonON.Visible = false;
            }
        }
    }
}
