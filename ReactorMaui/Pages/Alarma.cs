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

    }

    public class Alarma: Component<AlarmaState>
    {
        public MauiControls.Label Aviso;
        public MauiControls.StackLayout Pagina;

        public override VisualNode Render()
        {

            return new ContentPage("Alarma")
            {
                new StackLayout()
                {
                    new Label(lbl => Aviso = lbl)
                        .Text("Favor de ingresar sus datos en el area de configuración")
                        .IsVisible(true)
                        .HCenter()
                        .VCenter()
                        .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Italic)
                        .FontSize(20)
                        .TextColor(Color.Parse("black"))
                        .HorizontalTextAlignment(TextAlignment.Center)
                        .IsVisible(false),

                    new StackLayout(sl => Pagina = sl)
                    {
                        new Button("Pánico.")
                            .HCenter()
                            .VCenter()
                            .OnClicked(() => EnviarNotificacion("Una persona se encuentra en pánico")),

                        new Button("Avistamiento de ladron.")
                            .HCenter()
                            .VCenter()
                            .OnClicked(() => EnviarNotificacion("Acabo de ver a un ladrón.")),

                        new Button("Roban carro.")
                            .HCenter()
                            .VCenter()
                            .OnClicked(() => EnviarNotificacion("Se estan robando un carro.")),

                        new Button("Asalto.")
                            .HCenter()
                            .VCenter()
                            .OnClicked(() => EnviarNotificacion("Estan asaltando a alguien.")),

                        new Button("Roban casa.")
                            .HCenter()
                            .VCenter()
                            .OnClicked(() => EnviarNotificacion("Un ladrón esta robando una casa.")),

                        new Button("Ambulancia.")
                            .HCenter()
                            .VCenter()
                            .OnClicked(() => EnviarNotificacion("Se necesita asistencia medica.")),

                        new Button("Apagar alarma.")
                            .HCenter()
                            .VCenter()
                            .OnClicked(DesactivarAlarma)

                    }
                    .IsVisible(true)
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
                to = $"/topics/{ Preferences.Get("NumSerie", null)}",
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

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Alerta.DesplegarAlerta("Alerta enviada.");
            }
        }

        public async void DesactivarAlarma()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = Constantes.AuthSecretRealtime,
                BasePath = Constantes.BasePathRealtime
            };

            IFirebaseClient client = new FirebaseClient(config);

            FirebaseResponse res = await client.GetAsync("NumSerie");
            Actores actores = res.ResultAs<Actores>();
            actores.Alarma = false;
            await client.UpdateAsync("NumSerie", actores);
        }
    }
}
