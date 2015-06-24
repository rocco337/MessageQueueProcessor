using Akka.Actor;
using Akka.Event;
using Apache.NMS;
using System;
using System.Collections.Generic;

namespace Message.Queue.Processor.Core
{
    internal class MessageReaderActor : ReceiveActor
    {
        public MessageReaderActor()
        {
            System.Diagnostics.Debug.WriteLine("{0} actor initiated - {1}", DateTime.Now.ToShortTimeString(), GetHashCode());

            Receive<IMessage>((r) =>
            {
                r.NMSCorrelationID = Guid.NewGuid().ToString();
                Console.WriteLine(r.NMSCorrelationID);
                Sender.Tell(new Status.Success(r));
            });

            Receive<List<IMessage>>((r) =>
            {
                foreach (var message in r)
                {
                    message.NMSCorrelationID = Guid.NewGuid().ToString();
                    Console.WriteLine("Instance: {0} - Id: {1}", GetHashCode(), message.NMSCorrelationID);
                }

                Sender.Tell(new Status.Success(r));
            });
        }
    }
}