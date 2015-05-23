using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanCommon
{
    public class CommonSpells : IEnumerable
    {
        public List<CommonSpell> SpellList;
        private CommonChampion champion;

        public CommonSpell Q
        {
            get { return SpellList.Where(x => x.Slot == SpellSlot.Q).First(); }
        }

        public CommonSpell W
        {
            get { return SpellList.Where(x => x.Slot == SpellSlot.W).First(); }
        }

        public CommonSpell E
        {
            get { return SpellList.Where(x => x.Slot == SpellSlot.E).First(); }
        }

        public CommonSpell R
        {
            get { return SpellList.Where(x => x.Slot == SpellSlot.R).First(); }
        }

        private CommonSpell _rflash;

        public CommonSpell RFlash
        {
            get
            {
                if (_rflash == null && R != null)
                {
                    _rflash = new CommonSpell(R.Slot, R.Range + 400, R.DamageType);

                    if (R.IsSkillshot)
                    {
                        _rflash.SetSkillshot(R.Delay, R.Width, R.Speed, R.Collision, R.Type, R.From, R.RangeCheckFrom);
                    }
                }

                return _rflash;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return SpellList.GetEnumerator();
        }

        public CommonSpells(CommonChampion player)
        {
            this.champion = player;

            SpellList = new List<CommonSpell>();
        }

        public float MaxRangeCombo
        {
            get
            {
                float range = 0f;
                if (SpellList != null && SpellList.Count > 0)
                {
                    CommonSpell ultimate = SpellList.
                            Where(x => x.Slot == SpellSlot.R).First();
                    if (ultimate != null && ultimate.UseOnCombo && ultimate.IsReady() && ultimate.CanCast())
                    {
                        range = ultimate.Range;
                    }
                    else
                    {
                        List<CommonSpell> Spells = SpellList.Where(x => x.Slot != SpellSlot.R && x.UseOnCombo && x.IsReady() && x.CanCast()).ToList();
                        if (Spells != null && Spells.Count > 0)
                        {
                            foreach (CommonSpell spell in Spells)
                            {
                                range = Math.Max(spell.Range, range);
                            }
                        }
                    }
                }

                return range;
            }
        }

        public float MaxRangeHaras
        {
            get
            {
                float range = 0f;

                if (SpellList != null && SpellList.Count > 0)
                {
                    List<CommonSpell> Spells = SpellList.Where(x => x.UseOnHaras && x.IsReady() && x.CanCast()).ToList();
                    if (Spells != null && Spells.Count > 0)
                    {
                        foreach (CommonSpell spell in Spells)
                        {
                            range = Math.Max(spell.Range, range);
                        }
                    }
                }

                return range;
            }
        }

        public float MaxComboDamage(Obj_AI_Hero target)
        {
            float damage = 0f;

            if (SpellList != null && SpellList.Count > 0)
            {
                List<CommonSpell> Spells = SpellList.Where(x => x.UseOnCombo && x.IsReady() && x.CanCast()).ToList();
                if (Spells != null && Spells.Count > 0)
                {
                    foreach (CommonSpell spell in Spells)
                    {
                        damage += spell.GetDamage(target);
                    }
                }
            }

            return damage;
        }

        public void AddSpell(CommonSpell spell)
        {
            char slot = char.MinValue;

            switch (spell.Slot)
            {
                case (SpellSlot.Q):
                    slot = 'q';
                    break;
                case (SpellSlot.W):
                    slot = 'w';
                    break;
                case (SpellSlot.E):
                    slot = 'e';
                    break;
                case (SpellSlot.R):
                    slot = 'r';
                    break;
            }

            spell.UseOnComboMenu = KoreanUtils.GetParam(champion.MainMenu, string.Format("use{0}tocombo", slot));
            spell.UseOnHarasMenu = KoreanUtils.GetParam(champion.MainMenu, string.Format("use{0}toharas", slot));
            spell.UseOnLaneClearMenu = KoreanUtils.GetParam(champion.MainMenu, string.Format("use{0}tolaneclear", slot));
            SpellList.Add(spell);
        }

        public void RemoveSpell(CommonSpell spell)
        {
            SpellList.Remove(spell);
        }

        public bool CheckOverkill(Obj_AI_Hero target)
        {
            float totalDamage = 0f;

            if (R.IsReady() && R.CanCast() && target.IsValidTarget(R.Range))
            {
                List<CommonSpell> Spells = SpellList.
                    Where(x => x.CanCast() && x.IsReady() && target.IsValidTarget(x.Range) &&
                               x.IsKillable(target) && x.Slot != SpellSlot.R).ToList();

                if (Spells != null && Spells.Count > 0)
                {
                    foreach (CommonSpell spell in Spells)
                    {
                        totalDamage += spell.GetDamage(target);
                    }
                }
            }

            return totalDamage > target.Health;

            //return ((Q.IsReady()) && (target.IsValidTarget(Q.Range)) && (Q.IsKillable(target))) ||
            //       ((E.IsReady()) && (target.IsValidTarget(E.Range)) && (E.IsKillable(target))) ||
            //       ((W.IsReady()) && (target.IsValidTarget(W.Range)) && (W.IsKillable(target)));
        }

        public bool HarasReady()
        {
            return SpellList.Where(x => x.UseOnHaras && x.IsReady() && x.CanCast()).Count() == SpellList.Where(x => x.UseOnHaras).Count();
        }

        public bool ComboReady()
        {
            return SpellList.Where(x => x.UseOnCombo && x.IsReady() && x.CanCast()).Count() == SpellList.Where(x => x.UseOnCombo).Count();
        }

        public bool SomeSkillReady()
        {
            return SpellList.Where(x => x.Range > 0 && x.IsReady() && x.CanCast()).Count() > 0;
        }
    }

    public struct FlashStruct
    {
        public SpellSlot Slot;
        public bool IsReady;
    }

    public static class FlashSpell
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
