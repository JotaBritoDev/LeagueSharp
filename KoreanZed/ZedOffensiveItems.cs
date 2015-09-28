namespace KoreanZed
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    class ZedOffensiveItems
    {
        private readonly ZedMenu zedMenu;

        public ZedOffensiveItems(ZedMenu menu)
        {
            zedMenu = menu;
        }

        public void UseItems(Obj_AI_Hero target)
        {
            UseYomuusBlade();
            UseBilgewaterCutlass(target);
            UseBOTRK(target);
        }

        private void UseBilgewaterCutlass(Obj_AI_Hero target)
        {
            if (zedMenu.GetParamBool("koreanzed.combomenu.items.bilgewater")
                && ItemData.Bilgewater_Cutlass.GetItem().IsInRange(target)
                && ItemData.Bilgewater_Cutlass.GetItem().IsReady())
            {
                ItemData.Bilgewater_Cutlass.GetItem().Cast(target);
            }
        }

        private void UseBOTRK(Obj_AI_Hero target)
        {
            if (zedMenu.GetParamBool("koreanzed.combomenu.items.botrk")
                && ItemData.Blade_of_the_Ruined_King.GetItem().IsInRange(target)
                && ItemData.Blade_of_the_Ruined_King.GetItem().IsReady())
            {
                ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
            }
        }

        private void UseYomuusBlade()
        {
            if (zedMenu.GetParamBool("koreanzed.combomenu.items.yomuus")
                && ItemData.Youmuus_Ghostblade.GetItem().IsReady())
            {
                ItemData.Youmuus_Ghostblade.GetItem().Cast();
            }
        }
    }
}
