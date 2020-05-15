using Pizzaria.Dominio.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pizzaria.Dominio.ConfigurationModel
{
    public sealed class MQConnection
    {
        public MQConnectionTypeEnum ConnectionType { get; set; }

        public string Hostname { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
