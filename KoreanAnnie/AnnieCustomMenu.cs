using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    static class AnnieCustomMenu
    {
        public static void Load(CommonMenu MainMenu)
        {
            RemoveItems(MainMenu);
            LoadLaneClear(MainMenu);
            LoadMisc(MainMenu);
        }

        private static void RemoveItems(CommonMenu MainMenu)
        {
            MainMenu.HarasMenu.Items.Remove(MainMenu.HarasMenu.Item(KoreanUtils.ParamName(MainMenu, "useetoharas")));
            MainMenu.LaneClearMenu.Items.Remove(MainMenu.LaneClearMenu.Item(KoreanUtils.ParamName(MainMenu, "useetolaneclear")));
            MainMenu.ComboMenu.Items.Remove(MainMenu.ComboMenu.Item(KoreanUtils.ParamName(MainMenu, "useetocombo")));
        }

        private static void LoadLaneClear(CommonMenu MainMenu)
        {
            MainMenu.LaneClearMenu.AddItem(new MenuItem(KoreanUtils.ParamName(MainMenu, "saveqtofarm"), "Save Q to farm").SetValue(true));
            MainMenu.LaneClearMenu.AddItem(new MenuItem(KoreanUtils.ParamName(MainMenu, "minminionstow"), "W must hit at least").SetValue(new Slider(3, 1, 6)));
        }

        private static void LoadMisc(CommonMenu MainMenu)
        {
            Menu PassiveStunMenu = MainMenu.MiscMenu.AddSubMenu(new Menu("Passive Stun", KoreanUtils.ParamName(MainMenu, "passivestunmenu")));
            PassiveStunMenu.AddItem(new MenuItem(KoreanUtils.ParamName(MainMenu, "useetostack"), "Use E to stack").SetValue(true));
            PassiveStunMenu.AddItem(new MenuItem(KoreanUtils.ParamName(MainMenu, "manalimitforstacking"), "Mana limit for stacking").SetValue(new Slider(30, 0, 100)));
            PassiveStunMenu.AddItem(new MenuItem(KoreanUtils.ParamName(MainMenu, "savestunforcombo"), "Save stun for combo/haras").SetValue(false));
            PassiveStunMenu.AddItem(new MenuItem(KoreanUtils.ParamName(MainMenu, "showeeasybutton"), "Show easy button").SetValue(true));

            MainMenu.MiscMenu.AddItem(new MenuItem(KoreanUtils.ParamName(MainMenu, "useqtofarm"), "Use Q to farm").SetValue(true));
            MainMenu.MiscMenu.AddItem(new MenuItem(KoreanUtils.ParamName(MainMenu, "useeagainstaa"), "Use E against AA").SetValue(true));
            MainMenu.MiscMenu.AddItem(new MenuItem(KoreanUtils.ParamName(MainMenu, "antigapcloser"), "Anti gap closer").SetValue(true));
            MainMenu.MiscMenu.AddItem(new MenuItem(KoreanUtils.ParamName(MainMenu, "interruptspells"), "Interrupt dangerous spells if possible").SetValue(true));
            MainMenu.MiscMenu.AddItem(new MenuItem(KoreanUtils.ParamName(MainMenu, "flashtibbers"), "Flash + Tibbers").SetValue(new KeyBind('T', KeyBindType.Press)));
        }
    }
}
