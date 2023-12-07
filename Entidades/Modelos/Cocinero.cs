using Entidades.DataBase;
using Entidades.Excepciones;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;


namespace Entidades.Modelos
{
    public delegate void DelegadoDemoraAtencion(double demora);
    public delegate void DelegadoPedidoEnCurso<T>(T menu);
    public delegate void DelegadoNuevoPedido(IComestible menu);

    public class Cocinero<T> where T : IComestible, new()
    {
        private CancellationTokenSource cancellation;
        private int cantPedidosFinalizados;
        private double demoraPreparacionTotal;
        private Mozo<T> mozo;
        private string nombre;
        private T pedidoEnPreparacion;
        private Queue<T> pedidos;
        private Task tarea;
        public event DelegadoDemoraAtencion OnDemora;
        public event DelegadoNuevoPedido OnPedido;

        public Cocinero(string nombre)
        {
            this.nombre = nombre;
            this.mozo = new Mozo<T>();
            this.pedidos = new Queue<T>();
            this.mozo.OnPedido += TomarNuevoPedido;
        }

        //No hacer nada
        public bool HabilitarCocina
        {
            get
            {
                return this.tarea is not null && (this.tarea.Status == TaskStatus.Running ||
                this.tarea.Status == TaskStatus.WaitingToRun ||
                this.tarea.Status == TaskStatus.WaitingForActivation);
            }
            set
            {
                if (value && !this.HabilitarCocina)
                {
                    mozo.EmpezarATrabajar = true;
                    this.EmpezarACocinar();
                }
                else
                {
                    this.cancellation.Cancel();
                    mozo.EmpezarATrabajar = false;
                }
            }
        }

        //no hacer nada
        public double TiempoMedioDePreparacion { get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados; }
        public string Nombre { get => nombre; }
        public int CantPedidosFinalizados { get => cantPedidosFinalizados; }
        public Queue<T> Pedidos { get; }


        private void EmpezarACocinar()
        {
            tarea = Task.Run(() =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    if (this.OnPedido != null)
                    {
                        if (pedidos.Count > 0)
                        {
                            this.pedidoEnPreparacion = pedidos.Dequeue();
                            this.OnPedido.Invoke(this.pedidoEnPreparacion);
                            this.EsperarProximoIngreso();
                            cantPedidosFinalizados++;
                            DataBaseManager.GuardarTicket(nombre, this.pedidoEnPreparacion);
                        }
                    }
                }   
            },
            cancellation.Token);
        }

        private void EsperarProximoIngreso()
        {
            if (this.OnDemora is not null)
            {
                int tiempoEspera = 0;

                while (!cancellation.IsCancellationRequested)
                {
                    OnDemora.Invoke(tiempoEspera);

                    Thread.Sleep(1000);

                    tiempoEspera++;
                }
                demoraPreparacionTotal += tiempoEspera;
            }
        }

        private void TomarNuevoPedido(T menu)
        {
            if (OnPedido is not null)
            {
                this.pedidos.Enqueue(menu);
            }
        }
    }
}
