using Pizzaria.Dominio.EventModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pizzaria.Infraestrutura.Interface
{
    public delegate void MessageArrivedEventHandler(object sender, IMQConnection messageContector, MessageArrivedArgs e);
}