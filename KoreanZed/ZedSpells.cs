namespace KoreanZed
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    class ZedSpells
    {
        public ZedSpell Q { get; set; }
        public ZedSpell W { get; set; }
        public ZedSpell E { get; set; }
        public ZedSpell R { get; set; }

        public ZedSpells()
        {
            float qRange = 800F; //ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.CastRange; <<BUGGED
            float qDelay = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.SpellCastTime;
            float qWidth = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.LineWidth;
            float qSpeed = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.MissileSpeed;

            Q = new ZedSpell(SpellSlot.Q, qRange, TargetSelector.DamageType.Physical);
            Q.SetSkillshot(qDelay, qWidth, qSpeed, false, SkillshotType.SkillshotLine);

            float wRange = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.CastRange;
            W = new ZedSpell(SpellSlot.W, wRange, TargetSelector.DamageType.Physical);

            float eRange = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange;
            E = new ZedSpell(SpellSlot.E, eRange, TargetSelector.DamageType.Physical);

            float rRange = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.CastRange;
            R = new ZedSpell(SpellSlot.R, rRange, TargetSelector.DamageType.Physical);
        }
    }
}
