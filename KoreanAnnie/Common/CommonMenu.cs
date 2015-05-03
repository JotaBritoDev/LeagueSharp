using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace KoreanAnnie
{
    class CommonMenu
    {
        private Menu _mainMenu;

        public Menu MainMenu
        {
            get { return _mainMenu; }
        }

        private Menu _harasMenu;

        public Menu HarasMenu
        {
            get { return _harasMenu; }
        }

        private Menu _laneClearMenu;

        public Menu LaneClearMenu
        {
            get { return _laneClearMenu; }
        }

        private Menu _comboMenu;

        public Menu ComboMenu
        {
            get { return _comboMenu; }
        }

        private Menu _miscMenu;

        public Menu MiscMenu
        {
            get { return _miscMenu; }
        }

        private string _menuName;

        public string MenuName
        {
            get { return _menuName; }
        }

        private Orbwalking.Orbwalker _orbwalker;

	    public Orbwalking.Orbwalker Orbwalker
	    {
		    get { return _orbwalker;}
		    set { _orbwalker = value;}
	    }
        
        public CommonMenu(string displayName, bool misc)
        {
            _menuName = displayName.Replace(" ", "_").ToLowerInvariant(); ;

            _mainMenu = new Menu(displayName, _menuName, true);

            addOrbwalker(_mainMenu);
            addTargetSelector(_mainMenu);
            _harasMenu = addHarasMenu(_mainMenu);
            _laneClearMenu = addLaneClearMenu(_mainMenu);
            _comboMenu = addComboMenu(_mainMenu);

            if (misc)
            {
                _miscMenu = addMiscMenu(_mainMenu);
            }

            _mainMenu.AddToMainMenu();
        }

        private void addOrbwalker(Menu RootMenu)
        {
            Menu orbwalkerMenu = new Menu("Orbwalker", string.Format("{0}.orbwalker", MenuName));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            RootMenu.AddSubMenu(orbwalkerMenu);
        }

        private void addTargetSelector(Menu RootMenu)
        {
            Menu targetselectorMenu = new Menu("Target Selector", string.Format("{0}.targetselector", MenuName));
            TargetSelector.AddToMenu(targetselectorMenu);
            RootMenu.AddSubMenu(targetselectorMenu);
        }

        private Menu addHarasMenu(Menu RootMenu)
        {
            Menu newMenu = new Menu("Haras", string.Format("{0}.haras", MenuName));
            RootMenu.AddSubMenu(newMenu);

            MenuItem UseQToHaras = new MenuItem(string.Format("{0}.useqtoharas", MenuName), "Use Q").SetValue(true);
            MenuItem UseWToHaras = new MenuItem(string.Format("{0}.usewtoharas", MenuName), "Use W").SetValue(true);
            MenuItem UseEToHaras = new MenuItem(string.Format("{0}.useetoharas", MenuName), "Use E").SetValue(true);
            newMenu.AddItem(UseQToHaras);
            newMenu.AddItem(UseWToHaras);
            newMenu.AddItem(UseEToHaras);

            MenuItem ManaLimitToHaras = new MenuItem(string.Format("{0}.manalimittoharas", MenuName), "Mana limit").SetValue(new Slider(0, 0, 100));
            newMenu.AddItem(ManaLimitToHaras);

            return newMenu;
        }

        private Menu addLaneClearMenu(Menu RootMenu)
        {
            Menu newMenu = new Menu("Lane Clear", string.Format("{0}.laneclear", MenuName));
            RootMenu.AddSubMenu(newMenu);

            MenuItem UseQToLaneclear = new MenuItem(string.Format("{0}.useqtolaneclear", MenuName), "Use Q").SetValue(true);
            MenuItem UseWToLaneclear = new MenuItem(string.Format("{0}.usewtolaneclear", MenuName), "Use W").SetValue(true);
            MenuItem UseEToLaneclear = new MenuItem(string.Format("{0}.useetolaneclear", MenuName), "Use E").SetValue(true);
            newMenu.AddItem(UseQToLaneclear);
            newMenu.AddItem(UseWToLaneclear);
            newMenu.AddItem(UseEToLaneclear);

            MenuItem ManaLimitToLaneClear = new MenuItem(string.Format("{0}.manalimittolaneclear", MenuName), "Mana limit").SetValue(new Slider(50, 0, 100));
            newMenu.AddItem(ManaLimitToLaneClear);

            return newMenu;
        }

        private Menu addComboMenu(Menu RootMenu)
        {
            Menu newMenu = new Menu("Combo", string.Format("{0}.combo", MenuName));
            RootMenu.AddSubMenu(newMenu);

            MenuItem UseQToCombo = new MenuItem(string.Format("{0}.useqtocombo", MenuName), "Use Q").SetValue(true);
            MenuItem UseWToCombo = new MenuItem(string.Format("{0}.usewtocombo", MenuName), "Use W").SetValue(true);
            MenuItem UseEToCombo = new MenuItem(string.Format("{0}.useetocombo", MenuName), "Use E").SetValue(true);
            MenuItem UseRToCombo = new MenuItem(string.Format("{0}.usertocombo", MenuName), "Use R").SetValue(true);
            newMenu.AddItem(UseQToCombo);
            newMenu.AddItem(UseWToCombo);
            newMenu.AddItem(UseEToCombo);
            newMenu.AddItem(UseRToCombo);

            return newMenu;
        }

        private Menu addMiscMenu(Menu RootMenu)
        {
            Menu newMenu = new Menu("Options", string.Format("{0}.misc", MenuName));
            RootMenu.AddSubMenu(newMenu);

            return newMenu;
        }
    }
}
