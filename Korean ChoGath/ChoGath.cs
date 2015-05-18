using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using KoreanCommon;

namespace KoreanChoGath
{
    class ChoGath : CommonChampion
    {
        public OrbwalkComplementation ChoGathOrbwalker { get; set; }
        public SmartE smartE { get; set; }
        public CancelAA cancelAA { get; set; }
        public StackPassive stackPassive { get; set; }
        public Drawing drawing { get; set; }

        public ChoGath() : base("Korean Cho'Gath")
        {
            KoreanChoGath.Spells.Load(this);
            ChoGathOrbwalker = new OrbwalkComplementation(this);
            ForceUltimate.ForceUltimate = ChoGathOrbwalker.Ultimate;

            CustomMenu.Load(this);
            smartE = new SmartE(this);
            cancelAA = new CancelAA(this);
            stackPassive = new StackPassive(this);
            drawing = new Drawing(this);
        }
    }
}
