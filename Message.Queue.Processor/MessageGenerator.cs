﻿using Apache.NMS;
using Apache.NMS.Util;
using Message.Queue.Processor.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Queue.Processor.Console
{
    public class MessageGenerator
    {
        private Uri _activeMqUri = Configuration.ActiveMqUri;

        public void Generate(int numOfMessages, QueueType type, object item)
        {
            Debug.WriteLine("About to connect to " + _activeMqUri);

            IConnectionFactory factory = new NMSConnectionFactory(_activeMqUri);

            using (IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession())
            {
                IDestination destination = SessionUtil.GetQueue(session, type.ToString());
                Debug.WriteLine("Using destination: " + destination);

                using (IMessageProducer producer = session.CreateProducer(destination))
                {
                    // Start the connection so that messages will be processed.
                    connection.Start();

                    var rnd = new Random();
                    for (var ii = 0; ii < numOfMessages; ii++)
                    {
                        var request = session.CreateObjectMessage(item);
                        request.NMSCorrelationID = Guid.NewGuid().ToString();
                        producer.Send(request);
                    }
                }
            }
        }
    }
}