namespace IgniteSharp
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class CastOnBestTarget
    {
        private readonly Spell ignite;

        private readonly Menu menu;

        public CastOnBestTarget(Ignite obj)
        {
            ignite = obj.SummonerDot;
            menu = obj.menu.Get();

            //Game.OnWndProc += CastIgnite;
            Game.OnUpdate += CastIgnite;
        }

        private void CastIgnite(EventArgs args)
        {
            if (menu.Item("ignite#key").GetValue<KeyBind>().Active)
            {
                Cast();
            }
        }

        private void CastIgnite(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_KEYDOWN
                && ObjectManager.Player.Spellbook.ActiveSpellSlot == ignite.Slot)
            {
                Cast();
            }
        }

        private void Cast()
        {
            if (!ignite.IsReady()
                || ObjectManager.Player.CountEnemiesInRange(ignite.Range) == 0)
            {
                return;
            }

            Obj_AI_Hero target;
            if (menu.Item("ignite#target").GetValue<StringList>().SelectedIndex == 0)
            {
                target =
                    HeroManager.Enemies.Where(hero => hero.IsValidTarget(ignite.Range))
                        .OrderBy(hero => hero.Health)
                        .First();
            }
            else
            {
                target = TargetSelector.GetTarget(
                    ObjectManager.Player,
                    ignite.Range,
                    TargetSelector.DamageType.True);
            }

            if (target != null)
            {
                ignite.CastOnUnit(target);
            }
        }
    }
}