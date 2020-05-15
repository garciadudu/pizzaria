using Pizzaria.Dominio.Enum;
using Pizzaria.Infraestrutura.Implementation;
using Pizzaria.Infraestrutura.Interface;
using Pizzaria.Util;
using System;

namespace Pizzaria.Infraestrutura.Factory
{
    public class MessageConnectionFactory
    {
        private static IMQConnection CreateRabbitMQ(string Hostname, int Port, string Username, string Password)
        {
            return new RabbitMQConnection(Hostname, Port, Username, Password);
        }

        public static IMQConnection CreateKafka(string Hostname, string Username, string Password)
        {
            return new KafkaMQConnection(Hostname, Username, Password);
        }

        public static IMQConnection CriarConexaoMQ(MQConnectionTypeEnum ConnectionTypeEnum, string HostName, int Port, string UserName, string Password)
        {
            switch (ConnectionTypeEnum)
            {
                case MQConnectionTypeEnum.RabbitMQ:
                    {
                        return CreateRabbitMQ(HostName, Port, UserName, Password);
                    }
                    break;
                case MQConnectionTypeEnum.Kafka: 
                    {
                        return CreateKafka(HostName, UserName, Password);
                    }
                    break;
                default:
                    return CreateRabbitMQ(HostName, Port, UserName, Password);
                    break;
            }
        }

        public static IMQConnection CriarConexaoMQ()
        {
            string connectionTypeName = Configuration.GetStringProperty("Message:Connection").ToUpper();

            MQConnectionTypeEnum connectionType = Enum.Parse<MQConnectionTypeEnum>(connectionTypeName, true);

            Dominio.ConfigurationModel.MQConnection mqConnection = new Dominio.ConfigurationModel.MQConnection
            {
                ConnectionType = connectionType,
                Hostname = Configuration.GetStringProperty("ConnectionStrings:MQConnection:HostName"),
                Port = Convert.ToInt32(Configuration.GetStringProperty("ConnectionStrings:MQConnection:Port")),
                Username = Configuration.GetStringProperty("ConnectionStrings:MQConnection:Username"),
                Password = Configuration.GetStringProperty("ConnectionStrings:MQConnection:Password")
            };

            return CriarConexaoMQ(mqConnection.ConnectionType, mqConnection.Hostname, mqConnection.Port, mqConnection.Username, mqConnection.Password);
        }
    }
}
