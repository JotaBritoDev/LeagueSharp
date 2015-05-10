using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    static class AnnieSpells
    {
        static public void Load(CommonChampion champion)
        {
            CommonSpell Q = new CommonSpell(SpellSlot.Q, 625, TargetSelector.DamageType.Magical);
            CommonSpell W = new CommonSpell(SpellSlot.W, 550, TargetSelector.DamageType.Magical);
            CommonSpell E = new CommonSpell(SpellSlot.E, 0, TargetSelector.DamageType.Magical);
            CommonSpell R = new CommonSpell(SpellSlot.R, 600, TargetSelector.DamageType.Magical);
            
            Q.SetTargetted(0.25f, 1400f);
            W.SetSkillshot(0.5f, 250f, float.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.2f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            champion.Spells.AddSpell(Q);
            champion.Spells.AddSpell(W);
            champion.Spells.AddSpell(E);
            champion.Spells.AddSpell(R);
        }

    }
}
