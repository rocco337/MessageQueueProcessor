using Akka.Actor;
using Akka.Routing;
using Apache.NMS;
using Apache.NMS.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Queue.Processor.Core
{
    public static class MessageQueueProcessorFactory
    {
        public static void Create(List<QueueType> queueTypes)
        {
            foreach (var queueType in queueTypes)
            {
                new MessageQueueProcessor(queueType);
            }
        }

        public static void Create(QueueType queueType)
        {
            new MessageQueueProcessor(queueType);
        }
    }

    internal class MessageQueueProcessor
    {
        private const int _batchSize = 100;
        private const int _numOfInstances = 10;

        private readonly ActorSystem system = ActorSystem.Create("MessageQueueProcessor");

        public MessageQueueProcessor(QueueType type)
        {
            var reader = system.ActorOf(Props.Create<MessageReaderActor>().WithRouter(new RoundRobinPool(_numOfInstances)), "reader-pool");
            ProcessMessages(reader, type);
        }

        private static void ProcessMessages(IActorRef messageReaderActor, QueueType type)
        {
            var sc = Stopwatch.StartNew();
            var tasks = new List<Task>();
            while (true)
            {
                var batch = (new ActiveMqMessageReader()).ReadBatchMessages(type, _batchSize);

                if (!batch.Any())
                {
                    sc.Stop();
                    break;
                }

                var ask = messageReaderActor.Ask<Status.Success>(new ConsistentHashableEnvelope(batch, batch.GetHashCode()), TimeSpan.FromSeconds(1));

                tasks.Add(ask);
            }

            Task.WhenAll(tasks).ContinueWith((t) =>
            {
                Console.WriteLine("Queue processing finished in: " + sc.ElapsedMilliseconds + " ms.");
            });
        }
    }

    public enum QueueType
    {
        ItemQueue
    }
}