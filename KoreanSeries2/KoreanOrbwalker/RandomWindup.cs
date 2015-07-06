namespace KoreanOrbwalker
{
    using System;

    internal static class RandomWindup
    {
        #region Public Methods and Operators

        public static int Get(FeelingLike feelingLike)
        {
            var rnd = new Random();
            switch (feelingLike)
            {
                case FeelingLike.Korean:
                    return rnd.Next(10);
                case FeelingLike.Platinum:
                    return rnd.Next(100);
                case FeelingLike.Bronze:
                    return rnd.Next(350);
                case FeelingLike.Dumbass:
                    return rnd.Next(700);
                default:
                    return 0;
            }
        }

        #endregion
    }
}