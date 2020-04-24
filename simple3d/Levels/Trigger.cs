using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace simple3d.Levels
{
    public class Trigger
    {
        static Dictionary<Vector2, Trigger> allTriggers = new Dictionary<Vector2, Trigger>();

        public static void AddTrigger(Vector2 position, Action action, bool reapetable = false) // можно удалять в дальнеейшем если не повторяемое но хз
        {
            if (allTriggers.ContainsKey(position))
            {
                allTriggers[position] = new Trigger(action);
            }
            else
            {
                allTriggers.Add(position, new Trigger(action));
            }
        }

        public static void CheckAndDo(Vector2 position)
        {
            position = new Vector2((int)position.X, (int)position.Y);// извините но это минус фпс
            if (allTriggers.ContainsKey(position))
            {
                allTriggers[position].DoIt();
            }
        }

        private bool itsDone;
        private Action action;
        private Trigger(Action action)
        {
            itsDone = false;
            this.action = action;
        }

        public void DoIt()
        {
            if (!itsDone)
            {
                action();
                itsDone = true;
            }
        }
        

    }
}
