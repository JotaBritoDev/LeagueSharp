namespace IgniteSharp
{
    using LeagueSharp;
    using LeagueSharp.Common;

    public class CustomMenu
    {
        private readonly Menu menu;

        private readonly Ignite obj;

        public CustomMenu(Ignite ignite)
        {
            obj = ignite;
            menu = new Menu("Ignite#", "ignite", true);

            Load(ignite);
        }

        public Menu Get()
        {
            return menu;
        }

        private void Load(Ignite ignite)
        {
            menu.AddItem(
                new MenuItem("ignite#key", "Key").SetValue(
                    new KeyBind('F', KeyBindType.Toggle)));

            menu.AddItem(
                new MenuItem("ignite#target", "Cast on").SetValue(
                    new StringList(new[] { "Lowest target", "Use target selector" })));

            MenuItem autoKill = new MenuItem("ignite#autokill", "Auto kill").SetValue(false);
            menu.AddItem(autoKill);

            autoKill.ValueChanged += delegate(object sender, OnValueChangeEventArgs e)
                {
                    if (e.GetNewValue<bool>())
                    {
                        Game.OnUpdate += obj.AutoKill.CastIgnite;
                    }
                    else
                    {
                        Game.OnUpdate -= obj.AutoKill.CastIgnite;
                    }
                };

            MenuItem drawRange = new MenuItem("ignite#range", "Draw range").SetValue(false);
            menu.AddItem(drawRange);

            drawRange.ValueChanged += delegate(object sender, OnValueChangeEventArgs e)
            {
                if (e.GetNewValue<bool>())
                {
                    Drawing.OnDraw += obj.drawings.IgniteRange;
                }
                else
                {
                    Drawing.OnDraw -= obj.drawings.IgniteRange;
                }
            };

            menu.AddToMainMenu();
        }
    }
}