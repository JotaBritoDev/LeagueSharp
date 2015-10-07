namespace KoreanOlaf
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    class OlafSpells
    {
        public OlafSpell Q { get; set; }
        public OlafSpell W { get; set; }
        public OlafSpell E { get; set; }
        public OlafSpell R { get; set; }

        public OlafSpells()
        {
            float qRange = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.CastRange;
            float qDelay = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.SpellCastTime;
            float qWidth = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.LineWidth;
            float qSpeed = 1500F; //ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.MissileSpeed;

            Q = new OlafSpell(SpellSlot.Q, qRange, TargetSelector.DamageType.Physical);
            Q.SetSkillshot(qDelay, qWidth, qSpeed, false, SkillshotType.SkillshotLine);

            W = new OlafSpell(SpellSlot.W);

            float eRange = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange;
            E = new OlafSpell(SpellSlot.E, eRange, TargetSelector.DamageType.True);

            R = new OlafSpell(SpellSlot.R, 1500F);
        }
    }
}
