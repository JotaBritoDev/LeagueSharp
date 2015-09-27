namespace KoreanZed
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    class ZedAntiGapCloser
    {
        private readonly ZedMenu zedMenu;

        private readonly ZedSpell w;

        private readonly ZedSpell e;

        public ZedAntiGapCloser(ZedMenu menu, ZedSpells spells)
        {
            zedMenu = menu;
            w = spells.W;
            e = spells.E;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (ObjectManager.Player.Distance(gapcloser.Sender.ServerPosition) < e.Range)
            {
                if (zedMenu.GetParamBool("koreanzed.miscmenu.useeantigc") && e.IsReady())
                {
                    e.Cast();
                }

                if (zedMenu.GetParamBool("koreanzed.miscmenu.usewantigc") && w.IsReady())
                {
                    w.Cast(Vector3.Negate(gapcloser.Sender.Position));
                    w.Cast();
                }
            }
        }
    }
}
