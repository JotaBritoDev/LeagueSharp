namespace KoreanOrbwalker
{
    using System;

    internal struct QueueItem
    {
        #region Fields

        public Action ComboAction;

        public Func<bool> ConditionToRemoveFunc;

        public Func<bool> PreConditionFunc;

        #endregion
    }
}