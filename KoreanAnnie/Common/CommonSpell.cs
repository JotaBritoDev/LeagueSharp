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

        public bool UseOnCombo
        {
            get { return UseOnComboMenu.GetValue<bool>(); }
        }

        public bool UseOnHaras
        {
            get { return UseOnHarasMenu.GetValue<bool>(); }
        }

        public bool UseOnLaneClear
        {
            get { return UseOnLaneClearMenu.GetValue<bool>(); }
        }
    }
}
