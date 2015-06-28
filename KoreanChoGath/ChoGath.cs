namespace KoreanChoGath
{
    using System;

    using KoreanCommon;

    internal class ChoGath : CommonChampion
    {
        public ChoGath()
            : base("Korean Cho'Gath")
        {
            KoreanChoGath.Spells.Load(this);
            ChoGathOrbwalker = new Core(this);
            ForceUltimate.ForceUltimate = ChoGathOrbwalker.Ultimate;

            CustomMenu.Load(this);
            smartE = new SmartE(this);
            cancelAA = new CancelAA(this);
            stackPassive = new StackPassive(this);
            drawing = new Drawing(this);
            flashUlt = new FlashUlt(this);
        }

        public Core ChoGathOrbwalker { get; set; }

        public SmartE smartE { get; set; }

        public CancelAA cancelAA { get; set; }

        public StackPassive stackPassive { get; set; }

        public Drawing drawing { get; set; }

        public FlashUlt flashUlt { get; set; }
    }
}