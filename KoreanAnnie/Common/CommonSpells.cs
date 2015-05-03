using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    struct FlashStruct
    {
        public SpellSlot Slot;
        public bool IsReady;
    }

    static class CommonSpells
    {
        public static FlashStruct Flash(Obj_AI_Hero Player)
        {
            FlashStruct flash = new FlashStruct();

            flash.Slot = ObjectManager.Player.GetSpellSlot("SummonerFlash");
            flash.IsReady = ((flash.Slot != SpellSlot.Unknown) && (Player.Spellbook.CanUseSpell(flash.Slot) == SpellState.Ready));

            return flash;
        }
    }
}
