namespace KoreanAnnie.Common
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class FlashSpell
    {
        #region Public Methods and Operators

        public static bool IsReady(Obj_AI_Hero player)
        {
            return ((Slot(player) != SpellSlot.Unknown)
                    && (player.Spellbook.CanUseSpell(Slot(player)) == SpellState.Ready));
        }

        public static SpellSlot Slot(Obj_AI_Hero player)
        {
            return player.GetSpellSlot("SummonerFlash");
        }

        #endregion
    }
}