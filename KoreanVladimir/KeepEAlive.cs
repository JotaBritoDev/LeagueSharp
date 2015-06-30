using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanVladimir
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using KoreanCommon;

    class KeepEAlive
    {
        private Vladimir Vladimir;
        private Spell E;
        private float NextCicleOn;

        public KeepEAlive(Vladimir vladimir)
        {
            Vladimir = vladimir;
            E = vladimir.Spells.E;
            NextCicleOn = 0f;

            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.E)
            {
                NextCicleOn = Game.Time + 9.8f;
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (!KoreanUtils.GetParamBool(Vladimir.MainMenu, "autostackeactive"))
            {
                return;
            }

            if (Vladimir.Player.IsRecalling() || Vladimir.Player.InFountain())
            {
                return;
            }

            if (Vladimir.Player.HealthPercent < KoreanUtils.GetParamSlider(Vladimir.MainMenu, "autostackelimit") ||
                Math.Round(Vladimir.Player.HPRegenRate*5, 0) <
                KoreanUtils.GetParamSlider(Vladimir.MainMenu, "autostackehealthregen"))
            {
                return;
            }

            if (NextCicleOn <= Game.Time)
            {
                Utility.DelayAction.Add(100, () => E.Cast()); 
            }
        }
    }
}
