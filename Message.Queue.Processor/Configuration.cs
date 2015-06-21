using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Queue.Processor
{
    public static class Configuration
    {
        public static Uri ActiveMqUri = new Uri("activemq:tcp://localhost:61616");

    }
}
