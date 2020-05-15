using Pizzaria.Infraestrutura.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pizzaria.Service.Factory
{
    public class PizzariaEngineFactory
    {
        public static IPizzariaEngine Create()
        {
            return new Engine.PizzariaEngine();
        }
    }
}
