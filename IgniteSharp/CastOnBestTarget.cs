using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace IgniteSharp
{
    class CastOnBestTarget
    {
        private readonly Spell ignite;

        private readonly Menu menu;

        public CastOnBestTarget(Ignite obj)
        {
            this.ignite = obj.SummonerDot;
            menu = obj.menu.Get();

            Game.OnWndProc += CastIgnite;
        }

        private void CastIgnite(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_KEYDOWN
                && ObjectManager.Player.Spellbook.ActiveSpellSlot == ignite.Slot
                && ignite.IsReady()
                && ObjectManager.Player.CountEnemiesInRange(ignite.Range) > 0)
            {
                Console.WriteLine("teste2");
                Obj_AI_Hero target;
                Console.WriteLine("teste2.1");
                if (menu.Item("ignite#target").GetValue<StringList>().SelectedIndex == 0)
                {
                    Console.WriteLine("teste3");
                    target =
                        HeroManager.Enemies.Where(hero => hero.IsValidTarget(ignite.Range))
                            .OrderByDescending(hero => hero.Health)
                            .First();
                    Console.WriteLine("teste4");
                }
                else
                {
                    Console.WriteLine("teste5");
                    target = TargetSelector.GetTarget(ObjectManager.Player, ignite.Range, TargetSelector.DamageType.True);
                    Console.WriteLine("teste6");
                }

                if (target != null)
                {
                    Console.WriteLine("teste7");
                    ignite.CastOnUnit(target);
                    Console.WriteLine("teste8");
                }
            }
        }
    }
}
