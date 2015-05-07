using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    class AnnieSpells
    {
        public Spell Q { get; set; }
        public Spell W { get; set; }
        public Spell E { get; set; }
        public Spell R { get; set; }
        public Spell RFlash { get; set; }

        private const float QRange = 625;
        private const float WRange = 525;
        private const float ERange = 0;
        private const float RRange = 600;
        private const float RFlashRange = 995;

        private const float QDelay = 0.25f;
        private const float WDelay = 0.50f;
        private const float EDelay = 0f;
        private const float RDelay = 0.20f;
        private const float RFlashDelay = 0.25f;

        private const float QSpeed = 1400f;
        private const float WSpeed = float.MaxValue;
        private const float ESpeed = 0f;
        private const float RSpeed = float.MaxValue; 
        private const float RFlashSpeed = float.MaxValue;

        private const float QWidth = 0f;
        private const float WWidth = 250f;
        private const float EWidth = 0f;
        private const float RWidth = 250f;
        private const float RFlashWidth = 250f;

        private Annie annie;

        public AnnieSpells(Annie annie)
        {
            Q = new Spell(SpellSlot.Q, QRange);
            W = new Spell(SpellSlot.W, WRange);
            E = new Spell(SpellSlot.E, ERange);
            R = new Spell(SpellSlot.R, RRange);
            RFlash = new Spell(SpellSlot.R, RFlashRange);

            Q.SetTargetted(QDelay, QSpeed);
            W.SetSkillshot(WDelay, WWidth, W.Speed, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(RDelay, RWidth, R.Speed, false, SkillshotType.SkillshotCircle);
            RFlash.SetSkillshot(RFlashDelay, RFlashWidth, RFlashSpeed, false, SkillshotType.SkillshotCircle);

            this.annie = annie;

            Game.OnUpdate += StackE;
        }

        public float MaxRangeForCombo()
        {
            return R.IsReady() ? R.Range : MaxRangeForHaras();
        }

        public float MaxRangeForHaras()
        {
            if ((Q.IsReady()) && (W.IsReady()))
            {
                return Math.Max(Q.Range, W.Range);
            }
            else if (Q.IsReady())
            {
                return Q.Range;
            }
            else if (W.IsReady())
            {
                return W.Range;
            }
            else
            {
                return 0f;
            }
        }

        public bool CheckOverkill(Obj_AI_Hero target)
        {
            return ((Q.IsReady()) && (target.IsValidTarget(Q.Range)) && (Q.IsKillable(target))) ||
                   ((W.IsReady()) && (target.IsValidTarget(W.Range)) && (W.IsKillable(target)));
        }

        private void StackE(EventArgs args)
        {
            if ((!annie.Player.IsRecalling()) && (!annie.CheckStun()) && (annie.GetParamBool("useetostack")) && (annie.Player.ManaPercent > annie.GetParamSlider("manalimitforstacking")) &&
                (E.IsReady()))
            {
                E.Cast();
            }
        }

        public float GetMaxDamage(Obj_AI_Hero champ)
        {
            float maxDamage = 0f;

            if (champ != null)
            {
                if (Q.IsReady())
                    maxDamage += (float)annie.Player.GetSpellDamage(champ, SpellSlot.Q);

                if (W.IsReady())
                    maxDamage += (float)annie.Player.GetSpellDamage(champ, SpellSlot.W);

                if (R.IsReady())
                    maxDamage += (float)annie.Player.GetSpellDamage(champ, SpellSlot.R);
            }

            return maxDamage * 0.97f;
        }

    }
}
