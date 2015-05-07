using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoreanAnnie
{
    interface CommonChamp
    {
        CommonMenu MainMenu { get; set; }
        CommonDamageDrawing DrawDamage { get; set; }
        CommonForceUltimate ForceUltimate { get; set; }
        float UltimateRange { get; set; }
    }
}
