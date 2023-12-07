using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.DataBase;
using Entidades.Interfaces;

namespace Entidades.Modelos
{
    public delegate void DelegadoNuevoPedido<T>(T menu);

    public class Mozo<T> where T : IComestible, new()
    {
        private CancellationTokenSource cancellation;
        private T menu;
        private Task tarea;
        public event DelegadoNuevoPedido<T> OnPedido;

        public bool EmpezarATrabajar 
        { 
            get
            {
                return tarea is not null && tarea.Status == TaskStatus.Running || tarea.Status == TaskStatus.WaitingToRun || tarea.Status == TaskStatus.WaitingForActivation;
            }
            set
            {
                if (value && tarea is null || tarea.Status != TaskStatus.Running || tarea.Status != TaskStatus.WaitingToRun || tarea.Status != TaskStatus.WaitingForActivation)
                {
                    this.cancellation = new CancellationTokenSource();
                    this.TomarPedidos();
                }
                else
                {
                    this.cancellation.Cancel();
                }
            }
        }

        private void NotificarNuevoPedido()
        {
            if (this.OnPedido is not null)
            {
                menu = new T();
                menu.IniciarPreparacion();
                OnPedido.Invoke(menu);
            }
        }

        private void TomarPedidos() 
        {
            tarea = Task.Run(() =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    NotificarNuevoPedido();
                    Thread.Sleep(5000);
                }
            },
            cancellation.Token);
        }
    }
}
