using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanVladimir
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using KoreanCommon;

    public static class Spells
    {
        public static void Load(CommonChampion akali)
        {
            var Q = new CommonSpell(SpellSlot.Q, 600f, TargetSelector.DamageType.Magical);
            var W = new CommonSpell(SpellSlot.W, 0f, TargetSelector.DamageType.Magical);
            var E = new CommonSpell(SpellSlot.E, 610f, TargetSelector.DamageType.Magical);
            var R = new CommonSpell(SpellSlot.R, 625f, TargetSelector.DamageType.Magical);

            Q.SetTargetted(0.300f, 2500f);
            R.SetSkillshot(0.2f, 175f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            akali.Spells.AddSpell(Q);
            akali.Spells.AddSpell(W);
            akali.Spells.AddSpell(E);
            akali.Spells.AddSpell(R);
        }
    }
}
