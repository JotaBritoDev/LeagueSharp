using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using KoreanCommon;

namespace KoreanLucian
{
    static class CustomMenu
    {
        static public void Load(CommonChampion champion)
        {
            LoadLaneClearMenu(champion);
            LoadMiscMenu(champion);
            LoadComboMenu(champion);
        }

        private static void LoadComboMenu(CommonChampion champion)
        {
            Menu menu = champion.MainMenu.ComboMenu;

            MenuItem useRToCombo = menu.Item(
                KoreanUtils.ParamName(champion.MainMenu, "usertocombo"));
            useRToCombo.SetValue(false);
            menu.Items.Remove(useRToCombo);

            menu.Items.Remove(menu.Item(KoreanUtils.ParamName(champion.MainMenu, "minenemiestor")));
            menu.Items.Remove(menu.Item(KoreanUtils.ParamName(champion.MainMenu, "disableaa")));
        }

        private static void LoadMiscMenu(CommonChampion champion)
        {
            Menu menu = champion.MainMenu.MiscMenu;

            Menu SemiAutoE = menu.AddSubMenu(new Menu("Semi-Automatic E", "semiautoe"));

            MenuItem semiAutomaticE = new MenuItem(KoreanUtils.ParamName(champion.MainMenu, "dashmode"), "Active").SetValue(true);

            SemiAutoE.AddItem(semiAutomaticE);
            semiAutomaticE.ValueChanged += delegate(object sender, OnValueChangeEventArgs e)
            {
                if (e.GetNewValue<bool>())
                {
                    Game.OnWndProc += ((Lucian)champion).semiAutomaticE.Game_OnWndProc; 
                }
                else
                {
                    Game.OnWndProc -= ((Lucian)champion).semiAutomaticE.Game_OnWndProc; 
                }
            };

            SemiAutoE.AddItem(new MenuItem("drawingetext", "Drawing text").SetValue(true));

            menu.AddItem(new MenuItem(KoreanUtils.ParamName(champion.MainMenu, "lockr"), "Lock R").SetValue(true));
            menu.AddItem(new MenuItem(KoreanUtils.ParamName(champion.MainMenu, "useyoumuu"), "Use Youmuu before R").SetValue(true));
            menu.AddItem(new MenuItem(KoreanUtils.ParamName(champion.MainMenu, "extendedq"), "Extended Q").SetValue(true));

            MenuItem ksOption = menu.AddItem(
                new MenuItem(KoreanUtils.ParamName(champion.MainMenu, "killsteal"), "Smart KillSteal").SetValue(true));

            ksOption.ValueChanged += delegate(object sender, OnValueChangeEventArgs e)
            {
                if (e.GetNewValue<bool>())
                {
                    Game.OnUpdate += ((Lucian)champion).killSteal.KS;
                }
                else
                {
                    Game.OnUpdate -= ((Lucian)champion).killSteal.KS;
                }
            };
        }

        private static void LoadLaneClearMenu(CommonChampion champion)
        {
            Menu menu = champion.MainMenu.LaneClearMenu;

            menu.AddItem(new MenuItem(KoreanUtils.ParamName(champion.MainMenu, "qcounthit"), "Q must hit")
                .SetValue(new Slider(3, 0, 6)));

            menu.AddItem(new MenuItem(KoreanUtils.ParamName(champion.MainMenu, "wcounthit"), "W must hit")
                .SetValue(new Slider(2, 0, 6)));

            MenuItem harasOnLaneClear = menu.Item(KoreanUtils.ParamName(champion.MainMenu, "harasonlaneclear"));
            menu.Items.Remove(harasOnLaneClear);
            menu.Items.Add(harasOnLaneClear);
        }
    }
}
