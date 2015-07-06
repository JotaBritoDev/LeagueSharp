namespace KoreanOrbwalker
{
    using LeagueSharp;

    internal class ResetableAA
    {
        #region Fields

        private readonly Orbwalker orbwalker;

        #endregion

        #region Constructors and Destructors

        public ResetableAA(Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        #endregion

        #region Methods

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            switch (ObjectManager.Player.SkinName.ToLowerInvariant())
            {
                case "lucian":
                    orbwalker.ResetAA();
                    orbwalker.ExtraDamage.Add(ObjectManager.Player.BaseAttackDamage * 0.4f, Game.Time + 4f);
                    break;
                case "jayce":
                    if (args.SData.Name == "jaycehypercharge")
                    {
                        orbwalker.ResetAA();
                    }
                    break;
            }

            //if (ObjectManager.Player.SkinName.ToLowerInvariant() == "lucian")
            //{
            //    orbwalker.ResetAA();
            //    orbwalker.ExtraDamage.Add(ObjectManager.Player.BaseAttackDamage * 0.4f, Game.Time + 4f);
            //}
        }

        #endregion
    }
}