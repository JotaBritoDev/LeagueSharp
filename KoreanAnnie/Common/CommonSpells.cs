using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    class CommonSpells : IEnumerable
    {
        public List<CommonSpell> SpellList;

        public IEnumerator GetEnumerator()
        {
            return SpellList.GetEnumerator();
        }

        public CommonSpells()
        {
            SpellList = new List<CommonSpell>();
        }

        public float MaxRangeCombo
        {
            get
            {
                float range = 0f;

                CommonSpell ultimate = SpellList.Where(x => x.Slot == SpellSlot.R).First();
                if (ultimate.IsReady())
                {
                    range = ultimate.Range;
                }
                else
                {
                    foreach (CommonSpell spell in SpellList)
                    {
                        if ((spell.IsReady()) && (spell.Range > range))
                        {
                            range = spell.IsReady() ? spell.Range : 0;
                        }
                    }
                }

                return range;
            } //TODO
        }

        public float MaxRangeHaras
        {
            get { return 0; } //TODO
        }

        public float MaxComboDamage(Obj_AI_Hero target)
        {
            float damage = 0f;

            foreach (CommonSpell spell in SpellList)
            {
                damage += spell.IsReady() ? spell.GetDamage(target) : 0;
            }

            return damage;
        }

        public void AddSpell(CommonSpell spell)
        {
            SpellList.Add(spell);
        }

        public void RemoveSpell(CommonSpell spell)
        {
            SpellList.Remove(spell);
        }
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
