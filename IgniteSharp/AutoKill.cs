using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace IgniteSharp
{
    public class AutoKill
    {
        private readonly Spell ignite;

        public AutoKill(Ignite obj)
        {
            this.ignite = obj.SummonerDot;

            if (obj.menu.Get().Item("ignite#autokill").GetValue<bool>())
            {
                Game.OnUpdate += CastIgnite;
            }
        }

        public void CastIgnite(EventArgs args)
        {
            ObjectManager.Get<Obj_AI_Hero>()
                .Where(
                    h =>
                        h.IsValidTarget(ignite.Range) &&
                        h.Health < ObjectManager.Player.GetSummonerSpellDamage(h, Damage.SummonerSpell.Ignite))
                .Any(enemy => ignite.Cast(enemy).IsCasted());
        }
    }
}
