namespace Pizzaria.Dominio.EventModel
{
    public class MessageArrivedArgs
    {
        public string DeliveryTag { get; set; }
        public ulong CorrelationId { get; set; }
        public string Command { get; set; }
        public object MetaData { get; set; }
        public string Body { get; set; }
    }
}
