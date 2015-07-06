namespace KoreanOrbwalker
{
    using System;
    using System.Collections.Generic;

    internal class Queue
    {
        #region Fields

        private readonly List<QueueItem> comboQueue;

        #endregion

        #region Constructors and Destructors

        public Queue()
        {
            comboQueue = new List<QueueItem>();
        }

        #endregion

        #region Public Methods and Operators

        public void Clear()
        {
            comboQueue.Clear();
        }

        public void Enqueue(Func<bool> preCondition, Action comboAction, Func<bool> conditionToRemove)
        {
            comboQueue.Add(
                new QueueItem()
                    {
                        PreConditionFunc = preCondition, ComboAction = comboAction,
                        ConditionToRemoveFunc = conditionToRemove
                    });
        }

        public bool ExecuteNext()
        {
            if (comboQueue.Count > 0)
            {
                try
                {
                    if (comboQueue[0].PreConditionFunc.Invoke())
                    {
                        comboQueue[0].ComboAction.Invoke();
                    }

                    if (comboQueue[0].ConditionToRemoveFunc.Invoke())
                    {
                        comboQueue.Remove(comboQueue[0]);
                    }
                }
                catch
                {
                    comboQueue.Remove(comboQueue[0]);
                    throw;
                }

                return true;
            }

            return false;
        }

        #endregion
    }
}