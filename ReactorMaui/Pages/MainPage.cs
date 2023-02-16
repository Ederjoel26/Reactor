using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using MauiReactor;
using Microsoft.Maui.Storage;
using Plugin.CloudFirestore;
using ReactorMaui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReactorMaui.Pages
{
    public class MainPageState
    {
        public int Counter { get; set; }
    }

    public class MainPage : Component<MainPageState>
    {
        public MauiControls.StackLayout Pagina;
        public MauiControls.Label Aviso;
        public override VisualNode Render()
        {

            return new NavigationPage()
            {
                new Shell
                {
                    new TabBar()
                    {
                        new Tab("Actuadores")
                        {
                            new ContentPage("Actuadores")
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
                                        new Button("Vehicular")
                                            .HCenter()
                                            .VCenter()
                                            .OnClicked(() => SetValor("Vehicular", true)),

                                        new Button("Peatonal")
                                            .HCenter()
                                            .VCenter()
                                            .OnClicked(() => SetValor("Peatonal", true)),

                                        new Button("Basura uno")
                                            .HCenter()
                                            .VCenter()
                                            .OnClicked(() => SetValor("Basura1", true)),

                                        new Button("Basura dos")
                                            .HCenter()
                                            .VCenter()
                                            .OnClicked(() => SetValor("Basura2", true)),

                                        new Button("Alarma encender")
                                            .HCenter()
                                            .VCenter()
                                            .OnClicked(() => SetValor("Alarma", true)),

                                        new Button("Alarma cerrar")
                                            .HCenter()
                                            .VCenter()
                                            .OnClicked(() => SetValor("Alarma", false))
                                    }
                                    .IsVisible(true)
                                }
                                .HCenter()
                                .VCenter()
                            }
                        },

                        new Tab("ALARMA")
                        {
                            new Alarma()
                        },

                        new Tab("CONFIGURACIÓN")
                        {
                            new Configuracion()
                        }
                    }
                }
            };
        }

        public async void SetValor(string Campo, bool Valor)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = Constantes.AuthSecretRealtime,
                BasePath = Constantes.BasePathRealtime
            };

            IFirebaseClient client = new FirebaseClient(config);

            FirebaseResponse res = await client.GetAsync("NumSerie");
            Actores actores = res.ResultAs<Actores>();

            string Mensaje = string.Empty;

            switch (Campo)
            {
                case "Vehicular":
                    actores.Vehicular = Valor;
                    Mensaje = "Abrio la puerta vehicular.";
                    break;
                case "Peatonal":
                    actores.Peatonal = Valor;
                    Mensaje = "Abrio la puerta peatonal.";
                    break;
                case "Basura1":
                    actores.Basura1 = Valor;
                    Mensaje = "Abrio el contenedor de basura 1.";
                    break;
                case "Basura2":
                    actores.Basura2 = Valor;
                    Mensaje = "Abrio el contenedor de basura 2.";
                    break;
                case "Alarma":
                    actores.Alarma = Valor;
                    Mensaje = Valor ? "Se activó la alarma." : "Se desactivó la alarma.";
                    break;
            }
            bool Sub = await SubirAccionHistorial(Mensaje);

            if (!Sub)
            {
                return;
            }

            await client.UpdateAsync("NumSerie", actores);
        }

        public void VerificarPreferences()
        {
            if (Preferences.Get("NumSerie", null) != null && Preferences.Get("Correo", null) != null)
            {
                Pagina.IsVisible = true;
                Aviso.IsVisible = false;
            }
        }

        public async Task<bool> SubirAccionHistorial(string Accion)
        {
            int EpochToday = EpochConvertidor.DateToEpoch(DateTime.Now);
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
                        if (correo.Correo.Trim() == Preferences.Get("Correo", null))
                        {
                            bandera = true;
                        }
                    }
                    catch
                    {

                    }
                });

                if (estatusPrivada.Estatus && contra.Contra == Preferences.Get("Contraseña", null) && bandera)
                {
                    var data = new Dictionary<string, string>
                    {
                        { EpochToday.ToString(), Accion }
                    };

                    await CrossCloudFirestore
                       .Current
                       .Instance
                       .Collection(Preferences.Get("NumSerie", null))
                       .Document("Usuarios")
                       .Collection("Usuarios")
                       .Document(Preferences.Get("NumCasa", null))
                       .Collection("Casa")
                       .Document("Historial")
                       .SetAsync(data);
                    //se sobre escribe en la base de datos
                    return true;
                }
                else
                {
                    Alerta.DesplegarAlerta("La privada a la que perteneces no está habilitada.");
                    return false;
                }
            }
            catch
            {
                Alerta.DesplegarAlerta("Aun no esta registrado en la aplicación, vaya al apartado de configuración.");
                return false;
            }
        }
    }
}