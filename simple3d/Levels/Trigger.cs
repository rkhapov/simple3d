using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace simple3d.Levels
{
    public class Trigger
    {
        static Dictionary<Vector2, Trigger> allTriggers = new Dictionary<Vector2, Trigger>(); // 

        public static void AddTrigger(Vector2 position, Action<Scene> action, bool reapetable = false) // можно удалять в дальнеейшем если не повторяемое но хз
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

        public static void CheckAndDo(Vector2 position, Scene scene)
        {
            if (scene.Player.interactMod)
            {
                position = new Vector2((int)position.X, (int)position.Y);
                if (allTriggers.ContainsKey(position))
                {
                    allTriggers[position].DoIt(scene);
                }
            }
        }

        private bool itsDone;
        private readonly Action<Scene> action;
        private Trigger(Action<Scene> action)
        {
            itsDone = false;
            this.action = action;
        }

        public void DoIt(Scene scene)
        {
            if (!itsDone)
            {
                action(scene);
                itsDone = true;
            }
        }
        

    }
}
