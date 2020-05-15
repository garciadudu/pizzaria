using Pizzaria.Infraestrutura.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pizzaria.Service.Factory
{
    public static class PirzzariaServiceFactory
    {
        public static IPizzariaService Create()
        {
            return new Service.PizzariaService();
        }
    }
}
