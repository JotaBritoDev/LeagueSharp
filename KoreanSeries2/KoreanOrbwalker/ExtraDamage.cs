namespace KoreanOrbwalker
{
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal struct ExtraDamageItem
    {
        #region Fields

        public float amount;

        public float expire;

        #endregion
    }

    internal class ExtraDamage
    {
        #region Fields

        private readonly List<ExtraDamageItem> extraDamageList;

        private readonly Orbwalker orbwalker;

        #endregion

        #region Constructors and Destructors

        public ExtraDamage(Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
            extraDamageList = new List<ExtraDamageItem>();
        }

        #endregion

        #region Public Methods and Operators

        public void Add(float amount, float expire = 0f)
        {
            extraDamageList.Add(new ExtraDamageItem() { amount = amount, expire = expire });
        }

        public float Get(Obj_AI_Base target)
        {
            var result = 0f;

            for (var i = extraDamageList.Count - 1; i == 0; i--)
            {
                var item = extraDamageList[i];

                if (Game.Time > item.expire)
                {
                    extraDamageList.Remove(item);
                }
                else
                {
                    result += item.amount;
                }
            }

            if (ObjectManager.Player.HasBuff("caitlynheadshot"))
            {
                result += (float)ObjectManager.Player.GetAutoAttackDamage(target) * 1.5F;
            }
            if (ObjectManager.Player.HasBuff("cardmasterstackparticle"))
            {
                result += (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            }
            if (ObjectManager.Player.HasBuff("sheen"))
            {
                result += ObjectManager.Player.BaseAttackDamage;
            }
            if (ObjectManager.Player.HasBuff("lichbane"))
            {
                result += (ObjectManager.Player.BaseAttackDamage * 0.75f)
                          + (ObjectManager.Player.TotalMagicalDamage * 0.5f);
            }

            return result;
        }

        #endregion
    }
}