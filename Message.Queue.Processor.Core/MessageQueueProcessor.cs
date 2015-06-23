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
        public static void Create(int intervalMiliseconds, Dictionary<QueueType, Action<IMessage>> queueDef)
        {
            new MessageQueueProcessor(intervalMiliseconds, queueDef);
        }

        public static void Create(int numOfInstances, int intervalMiliseconds, Dictionary<QueueType, Action<IMessage>> queueDef)
        {
            for (var ii = 0; ii < numOfInstances; ii++)
            {
                new MessageQueueProcessor(intervalMiliseconds, queueDef);
            }
        }
    }

    internal class MessageQueueProcessor
    {
        private readonly ActorSystem system = ActorSystem.Create("MessageQueueProcessor");

        public MessageQueueProcessor(int intervalMiliseconds, Dictionary<QueueType, Action<IMessage>> queueDef)
        {
            var reader = system.ActorOf(Props.Create<MessageReaderActor>().WithRouter(new ConsistentHashingPool(10)));

            ProcessMessages(reader);
        }

        private static async void ProcessMessages(IActorRef myBusinessActor)
        {
            while (true)
            {
                var batch = (new ActiveMqMessageReader()).ReadBatchMessages(QueueType.ItemQueue, 10);

                var tasks = (
                    from res in batch
                    let importantMessage = res
                    let ask = myBusinessActor.Ask<Status.Success>(new ConsistentHashableEnvelope(importantMessage,
                            importantMessage.GetHashCode()), TimeSpan.FromSeconds(1))
                    let done = ask.ContinueWith(t =>
                    {
                        if (t.IsCanceled)
                        {
                            Console.WriteLine("Failed to ack {0}", importantMessage);
                        }
                        else
                        {
                            res.Acknowledge();
                            Console.WriteLine("Completed {0}", importantMessage);
                        }
                    }, TaskContinuationOptions.None)
                    select done).ToList();

                await Task.WhenAll(tasks);
                Debug.WriteLine("All messages handled (acked or failed)");
            }
        }
    }
        public enum QueueType
        {
            ItemQueue
        }
    
}
