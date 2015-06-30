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

    internal static class CustomMenu
    {
        public static void Load(CommonChampion vladimir)
        {
            HarasMenu(vladimir);
            LaneClearMenu(vladimir);
            ComboMenu(vladimir);
            OptionsMenu(vladimir);
        }

        private static void HarasMenu(CommonChampion vladimir)
        {
            var menu = vladimir.MainMenu;

            menu.HarasMenu.Items.Remove(KoreanUtils.GetParam(menu, "manalimittoharas"));
            KoreanUtils.GetParam(menu, "usewtoharas").SetValue(false);
            menu.HarasMenu.Items.Remove(KoreanUtils.GetParam(menu, "usewtoharas"));
        }

        private static void LaneClearMenu(CommonChampion vladimir)
        {
            var menu = vladimir.MainMenu;

            menu.LaneClearMenu.Items.Remove(KoreanUtils.GetParam(menu, "manalimittolaneclear"));

            menu.LaneClearMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "saveqtofarm"), "Save Q to last hit").SetValue(true));

            menu.LaneClearMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "minminionstoe"), "Use E if will hit x minions").SetValue(
                    new Slider(6, 1, 10)));
        }

        private static void ComboMenu(CommonChampion vladimir)
        {

        }

        private static void OptionsMenu(CommonChampion vladimir)
        {
            var menu = vladimir.MainMenu;

            menu.MiscMenu.AddItem(new MenuItem(KoreanUtils.ParamName(menu, "useqtofarm"), "Use Q to farm").SetValue(true));

            var autoStackMenu = menu.MiscMenu.AddSubMenu(new Menu("Auto stack E", "autostackemenu"));

            autoStackMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "autostackeactive"), "Active").SetValue(true));

            autoStackMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "autostackelimit"), "% Health limit").SetValue(new Slider(80, 0,
                    100)));

            autoStackMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(menu, "autostackehealthregen"),
                    "Health regen per 5sec to start stackin ").SetValue(new Slider(15, 10, 30)));

            menu.MiscMenu.AddItem(new MenuItem(KoreanUtils.ParamName(menu, "antigapcloser"), "Anti GapCloser").SetValue(true));
        }
    }
}
