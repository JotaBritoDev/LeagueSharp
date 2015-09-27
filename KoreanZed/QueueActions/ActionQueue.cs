namespace KoreanZed.QueueActions
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;

    class ActionQueue
    {
        public void EnqueueAction(ActionQueueList list, Func<bool> preCondition, Action comboAction, Func<bool> conditionToRemove)
        {
            list.Add(new ActionQueueItem()
            {
                Time = Game.Time,
                PreConditionFunc = preCondition,
                ComboAction = comboAction,
                ConditionToRemoveFunc = conditionToRemove
            });
        }

        public bool ExecuteNextAction(ActionQueueList list)
        {
            if (list.Count > 0)
            {
                if (list[0].PreConditionFunc.Invoke())
                {
                    list[0].ComboAction.Invoke();
                }

                if (list[0].ConditionToRemoveFunc.Invoke() || Game.Time > list[0].Time + 0.75F)
                {
                    list.Remove(list[0]);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
