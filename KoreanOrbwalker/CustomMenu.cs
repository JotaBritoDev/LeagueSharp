namespace KoreanOrbwalker
{
    using LeagueSharp.Common;

    internal class CustomMenu
    {
        #region Fields

        private readonly MenuItem comboMode;

        private readonly MenuItem howDoYouWannaFeel;

        private readonly MenuItem laneclearMode;

        private readonly MenuItem lastHitMode;

        private readonly Menu mainMenu;

        private readonly MenuItem mixedMode;

        private readonly MenuItem showRanges;

        #endregion

        #region Constructors and Destructors

        public CustomMenu()
        {
            mainMenu = new Menu("Korean Orbwalker", "koreanorbwalker", true);

            howDoYouWannaFeel =
                new MenuItem("orbwalkerlevel", "How do you wanna feel?").SetValue(
                    new StringList(new string[] { "Korean", "Platinum", "Bronze", "Totally dumbass" }));
            lastHitMode = new MenuItem("orbwalkerlasthit", "Last Hit").SetValue(new KeyBind('X', KeyBindType.Press));
            mixedMode = new MenuItem("orbwalkermixe", "Last Hit + Haras").SetValue(new KeyBind('C', KeyBindType.Press));
            laneclearMode =
                new MenuItem("orbwalkerlaneclear", "Lane Clear").SetValue(new KeyBind('V', KeyBindType.Press));
            comboMode = new MenuItem("orbwalkercombo", "Combo").SetValue(new KeyBind(' ', KeyBindType.Press));
            showRanges = new MenuItem("orbwalkerrange", "Show Range").SetValue(true);

            mainMenu.AddItem(howDoYouWannaFeel);
            mainMenu.AddItem(lastHitMode);
            mainMenu.AddItem(mixedMode);
            mainMenu.AddItem(laneclearMode);
            mainMenu.AddItem(comboMode);
            mainMenu.AddItem(showRanges);

            mainMenu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool ComboActive
        {
            get
            {
                return comboMode.GetValue<KeyBind>().Active;
            }
        }

        public FeelingLike FeelingLike
        {
            get
            {
                return (FeelingLike)howDoYouWannaFeel.GetValue<StringList>().SelectedIndex;
            }
        }

        public bool LaneClearActive
        {
            get
            {
                return laneclearMode.GetValue<KeyBind>().Active;
            }
        }

        public bool LastHitActive
        {
            get
            {
                return lastHitMode.GetValue<KeyBind>().Active;
            }
        }

        public bool MixedActive
        {
            get
            {
                return mixedMode.GetValue<KeyBind>().Active;
            }
        }

        public bool RangesActive
        {
            get
            {
                return showRanges.GetValue<bool>();
            }
        }

        #endregion
    }
}