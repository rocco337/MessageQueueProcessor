using Apache.NMS;
using Message.Queue.Processor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Queue.Processor.Console
{
    class Program
    {
        private static readonly MessageGenerator _messageGenerator = new MessageGenerator();

        static void Main(string[] args)
        {
            _messageGenerator.Generate(100000, QueueType.ItemQueue, new Item() { Id = DateTime.Now.Ticks, Content = "Content" });
            System.Console.WriteLine("Queue filled! Press any key to continue.");
            System.Console.ReadLine();


            var queueDef= new Dictionary<QueueType, Action<IMessage>>();
            queueDef.Add(QueueType.ItemQueue, (t) => { });

            MessageQueueProcessorFactory.Create(500, queueDef);
            
            System.Console.WriteLine("Queue processing finished in: " + sc.ElapsedMilliseconds + " ms.");
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
