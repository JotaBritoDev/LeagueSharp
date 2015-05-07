using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace KoreanAnnie
{
    class CommonDamageDrawing
    {
        private CommonChamp champion;
        private Color barColor = Color.Lime;
        private Color comboDamageColor = Color.FromArgb(100, Color.Black);

        public bool Active = true;
        private const int Width = 103;
        private const int Height = 8;

        private readonly Render.Text Text = new Render.Text(0, 0, "KILLABLE", 20, new ColorBGRA(255, 0, 0, 255));

        public delegate float DrawDamageDelegate(Obj_AI_Hero hero);

        private DrawDamageDelegate _amountOfDamage;

        public DrawDamageDelegate AmountOfDamage
        {
            get { return _amountOfDamage; }

            set
            {
                if (_amountOfDamage == null)
                {
                    Drawing.OnDraw += DrawDamage;
                }
                _amountOfDamage = value;
            }
        }

        public CommonDamageDrawing(CommonChamp champion)
        {
            this.champion = champion;
        }

        private bool Enabled()
        {
            return ((Active) && (_amountOfDamage != null) &&
                ((KoreanUtils.GetParamBool(champion.MainMenu, "damageindicator")) || KoreanUtils.GetParamBool(champion.MainMenu, "killableindicator")));
        }

        private void DrawDamage(EventArgs args)
        {
            if (Enabled())
            {
                foreach (var champ in ObjectManager.Get<Obj_AI_Hero>().
                    Where(h => h.IsVisible && h.IsEnemy && h.IsValid && h.IsHPBarRendered))
                {
                    float damage = _amountOfDamage(champ);

                    if (damage > 0)
                    {
                        Vector2 pos = champ.HPBarPosition;

                        if (KoreanUtils.GetParamBool(champion.MainMenu, "killableindicator") && (damage > champ.Health))
                        {
                            Render.Circle.DrawCircle(champ.Position, 100, Color.Red);
                            Render.Circle.DrawCircle(champ.Position, 75, Color.Red);
                            Render.Circle.DrawCircle(champ.Position, 50, Color.Red);
                            Text.X = (int)pos.X + 40;
                            Text.Y = (int)pos.Y - 20;
                            Text.OnEndScene();
                        }

                        if (KoreanUtils.GetParamBool(champion.MainMenu, "damageindicator"))
                        {
                            float healthAfterDamage = Math.Max(0, champ.Health - damage) / champ.MaxHealth;
                            float posY = pos.Y + 20f;
                            float posDamageX = pos.X + 12f + Width * healthAfterDamage;
                            float posCurrHealthX = pos.X + 12f + Width * champ.Health / champ.MaxHealth;

                            Drawing.DrawLine(posDamageX, posY, posDamageX, posY + Height, 2, barColor);

                            float diff = (posCurrHealthX - posDamageX) + 3;

                            var pos1 = pos.X + 9 + (107 * healthAfterDamage);

                            for (int i = 0; i < diff; i++)
                            {
                                Drawing.DrawLine(pos1 + i, posY, pos1 + i, posY + Height, 1, comboDamageColor);
                            }
                        }
                    }
                }
            }
        }
    }
}

