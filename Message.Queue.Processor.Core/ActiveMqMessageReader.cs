using Apache.NMS;
using Apache.NMS.Util;
using Message.Queue.Processor.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Queue.Processor
{
    public interface IActiveMqMessageReader { void ReadMessages(QueueType type, Action<IMessage> onMessageRead); }

    public class ActiveMqMessageReader : IActiveMqMessageReader
    {
        public void ReadMessages(QueueType type, Action<IMessage> onMessageRead)
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
                    connection.Start();
                    
                    IMessage message;
                    while ((message = consumer.Receive(TimeSpan.FromMilliseconds(100))) != null)
                    {
                        onMessageRead(message);
                        message.Acknowledge();
                    }
                }
            }
        }

        public List<IMessage> ReadBatchMessages(QueueType type,int batchSize)
        {
            var result = new List<IMessage>();

            Debug.WriteLine("About to connect to " + Configuration.ActiveMqUri);

            IConnectionFactory factory = new NMSConnectionFactory(Configuration.ActiveMqUri);

            using (IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession())
            {
                IDestination destination = SessionUtil.GetQueue(session, type.ToString());
                Debug.WriteLine("Using destination: " + destination);

                using (IMessageConsumer consumer = session.CreateConsumer(destination))
                {
                    connection.Start();

                    IMessage message;
                    int ii=0;
                    while ((message = consumer.Receive(TimeSpan.FromMilliseconds(100))) != null && ii<batchSize)
                    {
                        result.Add(message);
                        ii++;
                    }
                }
            }

            return result;
        }
    }
}
