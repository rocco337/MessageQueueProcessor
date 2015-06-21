using Akka.Actor;
using Apache.NMS;
using Apache.NMS.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Queue.Processor
{
    public class MessageQueueProcessor
    {
        public static ActorSystem system = ActorSystem.Create("MessageQueueProcessor");
        
        public MessageQueueProcessor()
        {
            
        }

        class MessageReaderActor : ReceiveActor
        {
            public MessageReaderActor()
            {
                 
            }

            private void read(QueueType type)
            {
                Debug.WriteLine("About to connect to " + Configuration.ActiveMqUri);

                // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
                IConnectionFactory factory = new NMSConnectionFactory(Configuration.ActiveMqUri);

                using (IConnection connection = factory.CreateConnection())
                using (ISession session = connection.CreateSession())
                {
                    IDestination destination = SessionUtil.GetQueue(session, type.ToString());
                    Debug.WriteLine("Using destination: " + destination);

                    using (IMessageConsumer consumer = session.CreateConsumer(destination))
                    {
                        // Start the connection so that messages will be processed.
                        connection.Start();

                        IMessage message;
                        while ((message = consumer.Receive()) != null)
                        {
                            var objectMessage = message as IObjectMessage;
                            if (objectMessage != null)
                            {
                               //
                            }
                            else
                            {
                               //
                            }
                        }
                      
                    }
                }
            }
        }
    }
}
