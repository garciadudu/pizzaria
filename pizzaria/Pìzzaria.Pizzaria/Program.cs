using Pizzaria.Dominio.Enum;
using Pizzaria.Infraestrutura.Interface;
using Pizzaria.Service.Factory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pìzzaria.Pizzaria
{
    class Program
    {
        static CancellationTokenSource cancellationTokenSource;

        static void Main(string[] args)
        {
            Console.WriteLine("Pizzaria");

            var t = Task.Run(() =>
            {
                using (IPizzariaService service = PirzzariaServiceFactory.Create())
                {
                    cancellationTokenSource = service.StartService();


                    do
                    {
                        Thread.Sleep(300);
                    } while (service.ServiceStatus != ServiceStatus.Stopped);
                }
            });

            Console.WriteLine("Pressione enter para encerrar...");

            Console.ReadKey();

            cancellationTokenSource.Cancel();
        }
    }
}
