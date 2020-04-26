using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace simple3d.Levels
{
    public class Trigger
    {
        static List<Trigger> allTriggers = new List<Trigger>(); // 

        public static void AddTrigger(Vector2 position, Action<Scene> action, bool interact = true,
            bool reapetable = false) // можно удалять в дальнеейшем если не повторяемое но хз
        {
            allTriggers.Add(new Trigger((int) position.Y, (int) position.X, action, interact));
        }
        
        public static void AddTrigger(int y, int x, Action<Scene> action, bool interact = true,
            bool reapetable = false) // можно удалять в дальнеейшем если не повторяемое но хз
        {
            allTriggers.Add(new Trigger(y, x, action, interact));
        }

        public static void CheckAndDo(Vector2 position, Scene scene)
        {
            var xx = (int) position.X;
            var yy = (int) position.Y;

            foreach (var trigger in allTriggers.Where(trigger => trigger.x == xx && trigger.y == yy))
            {
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

        private bool itsDone;
        private bool interact;
        private readonly Action<Scene> action;
        private readonly int x;
        private readonly int y;

        private Trigger(int y, int x, Action<Scene> action, bool interact = true)
        {
            this.interact = interact;
            itsDone = false;
            this.action = action;
            this.x = x;
            this.y = y;
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