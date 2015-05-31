namespace KoreanChoGath
{
    using KoreanCommon;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class StackPassive
    {
        private readonly CommonChampion champion;

        private readonly Spell R;

        public StackPassive(CommonChampion champion)
        {
            R = champion.Spells.R;
            this.champion = champion;

            if (KoreanUtils.GetParamBool(champion.MainMenu, "autostackpassive"))
            {
                Orbwalking.BeforeAttack += StackR;
            }
        }

        public void StackR(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target is Obj_AI_Minion && R.IsReady() && R.CanCast((Obj_AI_Minion)args.Target)
                && champion.Player.Mana > R.Instance.ManaCost && R.IsKillable((Obj_AI_Minion)args.Target)
                && champion.Player.CountEnemiesInRange(3000f) == 0 && champion.Player.GetBuffCount("Feast") < 6)
            {
                args.Process = false;
                R.Cast((Obj_AI_Minion)args.Target);
            }
        }
    }
}