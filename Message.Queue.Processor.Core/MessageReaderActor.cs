using Akka.Actor;
using Apache.NMS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Queue.Processor.Core
{
    internal class MessageReaderActor : ReceiveActor
    {
        private IActiveMqMessageReader messageReader = new ActiveMqMessageReader();

        public MessageReaderActor()
        {
            //Receive<KeyValuePair<QueueType,Action<IMessage>>>((def) =>
            //{
            //    messageReader.ReadMessages(def.Key, def.Value);
            //    Sender.Tell(new ReadingFinished());
            //});

            Receive<IMessage>((r) =>
            {
                Debug.WriteLine(r);
                Sender.Tell(new Status.Success(r));
            });
        }

        class ReadingFinished { }

    }
}
