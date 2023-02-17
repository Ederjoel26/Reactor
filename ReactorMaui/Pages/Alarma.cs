using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using FireSharp;
using MauiReactor;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using Plugin.CloudFirestore;
using ReactorMaui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Pages
{
    public class AlarmaState
    {
        public string AlarmaMensaje { get; set; }
    }

    public class Alarma: Component<AlarmaState>
    {
        public MauiControls.Label Aviso;
        public MauiControls.StackLayout Pagina;

        public override VisualNode Render()
        {

            return new ContentPage("Alertas")
            { 
                new Grid("5*, 90*, 5*", "5*, 90*, 5*")
                {
                    new Grid("30*, 70*", "*")
                    {
                        new Frame()
                        {
                            new Grid("5*, 30*, 30*, 30*, 5*", "10*, 80*, 10*")
                            {
                                new Entry()
                                    .Placeholder("Escriba el mensaje de alerta")
                                    .GridColumn(1)
                                    .GridRow(1)
                                    .HCenter()
                                    .HFill()
                                    .TextColor(Color.Parse("black"))
                                    .OnTextChanged(text => SetState(s => s.AlarmaMensaje = text))
                                    .VCenter(),

                                new Button()
                                    .Text("Enviar mensaje y activar alarma")
                                    .GridColumn(1)
                                    .GridRow(2)
                                    .VCenter()
                                    .OnClicked(() =>
                                    {

                                        if(State.AlarmaMensaje == null)
                                        {
                                            Alerta.DesplegarAlerta("Favor de escribir el mensaje de alerta.");
                                            return;
                                        }

                                        EnviarNotificacion(State.AlarmaMensaje);
                                        ActivaroDesactivarAlarma(true);
                                    }),

                                new Button()
                                    .Text("Enviar mensaje sin activar alarma")
                                    .GridColumn(1)
                                    .GridRow(3)
                                    .VCenter()
                                    .OnClicked(() =>
                                    {
                                        if(State.AlarmaMensaje == null)
                                        {
                                            Alerta.DesplegarAlerta("Favor de escribir el mensaje de alerta.");
                                            return;
                                        }
                                        EnviarNotificacion(State.AlarmaMensaje);
                                    })
                            }
                            .RowSpacing(5)
                            .ColumnSpacing(5)
                        }
                        .GridColumn(0)
                        .GridRow(0)
                        .BorderColor(Color.Parse("black")),

                        new Frame()
                        {
                            new Grid("25*, 25*, 25*, 25*", "50*, 50*")
                            {
                                new ImageButton("ayuda.png")
                                    .HCenter()
                                    .VCenter()
                                    .OnClicked(() => {
                                        EnviarNotificacion("Necesito ayuda urgentemente.");
                                        ActivaroDesactivarAlarma(true);
                                    })
                                    .GridColumn(0)
                                    .GridRow(0)
                                    .Margin(5),

                                new ImageButton("ver_ladron.png")
                                    .HCenter()
                                    .VCenter()
                                    .OnClicked(() =>
                                    {
                                        EnviarNotificacion("Acabo de ver a un ladrón.");
                                        ActivaroDesactivarAlarma(true);
                                    })
                                    .GridColumn(0)
                                    .GridRow(1)
                                    .Margin(5),

                                new ImageButton("ladron_auto.png")
                                    .HCenter()
                                    .VCenter()
                                    .OnClicked(() =>
                                    {
                                        EnviarNotificacion("Se están robando un carro.");
                                        ActivaroDesactivarAlarma(true);
                                    })
                                    .GridColumn(0)
                                    .GridRow(2)
                                    .Margin(5),

                                new ImageButton("ladron_robando.png")
                                    .HCenter()
                                    .VCenter()
                                    .OnClicked(() =>
                                    {
                                        EnviarNotificacion("Están asaltando a alguien.");
                                        ActivaroDesactivarAlarma(true);
                                    })
                                    .GridRow(0)
                                    .GridColumn(1)
                                    .Margin(5),

                                new ImageButton("ladron_casa.png")
                                    .HCenter()
                                    .VCenter()
                                    .OnClicked(() =>
                                    {
                                        EnviarNotificacion("Un ladrón esta robando una casa.");
                                        ActivaroDesactivarAlarma(true);
                                    })
                                    .GridColumn(1)
                                    .GridRow(1)
                                    .Margin(5),

                                new ImageButton("emergencia.png")
                                    .HCenter()
                                    .VCenter()
                                    .OnClicked(() =>
                                    {
                                        EnviarNotificacion("Se necesita asistencia medica.");
                                        ActivaroDesactivarAlarma(true);
                                    })
                                    .GridColumn(1)
                                    .GridRow(2)
                                    .Margin(5),

                                new ImageButton("alarma_desactivar.png")
                                    .HCenter()
                                    .VCenter()
                                    .OnClicked(() => ActivaroDesactivarAlarma(false))
                                    .GridRow(3)
                                    .GridColumnSpan(2)
                                    .Margin(5)
                            }
                        }
                        .GridColumn(0)
                        .GridRow(1)
                        .BorderColor(Color.Parse("black"))
                    }
                    .GridRow(1)
                    .GridColumn(1)
                    .ColumnSpacing(10)
                    .RowSpacing(10)
                }
                .HCenter()
                .VCenter()
            }
            .BackgroundImageSource("fondo_alertas.png");
        }

        public async void EnviarNotificacion(string Mensaje)
        {
            bool estaRegistrado = await Permisos.VerificarRegistro();

            if(!estaRegistrado)
            {
                return; 
            }

            Notificaciones.MandarNotificacion("¡ALERTA!", Mensaje);
            Alerta.DesplegarAlerta("Mensaje enviado.");
        }

        public async void ActivaroDesactivarAlarma(bool Alarma)
        {
            bool estaRegistrado = await Permisos.VerificarRegistro();

            if (!estaRegistrado)
            {
                return;
            }

            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = Constantes.AuthSecretRealtime,
                BasePath = Constantes.BasePathRealtime
            };

            IFirebaseClient client = new FirebaseClient(config);

            FirebaseResponse res = await client.GetAsync("NumSerie");
            Actores actores = res.ResultAs<Actores>();
            actores.Alarma = Alarma;
            await client.UpdateAsync("NumSerie", actores);
            string mensaje = Alarma ? "Se activó la alarma." : "Se desactivó la alarma.";
            Auditoria.SubirAccionHistorial(mensaje);
            Alerta.DesplegarAlerta(mensaje);
        }
    }
}
