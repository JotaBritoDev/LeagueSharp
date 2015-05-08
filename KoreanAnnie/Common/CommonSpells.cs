using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    abstract class CommonSpells
    {
        Spell Q { get; set; }
        Spell W { get; set; }
        Spell E { get; set; }
        Spell R { get; set; }
        Spell RFlash { get; set; }
    }

    struct FlashStruct
    {
        public SpellSlot Slot;
        public bool IsReady;
    }

    static class FlashSpell
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
