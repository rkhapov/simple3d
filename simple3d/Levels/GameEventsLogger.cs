using System.Collections.Generic;
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

        private object myLock = new object();
        private readonly Queue<Message> messages = new Queue<Message>();

        public void AddCollectOfItem(string itemName)
        {
            EnqueueMessage($"Собрано {itemName}");
        }

        public void AddDeathAnnouncement()
        {
            EnqueueMessage("Вы погибли");
        }
        
        public void MonsterAttacks(string monsterName)
        {
            EnqueueMessage($"{monsterName} атакует");
        }

        public void MonsterDeath(string monsterName)
        {
            EnqueueMessage($"{monsterName} погибает");
        }

        public void NoMoreArrows()
        {
            EnqueueMessage("Стрелы закончились...");
        }

        private void EnqueueMessage(string text)
        {
            lock (myLock)
            {
                var message = new Message {Text = text, LifeTime = MessageLifeTime};

                messages.Enqueue(message);

                if (messages.Count > MaxSize)
                    messages.Dequeue();
            }
        }

        public void Update(float elapsedMilliseconds)
        {
            lock (myLock)
            {
                foreach (var message in messages.Where(m => m != null).ToArray())
                {
                    message.LifeTime -= elapsedMilliseconds;
                }

                while (messages.Count > 0 && messages.Peek().LifeTime < 0)
                {
                    messages.Dequeue();
                }
            }
        }

        public IEnumerable<string> GetMessages()
        {
            lock (myLock)
            {
                return messages.Select(msg => msg.Text).ToArray();
            }
        }

        public void MonsterHit(string name, int damage)
        {
            EnqueueMessage($"{name} получает урон - {damage}");
        }

        public void SuccessfullyDefence(string name)
        {
            EnqueueMessage($"{name} отражает удар");
        }

        public void ArrowMissed(string name)
        {
            EnqueueMessage($"Стрела прошла мимо {name}!");
        }
    }
}