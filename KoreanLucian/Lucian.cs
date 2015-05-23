using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using KoreanCommon;

namespace KoreanLucian
{
    class Lucian : CommonChampion
    {
        private readonly Core core;

        private readonly KoreanLucian.Drawing drawing;

        public readonly KillSteal killSteal;

        public Lucian() : 
            base ("Korean Lucian")
        {
            KoreanLucian.Spells.Load(this);
            core = new Core(this);
            CustomMenu.Load(this);
            ForceUltimate.ForceUltimate = core.Ultimate;
            drawing = new Drawing(this);
            killSteal = new KillSteal(this);
            DrawDamage.AmountOfDamage = KoreanLucian.Spells.MaxComboDamage;
        }
    }
}
