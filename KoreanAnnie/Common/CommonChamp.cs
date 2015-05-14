namespace KoreanAnnie.Common
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal interface ICommonChampion
    {
        #region Public Properties

        CommonDamageDrawing DrawDamage { get; set; }
        CommonForceUltimate ForceUltimate { get; set; }
        CommonMenu MainMenu { get; set; }
        Orbwalking.Orbwalker Orbwalker { get; set; }
        Obj_AI_Hero Player { get; set; }
        CommonSpells Spells { get; set; }
        float UltimateRange { get; set; }

        #endregion
    }
}