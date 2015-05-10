using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    class CommonSpell : Spell
    {
        public CommonSpell(SpellSlot slot, float range = 0, TargetSelector.DamageType damageType = TargetSelector.DamageType.Physical)
            : base (slot, range, damageType)
        {

        }

        public MenuItem UseOnComboMenu { get; set; }
        public MenuItem UseOnHarasMenu { get; set; }
        public MenuItem UseOnLaneClearMenu { get; set; }

        public bool CanCast()
        {
            return this.Instance.ToggleState <= 1;
        }

        public bool UseOnCombo
        {
            get 
            {
                bool b;

                if (UseOnComboMenu != null)
                {
                    b = UseOnComboMenu.GetValue<bool>();
                }
                else
                {
                    b = false;
                }
                return b;
            }
        }

        public bool UseOnHaras
        {
            get
            {
                bool b;

                if (UseOnHarasMenu != null)
                {
                    b = UseOnHarasMenu.GetValue<bool>();
                }
                else
                {
                    b = false;
                }
                return b;
            }
        }

        public bool UseOnLaneClear
        {
            get
            {
                bool b;

                if (UseOnComboMenu != null)
                {
                    b = UseOnLaneClearMenu.GetValue<bool>();
                }
                else
                {
                    b = false;
                }
                return b;
            }
        }

        public float LastTimeUsed
        {
            get
            {
                return this.Instance.CooldownExpires - this.Instance.Cooldown;
            }
        }

        public float UsedforXSecAgo
        {
            get
            {
                return Game.Time - this.LastTimeUsed;
            }
        }
    }
}
