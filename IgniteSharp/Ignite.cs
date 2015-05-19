namespace IgniteSharp
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Ignite
    {
        private readonly Spell ignite;

        private Menu menu;

        public Ignite()
        {
            ignite = new Spell(ObjectManager.Player.GetSpellSlot("SummonerDot"), 600);
            ignite.SetTargetted(0.1f, float.MaxValue);

            LoadMenu();

            //Spellbook.OnCastSpell += SmartIgnite;
            Game.OnWndProc += GetIgniteKey;
        }

        private void GetIgniteKey(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_KEYDOWN
                && ObjectManager.Player.Spellbook.ActiveSpellSlot == ignite.Slot
                && ignite.IsReady()
                && ObjectManager.Player.CountEnemiesInRange(ignite.Range) > 0)
            {
                Obj_AI_Hero target;

                if (menu.Item("ignite#target").GetValue<Slider>().Value == 0)
                {
                    target =
                        HeroManager.Enemies.Where(hero => hero.IsValidTarget(ignite.Range))
                            .OrderByDescending(hero => hero.Health)
                            .First();
                }
                else
                {
                    target = TargetSelector.GetTarget(ObjectManager.Player, ignite.Range, TargetSelector.DamageType.True);
                }

                if (target != null)
                {
                    ignite.Cast(target);
                }
            }
        }

        private void LoadMenu()
        {
            menu = new Menu("Ignite#", "ignite", true);
            menu.AddItem(
                new MenuItem("ignite#target", "Cast on").SetValue(
                    new StringList(new[] { "Lowest target", "Use target selector" })));
            menu.AddToMainMenu();
        }

        private void SmartIgnite(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == ignite.Slot)
            {
                args.Process = false;
            }
        }
    }
}