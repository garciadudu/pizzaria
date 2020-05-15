using Pizzaria.Dominio.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Pizzaria.Infraestrutura.Interface
{
    public interface IPizzariaService: IDisposable
    {
        ServiceStatus ServiceStatus { get;  }

        CancellationTokenSource StartService();

        void Disposable();
    }
}
