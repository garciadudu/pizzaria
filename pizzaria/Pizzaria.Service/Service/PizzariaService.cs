using Newtonsoft.Json;
using Pizzaria.Dominio.Enum;
using Pizzaria.Infraestrutura.Factory;
using Pizzaria.Infraestrutura.Interface;
using Pizzaria.Service.Factory;
using Pizzaria.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pizzaria.Service.Service
{
    internal sealed class PizzariaService : IPizzariaService
    {
        public ServiceStatus ServiceStatus { get; private set;  } = ServiceStatus.Raising;


        public CancellationTokenSource StartService()
        {
            IList<IMQConnection> messageConnectors = null;

            var tokenSource = new CancellationTokenSource();
            string Queues = Configuration.GetStringProperty("Pizzaria");

            var t = Task.Run(() =>
            {
                try
                {
                    messageConnectors = new List<IMQConnection>();

                    foreach (string queueName in Queues.Replace(",", ";").Split(';'))
                    {
                        IMQConnection messageConnector;
                        messageConnector = MessageConnectionFactory.CriarConexaoMQ();

                        messageConnector.MessageArrived += MessageConnector_MessageArrived;

                        messageConnector.ListenTo(queueName);

                        messageConnectors.Add(messageConnector);
                    }

                    ServiceStatus = ServiceStatus.Running;

                    do
                    {
                        if (tokenSource.Token.IsCancellationRequested) ServiceStatus = ServiceStatus.Cancelling;

                        Thread.Sleep(300);
                    } while (ServiceStatus == ServiceStatus.Running);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    foreach (IMQConnection messageConnector in messageConnectors) messageConnector.Dispose();

                    ServiceStatus = ServiceStatus.Stopped;
                }
            }, tokenSource.Token);

            return tokenSource;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Disposable()
        {
            throw new NotImplementedException();
        }

        private async void MessageConnector_MessageArrived(object sender, IMQConnection messageConnector, Dominio.EventModel.MessageArrivedArgs e)
        {
            try
            {

                using (IPizzariaEngine engine = PizzariaEngineFactory.Create())
                {
                    switch (e.Command)
                    {
                        case "conversa":
                            {
                                var test = JsonConvert.DeserializeObject<Dominio.Messages.Conversa.Conversa>(e.Body);


                                break;
                            }
                    }
                }

            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
