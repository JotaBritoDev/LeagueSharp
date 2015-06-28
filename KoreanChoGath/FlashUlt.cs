namespace KoreanChoGath
{
    using System;
    using System.Linq;

    using KoreanCommon;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class FlashUlt
    {
        private readonly ChoGath choGath;

        private FlashStruct flash;

        public FlashUlt(ChoGath choGath)
        {
            this.choGath = choGath;
            flash = FlashSpell.Flash(choGath.Player);

            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (choGath.Spells.R.IsReady() && flash.IsReady && KoreanUtils.GetParamKeyBind(choGath.MainMenu, "flashult"))
            {
                Obj_AI_Hero target =
                    HeroManager.Enemies.FirstOrDefault(
                        champ =>
                        choGath.Spells.RFlash.Range - 20f >= choGath.Player.Distance(champ)
                        && choGath.Spells.R.GetDamage(champ) > champ.Health + 20f);

                if (target != null)
                {
                    choGath.Player.Spellbook.CastSpell(flash.Slot, target.Position);
                    Utility.DelayAction.Add(50, () => choGath.Spells.R.Cast(target.Position));
                }

                if (KoreanUtils.GetParamBool(choGath.MainMenu, "orbwalktoflashult"))
                {
                    choGath.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
                choGath.ChoGathOrbwalker.ComboMode();
            }
        }
    }
}