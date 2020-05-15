using System;
using System.Collections.Generic;
using System.Text;

namespace Pizzaria.Infraestrutura.Interface
{
    public interface IMQConnection: IDisposable
    {
        string ActiveQueue { get;  }

        event MessageArrivedEventHandler MessageArrived;

        uint MessageCount();

        void ListenTo(string QueueName);

        void ListenTo(string QueueName, bool Durable, bool Exclusive, bool AutoDelete, IDictionary<string, object> Arguments);

        void SendAck(ulong DeliveryTag);

        void SendNack(ulong DeliveryTag);

        void PostMessage(string QueuerName, string message);

        void PostMessage(string Exchange, string RoutingKey, string Message);

    }
}
