namespace KoreanAnnie
{
    using KoreanAnnie.Common;

    using LeagueSharp.Common;

    internal static class AnnieCustomMenu
    {
        #region Public Methods and Operators

        public static void Load(CommonMenu mainMenu)
        {
            RemoveItems(mainMenu);
            LoadLaneClear(mainMenu);
            LoadCombo(mainMenu);
            LoadMisc(mainMenu);
        }

        #endregion

        #region Methods

        private static void LoadCombo(CommonMenu mainMenu)
        {

        }

        private static void LoadLaneClear(CommonMenu mainMenu)
        {
            mainMenu.LaneClearMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "saveqtofarm"), "Save Q to farm").SetValue(true));
            mainMenu.LaneClearMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "minminionstow"), "W must hit at least").SetValue(
                    new Slider(3, 1, 6)));
        }

        private static void LoadMisc(CommonMenu mainMenu)
        {
            Menu passiveStunMenu =
                mainMenu.MiscMenu.AddSubMenu(
                    new Menu("Pyromania control (passive)", KoreanUtils.ParamName(mainMenu, "passivestunmenu")));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "useetostack"), "Use E to stack").SetValue(true));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "manalimitforstacking"), "Mana limit for stacking")
                    .SetValue(new Slider(30, 0, 100)));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "savestunforcombo"), "Save stun for combo/haras").SetValue(
                    false));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "showeeasybutton"), "Show easy button").SetValue(true));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "easybuttonpositionx"), "Button position X (ReadOnly)")
                    .SetValue(0));
            passiveStunMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "easybuttonpositiony"), "Button position Y (ReadOnly)")
                    .SetValue(0));

            Menu flashTibbers =
                mainMenu.MiscMenu.AddSubMenu(
                    new Menu("Flash + Tibbers", KoreanUtils.ParamName(mainMenu, "flashtibbersmenu")));
            flashTibbers.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "flashtibbers"), "key").SetValue(
                    new KeyBind('T', KeyBindType.Press)));
            flashTibbers.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "minenemiestoflashr"), "Only use if will hit at least")
                    .SetValue(new Slider(2, 1, 5)));
            flashTibbers.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "orbwalktoflashtibbers"), "Orbwalk").SetValue(false));

            mainMenu.MiscMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "supportmode"), "Support mode").SetValue(false));

            mainMenu.MiscMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "useqtofarm"), "Use Q to farm").SetValue(true));
            mainMenu.MiscMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "useeagainstaa"), "Use E against AA").SetValue(true));
            mainMenu.MiscMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "antigapcloser"), "Anti gap closer").SetValue(true));
            mainMenu.MiscMenu.AddItem(
                new MenuItem(
                    KoreanUtils.ParamName(mainMenu, "interruptspells"),
                    "Interrupt dangerous spells if possible").SetValue(true));

            Menu DontUseComboMenu = mainMenu.MiscMenu.AddSubMenu(new Menu("Don't haras/combo on", "dontusecomboon"));

            foreach (var enemy in HeroManager.Enemies)
            {
                DontUseComboMenu.AddItem(
                    new MenuItem(
                        KoreanUtils.ParamName(mainMenu, "combo" + enemy.ChampionName.ToLowerInvariant()),
                        enemy.ChampionName).SetValue(true));
            }

            DontUseComboMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "dontuselabel1"), "========================"));
            DontUseComboMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "dontuselabel2"), "IMPORTANT: Targets setted \"OFF\" will"));
            DontUseComboMenu.AddItem(
                new MenuItem(KoreanUtils.ParamName(mainMenu, "dontuselabel3"), "    be attacked if they are alone or low"));
        }

        private static void RemoveItems(CommonMenu mainMenu)
        {
            mainMenu.HarasMenu.Items.Remove(mainMenu.HarasMenu.Item(KoreanUtils.ParamName(mainMenu, "useetoharas")));
            mainMenu.LaneClearMenu.Items.Remove(
                mainMenu.LaneClearMenu.Item(KoreanUtils.ParamName(mainMenu, "useetolaneclear")));
            mainMenu.ComboMenu.Items.Remove(mainMenu.ComboMenu.Item(KoreanUtils.ParamName(mainMenu, "useetocombo")));
        }

        #endregion
    }
}