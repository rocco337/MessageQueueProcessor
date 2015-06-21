using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Queue.Processor
{
    class Program
    {
        private static readonly MessageGenerator _messageGenerator = new MessageGenerator();

        static void Main(string[] args)
        {
            _messageGenerator.Generate(10, QueueType.ItemQueue, new Item() { Id = DateTime.Now.Ticks, Content = "Content" });
            Console.ReadLine();
        }
    }
}
