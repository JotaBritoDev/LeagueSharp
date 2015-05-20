namespace IgniteSharp
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class AutoKill
    {
        private readonly Spell ignite;

        public AutoKill(Ignite obj)
        {
            ignite = obj.SummonerDot;

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
                    h.IsValidTarget(ignite.Range)
                    && h.Health < ObjectManager.Player.GetSummonerSpellDamage(h, Damage.SummonerSpell.Ignite))
                .Any(enemy => ignite.Cast(enemy).IsCasted());
        }
    }
}