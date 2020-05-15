using System;
using System.Collections.Generic;
using System.Text;

namespace Pizzaria.Dominio.Messages.Common
{
    public class BaseMessage<T>
    {
        public T MetaData { get; set; }
        public long CorrelationId { get; set; }
        public string Command { get; set; }
    }
}
