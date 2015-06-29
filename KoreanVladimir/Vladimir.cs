using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanVladimir
{
    using KoreanCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    class Vladimir : CommonChampion
    {
        public Vladimir(string menuDisplay) : base(menuDisplay)
        {
            KoreanVladimir.Spells.Load(this);
            CustomMenu.Load(this);
            VladCore = new Core(this);
            KeepEAlive = new KeepEAlive(this);
            ForceUltimate.ForceUltimate = VladCore.Ultimate;
        }

        public Core VladCore;
        public KeepEAlive KeepEAlive;
    }
}
