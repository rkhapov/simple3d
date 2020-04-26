using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace simple3d.Levels
{
    public class Trigger
    {
        static Dictionary<Vector2, Trigger> allTriggers = new Dictionary<Vector2, Trigger>(); // 

        public static void AddTrigger(Vector2 position, Action<Scene> action, bool interact = true, bool reapetable = false) // можно удалять в дальнеейшем если не повторяемое но хз
        {
            if (allTriggers.ContainsKey(position))
            {
                allTriggers[position] = new Trigger(action, interact);
            }
            else
            {
                allTriggers.Add(position, new Trigger(action, interact));
            }
        }

        public static void CheckAndDo(Vector2 position, Scene scene)
        {
            position = new Vector2((int)position.X, (int)position.Y);
            if (allTriggers.ContainsKey(position))
            {
                if (allTriggers.ContainsKey(position))
                {
                    var trigger = allTriggers[position];
                    if (trigger.interact)
                    {
                        if (scene.Player.interactMod)
                        {
                            trigger.DoIt(scene);
                        }
                    }
                    else
                    {
                        trigger.DoIt(scene);
                    }
                }
            }

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
        private bool interact;
        private readonly Action<Scene> action;
        private Trigger(Action<Scene> action, bool interact = true)
        {
            this.interact = interact;
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
