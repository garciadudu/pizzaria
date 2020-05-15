using Confluent.Kafka;
using Newtonsoft.Json;
using Pizzaria.Dominio.EventModel;
using Pizzaria.Dominio.Messages.Common;
using Pizzaria.Infraestrutura.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Infraestrutura.Implementation
{
    public class KafkaMQConnection : IMQConnection
    {
        private const int MAX_RETRY_ATTEMPS = 6;

        private ConsumerConfig consumeConfig;

        private ProducerConfig produceConfig;

        public KafkaMQConnection(string Hostname, string Username, string Password)
        {
            Configure(Hostname, Username, Password);
        }

        public KafkaMQConnection(string Hostname, int Port, string Username, string Password)
        {
            Configure(Hostname, Port, Username, Password);
        }

        private void Configure(string Hostname, string Username, string Password)
        {
            this.Configure(Hostname, 5672, Username, Password);
        }


        private void Configure(string Hostname, int Port, string Username, string Password)
        {
            consumeConfig = new ConsumerConfig
            {
                BootstrapServers = (Hostname.Contains("\\") ? Hostname.Split("\\")[0] : Hostname) + ":"+Port,
                GroupId = "",
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true,
            };


            produceConfig = new ProducerConfig
            {
                BootstrapServers = (Hostname.Contains("\\") ? Hostname.Split("\\")[0] : Hostname) + ":" + Port,
            };
        }

        public string ActiveQueue => throw new NotImplementedException();

        public event MessageArrivedEventHandler MessageArrived;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void ListenTo(string QueueName)
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(consumeConfig)
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Assigned partitions :[{string.Join(", ", partitions)}]");
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
                })
                .Build())
            {
                consumer.Subscribe(QueueName);
                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume();


                            if (consumeResult.IsPartitionEOF)
                            {
                                Console.WriteLine($"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                                continue;
                            }

                            Console.WriteLine(consumeResult.Key);

                            GenericMessage basicMessage = JsonConvert.DeserializeObject<GenericMessage>(consumeResult.Value);

                            Stopwatch sw = new Stopwatch();

                            MessageArrived(this,
                                this,
                                new MessageArrivedArgs
                                {
                                    DeliveryTag = consumeResult.Key.ToString(),
                                    CorrelationId = Convert.ToUInt64(basicMessage.CorrelationId),
                                    Command = basicMessage.Command,
                                    MetaData = basicMessage.MetaData,
                                    Body = consumeResult.Value,
                                }
                            );


                            consumer.Commit(consumeResult);
                        }
                        catch (KafkaException e)
                        {
                            Console.WriteLine($"Commit error: {e.Error.Reason}");
                        }

                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");
                }

            }
        }

        public void ListenTo(string QueueName, bool Durable, bool Exclusive, bool AutoDelete, IDictionary<string, object> Arguments)
        {
            this.ListenTo(QueueName);
        }

        public uint MessageCount()
        {
            throw new NotImplementedException();
        }

        public async Task PostMessage(string QueuerName, string message)
        {
            using (var producer = new ProducerBuilder<string, string>(produceConfig).Build())
            {
                var deliveryReport = await producer.ProduceAsync(QueuerName,
                    new Message<string, string>
                    {
                        Key = QueuerName,
                        Value = message
                    }).ConfigureAwait(true);
            }
        }

        public void PostMessage(string Exchange, string RoutingKey, string Message)
        {
            throw new NotImplementedException();
        }

        public void SendAck(ulong DeliveryTag)
        {
            throw new NotImplementedException();
        }

        public void SendNack(ulong DeliveryTag)
        {
            throw new NotImplementedException();
        }

        void IMQConnection.PostMessage(string QueuerName, string message)
        {
            throw new NotImplementedException();
        }
    }
}
