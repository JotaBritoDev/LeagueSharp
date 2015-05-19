using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace IgniteSharp
{
    using System.Runtime.CompilerServices;

    public class CustomMenu
    {
        private readonly Menu menu;

        public Menu Get()
        {
            return menu;
        }

        private readonly Ignite obj;

        public CustomMenu(Ignite ignite)
        {
            obj = ignite;
            menu = new Menu("Ignite#", "ignite", true);            

            Load(ignite);
        }

        private void Load(Ignite ignite)
        {
            menu.AddItem(
                new MenuItem("ignite#target", "Cast on").SetValue(
                    new StringList(new[] { "Lowest target", "Use target selector" })));

            MenuItem autoKill = new MenuItem("ignite#autokill", "Auto cast on killable targets").SetValue(false);

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

            menu.AddToMainMenu();
        }

    }
}
