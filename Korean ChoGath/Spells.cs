using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoreanCommon;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanChoGath
{
    public static class Spells
    {
        public static void Load(CommonChampion champion)
        {
            CommonSpell Q = new CommonSpell(SpellSlot.Q, 950, TargetSelector.DamageType.Magical);
            CommonSpell W = new CommonSpell(SpellSlot.W, 650, TargetSelector.DamageType.Magical);
            CommonSpell E = new CommonSpell(SpellSlot.E, 0, TargetSelector.DamageType.Magical);
            CommonSpell R = new CommonSpell(SpellSlot.R, 175, TargetSelector.DamageType.Magical);

            Q.SetSkillshot(0.4f, 250f, 800f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 250f, float.MaxValue, false, SkillshotType.SkillshotCone);

            champion.Spells.AddSpell(Q);
            champion.Spells.AddSpell(W);
            champion.Spells.AddSpell(E);
            champion.Spells.AddSpell(R);
        }
    }
}
