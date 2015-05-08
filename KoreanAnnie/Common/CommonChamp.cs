using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    interface CommonChamp
    {
        CommonMenu MainMenu { get; set; }
        CommonDamageDrawing DrawDamage { get; set; }
        CommonForceUltimate ForceUltimate { get; set; }
        AnnieSpells Spells { get; set; } //TODO: make it generic
        Orbwalking.Orbwalker Orbwalker { get; set; }
        Obj_AI_Hero Player { get; set; }
        float UltimateRange { get; set; }
    }
}
