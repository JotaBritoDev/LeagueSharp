using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanVladimir
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using KoreanCommon;

    class FuckinAntiGapCloser
    {
        private Vladimir vlady;
        private Spell W;

        public FuckinAntiGapCloser(Vladimir vladimir)
        {
            vlady = vladimir;
            W = vlady.Spells.W;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (KoreanUtils.GetParamBool(vlady.MainMenu, "antigapcloser") && W.IsReady())
            {
                W.Cast();
            }
        }
    }
}
