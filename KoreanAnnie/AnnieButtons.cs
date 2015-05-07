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

        private Annie annie;

        public AnnieButtons(Annie annie)
        {
            this.annie = annie;

            ShowEasyButton = (KoreanUtils.GetParamBool(annie.MainMenu, "showeeasybutton"));

            int posX = KoreanUtils.GetParamInt(annie.MainMenu, "easybuttonpositionx");
            posX = (posX == 0) ? 50 : posX;
            int posY = KoreanUtils.GetParamInt(annie.MainMenu, "easybuttonpositiony");
            posY = (posY == 0) ? 50 : posY;
            Vector2 pos = new Vector2(posX, posY);

            stunButtonON = new Render.Sprite(Properties.Resources.StunON, pos);
            stunButtonON.Scale = new Vector2(0.90f, 0.90f);
            stunButtonON.Add();

            stunButtonOFF = new Render.Sprite(Properties.Resources.StunOFF, pos);
            stunButtonOFF.Scale = new Vector2(0.90f, 0.90f);
            stunButtonOFF.Add();

            if (ShowEasyButton) 
            {
                stunButtonON.Visible = KoreanUtils.GetParamBool(annie.MainMenu, "savestunforcombo");
                stunButtonOFF.Visible = !stunButtonON.Visible;
            }

            KoreanUtils.GetParam(annie.MainMenu, "savestunforcombo").ValueChanged += SaveStunForCombo_ValueChanged;
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
            ShowEasyButton = (KoreanUtils.GetParamBool(annie.MainMenu, "showeeasybutton"));

            if (ShowEasyButton)
            {
                if ((args.Msg == (uint)WindowsMessages.WM_LBUTTONUP) && (MouseOnButton(stunButtonON) || MouseOnButton(stunButtonOFF)))
                {
                    if (stunButtonON.Visible)
                    {
                        KoreanUtils.SetValueBool(annie.MainMenu, "savestunforcombo", false);
                    }
                    else if (stunButtonOFF.Visible)
                    {
                        KoreanUtils.SetValueBool(annie.MainMenu, "savestunforcombo", true);
                    }
                }
                else if ((args.Msg == (uint)WindowsMessages.WM_MOUSEMOVE) && (args.WParam == 5) && (MouseOnButton(stunButtonON) || MouseOnButton(stunButtonOFF)))
                {
                    MoveButtons(new Vector2(Utils.GetCursorPos().X - (stunButtonON.Width / 2), Utils.GetCursorPos().Y - 10));
                }

                stunButtonON.Visible = KoreanUtils.GetParamBool(annie.MainMenu, "savestunforcombo");
                stunButtonOFF.Visible = !stunButtonON.Visible;
            }
            else
            {
                stunButtonOFF.Visible = false;
                stunButtonON.Visible = false;
            }
        }

        private void MoveButtons(Vector2 newPosition)
        {
            stunButtonON.Position = newPosition;
            stunButtonON.PositionUpdate = () => newPosition;
            stunButtonON.PositionUpdate = null;

            stunButtonOFF.Position = newPosition;
            stunButtonOFF.PositionUpdate = () => newPosition;
            stunButtonOFF.PositionUpdate = null;

            KoreanUtils.SetValueInt(annie.MainMenu, "easybuttonpositionx", (int)stunButtonON.Position.X);
            KoreanUtils.SetValueInt(annie.MainMenu, "easybuttonpositiony", (int)stunButtonON.Position.Y);
        }
    }
}
