using Message.Queue.Processor.Core;
using System;
using System.Collections.Generic;

namespace Message.Queue.Processor.Console
{
    internal class Program
    {
        private static readonly MessageGenerator _messageGenerator = new MessageGenerator();

        private static void Main(string[] args)
        {
            _messageGenerator.Generate(1000, QueueType.ItemQueue, new Item() { Id = DateTime.Now.Ticks, Content = "Content" });

            System.Console.WriteLine("Queue filled! Press any key to continue.");
            System.Console.ReadLine();

            var queueDef = new List<QueueType>();
            queueDef.Add(QueueType.ItemQueue);

            MessageQueueProcessorFactory.Create(queueDef);

            System.Console.ReadLine();
        }

        [Serializable]
        public class Item
        {
            public long Id { get; set; }

            public string Content { get; set; }
        }
    }
}