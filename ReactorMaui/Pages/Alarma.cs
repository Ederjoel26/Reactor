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

            return new ContentPage("Alarma")
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
                                        EnviarNotificacion(State.AlarmaMensaje);
                                        ActivaroDesactivarAlarma(true);
                                    })
                            }
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
                                        EnviarNotificacion(State.AlarmaMensaje);
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
                                        EnviarNotificacion("Se estan robando un carro.");
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
                                        EnviarNotificacion("Estan asaltando a alguien.");
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
            };
        }

        public async void EnviarNotificacion(string Mensaje)
        {
            try
            {
                var documentPrivada = await CrossCloudFirestore
                                        .Current
                                        .Instance
                                        .Collection(Preferences.Get("NumSerie", null))
                                        .Document("Estatus")
                                        .GetAsync();

                var documentContra = await CrossCloudFirestore
                                        .Current
                                        .Instance
                                        .Collection(Preferences.Get("NumSerie", null))
                                        .Document("Contra")
                                        .GetAsync();

                var documentCasa = await CrossCloudFirestore
                                        .Current
                                        .Instance
                                        .Collection(Preferences.Get("NumSerie", null))
                                        .Document("Usuarios")
                                        .Collection("Usuarios")
                                        .Document(Preferences.Get("NumCasa", null))
                                        .Collection("Casa")
                                        .GetAsync();

                EstatusModel estatusPrivada = documentPrivada.ToObject<EstatusModel>();
                ContraModel contra = documentContra.ToObject<ContraModel>();

                bool bandera = false;
                documentCasa.Documents.ToList().ForEach(document =>
                {
                    try
                    {
                        CorreoModel correo = document.ToObject<CorreoModel>();
                        if (correo.Correo.Trim() == Preferences.Get("Correo", null) && correo.Estatus)
                        {
                            bandera = true;
                        }
                    }
                    catch
                    {

                    }
                });

                if (!estatusPrivada.Estatus)
                {
                    Alerta.DesplegarAlerta("La privada no está habilitada.");
                    return;
                }

                if(contra.Contra != Preferences.Get("Contraseña", null))
                {
                    Alerta.DesplegarAlerta("La contraseña es incorrecta.");
                    return;
                }

                if (!bandera)
                {
                    Alerta.DesplegarAlerta("El correo no es valido o no está habilitado");
                    return;
                }
            }
            catch(Exception ex)
            {
                Alerta.DesplegarAlerta("Aun no esta registrado en la aplicación, vaya al apartado de configuración.");
                return;
            }

            PushNotificationRequest notificacion = new PushNotificationRequest()
            {
                to = $"/topics/{Preferences.Get("NumSerie", null)}",
                notification = new NotificationMessageBody()
                {
                    title = "¡ALERTA!",
                    body = Mensaje
                }
            };

            string url = "https://fcm.googleapis.com/fcm/send";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("key", "=" + Constantes.Autorizacion);

            string serializarRequest = JsonConvert.SerializeObject(notificacion);
            var response = await client.PostAsync(url, new StringContent(serializarRequest, Encoding.UTF8, "application/json"));

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Alerta.DesplegarAlerta("Alerta enviada.");
            }
        }

        public async void ActivaroDesactivarAlarma(bool Alarma)
        {
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
        }
    }
}
