﻿using System.Collections.Generic;
using System.Linq;

namespace simple3d.Levels
{
    public class GameEventsLogger
    {
        private class Message
        {
            public string Text { get; set; }
            public float LifeTime { get; set; }
        }

        private const float MessageLifeTime = 3000f;
        private const int MaxSize = 10;

        private readonly Queue<Message> messages = new Queue<Message>();

        public void AddCollectOfItem(string itemName)
        {
            var message = new Message {Text = $"Собрано {itemName}", LifeTime = MessageLifeTime};

            messages.Enqueue(message);

            if (messages.Count > MaxSize)
                messages.Dequeue();
        }

        public void Update(float elapsedMilliseconds)
        {
            foreach (var message in messages)
            {
                message.LifeTime -= elapsedMilliseconds;
            }

            while (messages.Count > 0 && messages.Peek().LifeTime < 0)
            {
                messages.Dequeue();
            }
        }

        public IEnumerable<string> GetMessages()
        {
            return messages.Select(msg => msg.Text);
        }
    }
}