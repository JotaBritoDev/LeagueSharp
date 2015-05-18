using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanCommon
{
    public abstract class CommonChampion
    {
        public CommonMenu MainMenu { get; set; }
        public CommonDamageDrawing DrawDamage { get; set; }
        public CommonForceUltimate ForceUltimate { get; set; }
        public CommonSpells Spells { get; set; }
        public Orbwalking.Orbwalker Orbwalker { get; set; }
        public Obj_AI_Hero Player { get; set; }

        public CommonChampion(string menuDisplay)
        {
            Player = ObjectManager.Player;

            MainMenu = new CommonMenu(menuDisplay, true);
            Orbwalker = MainMenu.Orbwalker;
            Spells = new CommonSpells(this);
            ForceUltimate = new CommonForceUltimate(this);
            DrawDamage = new CommonDamageDrawing(this);
            DrawDamage.AmountOfDamage = Spells.MaxComboDamage;
            DrawDamage.Active = true;
        }

    }
}
