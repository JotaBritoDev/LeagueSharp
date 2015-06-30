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
        public Core VladCore;

        public Vladimir(string menuDisplay) : base(menuDisplay)
        {
            KoreanVladimir.Spells.Load(this);
            CustomMenu.Load(this);
            VladCore = new Core(this);
            ForceUltimate.ForceUltimate = VladCore.Ultimate;
            var keepKillingYourself = new KeepEAlive(this);
            var cmonBaby = new FuckinAntiGapCloser(this);
            var draws = new Draws(this);
        }
    }
}
