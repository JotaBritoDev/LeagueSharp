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
                    ignite.CastOnUnit(target);
                }
            }
        }
    }
}
