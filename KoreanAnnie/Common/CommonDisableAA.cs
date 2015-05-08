using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    enum CommonDisableAAMode
    {
        Never,
        Always,
        SomeSkill,
        FullCombo
    };

    class CommonDisableAA
    {
        private CommonChamp champion;

        public CommonDisableAAMode Mode
        {
            get { return (CommonDisableAAMode)KoreanUtils.GetParamStringList(champion.MainMenu, "disableaa"); }
        }

        public CommonDisableAA(CommonChamp champion)
        {
            this.champion = champion;

            Orbwalking.BeforeAttack += CancelAA;
        }

        private void CancelAA(Orbwalking.BeforeAttackEventArgs args)
        {
            if (champion.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                args.Process = true;
                return;
            }

            switch (Mode)
            {
                case CommonDisableAAMode.Always:
                    args.Process = false;
                    break;
                case CommonDisableAAMode.Never:
                    args.Process = true;
                    break;
                case CommonDisableAAMode.SomeSkill:
                    if (champion.Spells.Q.IsReady() || champion.Spells.W.IsReady() || champion.Spells.R.IsReady())
                    {
                        args.Process = false;
                    }
                    break;
                case CommonDisableAAMode.FullCombo:
                    Console.Clear();
                    Console.WriteLine("fulcombo");
                    if (champion.Spells.Q.IsReady() && champion.Spells.W.IsReady())
                    {
                        Console.WriteLine("canceling");
                        args.Process = false;
                    }
                    break;
            }
        }
    }
}
