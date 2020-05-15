using Pizzaria.Infraestrutura.Factory;
using Pizzaria.Infraestrutura.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pizzaria.Service.Engine.Common
{
    internal abstract class Base
    {
        private IMQConnection messageConnector;


        public Base()
        {
            messageConnector = MessageConnectionFactory.CriarConexaoMQ();
        }
    }
}
