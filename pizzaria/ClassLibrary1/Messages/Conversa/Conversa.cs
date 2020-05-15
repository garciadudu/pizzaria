using Pizzaria.Dominio.Messages.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pizzaria.Dominio.Messages.Conversa
{
    public class Conversa:  BaseMessage<ConversaMetaData>
    {
    }

    public class ConversaMetaData
    {
        public string Phone;
        public string Conversa;
    }
}
