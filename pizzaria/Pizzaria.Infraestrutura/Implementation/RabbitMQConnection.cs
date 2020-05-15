using Newtonsoft.Json;
using Pizzaria.Dominio.EventModel;
using Pizzaria.Dominio.Messages.Common;
using Pizzaria.Infraestrutura.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pizzaria.Infraestrutura.Implementation
{
    internal sealed class RabbitMQConnection: IMQConnection
    {
        private IConnection _RabbitConnection = null;
        private IModel _QueueChannel = null;

        private event MessageArrivedEventHandler MessageArrived;

        private EventingBasicConsumer CallbackEvent = null;

        public RabbitMQConnection(string Hostname, string Username, string Password)
        {
            Configure(Hostname, Username, Password);
        }

        public RabbitMQConnection(string Hostname, int Port, string Username, string Password)
        {
            Configure(Hostname, Port, Username, Password);
        }

        event MessageArrivedEventHandler IMQConnection.MessageArrived
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        private void Configure(string Hostname, string Username, string Password)
        {
            this.Configure(Hostname, 5672, Username, Password);
        }

        private void Configure(string Hostname, int Port, string Username, string Password)
        {
            _RabbitConnection = new ConnectionFactory
            {
                HostName = Hostname.Contains("\\") ? Hostname.Split("\\")[0] : Hostname,
                VirtualHost = Hostname.Contains("\\") ? Hostname.Split("\\")[1] : "/",
                Port = Port,
                UserName = Username,
                Password = Password,
                RequestedChannelMax = 1
            }.CreateConnection();

            _QueueChannel = _RabbitConnection.CreateModel();
        }

        public string ActiveQueue { get; private set; }

        public void ListenTo(string QueueName)
        {
            if (MessageArrived == null) throw new EntryPointNotFoundException("ReceiveCallback não definido");

            ListenTo(QueueName, true, false, false, null);
        }

        public void ListenTo(string QueueName, bool Durable, bool Exclusive, bool AutoDelete, IDictionary<string, object> Arguments)
        {
            this.ActiveQueue = QueueName;

            _QueueChannel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            CallbackEvent = new EventingBasicConsumer(_QueueChannel);
            CallbackEvent.Received += CallbackEvent_Received;

            _QueueChannel.BasicConsume(QueueName, false, CallbackEvent);
        }
        
        private void CallbackEvent_Received(object sender, BasicDeliverEventArgs e)
        {
            string body = Encoding.UTF8.GetString(e.Body);
            
            try
            {
                GenericMessage basicMessage = JsonConvert.DeserializeObject<GenericMessage>(Encoding.UTF8.GetString(e.Body));

                Stopwatch sw = new Stopwatch();

                MessageArrived(sender,
                    this,
                    new MessageArrivedArgs
                    {
                        DeliveryTag = Convert.ToString(e.DeliveryTag),
                        CorrelationId = Convert.ToUInt64(basicMessage.CorrelationId),
                        Command = basicMessage.Command,
                        MetaData = basicMessage.MetaData,
                        Body = body,
                    }
                );
                
            }
            catch(JsonReaderException)
            {

            }
        }

        public uint MessageCount()
        {
            if (_QueueChannel == null || _QueueChannel.IsClosed || string.IsNullOrEmpty(this.ActiveQueue)) throw new NullReferenceException("Not Listen");
            return _QueueChannel.MessageCount(this.ActiveQueue);
        }

        public void SendAck(ulong DeliveryTag)
        {
            _QueueChannel.BasicAck(DeliveryTag, false);
        }

        public void SendNack(ulong DeliveryTag)
        {
            _QueueChannel.BasicNack(DeliveryTag, false, true);
        }

        public void PostMessage(string Exchange, string RoutingKey, string Message)
        {
            _QueueChannel.BasicPublish(
                Exchange,
                RoutingKey,
                null,
                Encoding.UTF8.GetBytes(Message));
        }

        public void PostMessage(string QueueName, string Message)
        {
            _QueueChannel.BasicPublish(
                "",
                QueueName,
                null,
                Encoding.UTF8.GetBytes(Message));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
