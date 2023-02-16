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
                        new Tab("SERVICIOS")
                        {
                            new ContentPage("Servicios")
                            {
                                new Grid("5*, 90*, 5*", "5*, 90*, 5*")
                                {
                                    new Grid("20*, 20*, 20*, 20*, 20*", "*")
                                    {
                                        new Grid("80*, 20*", "*")
                                        {
                                            new ImageButton("porton.png")
                                                .HCenter()
                                                .VCenter()
                                                .OnClicked(() => SetValor("Vehicular", true))
                                                .GridRow(0)
                                                .GridColumn(0), 

                                            new Label()
                                                .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                                .HCenter()
                                                .VCenter()
                                                .HorizontalTextAlignment(TextAlignment.Center)
                                                .Text("Abrir puerta vehicular")
                                                .GridRow(1)
                                                .GridColumn(0)
                                        }
                                        .GridColumn(0)
                                        .GridRow(0),

                                        new Grid("80*, 20*", "*")
                                        {
                                             new ImageButton("puerta_peatonal.png")
                                                .HCenter()
                                                .VCenter()
                                                .OnClicked(() => SetValor("Peatonal", true))
                                                .GridRow(0)
                                                .GridColumn(0),

                                            new Label()
                                                .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                                .HCenter()
                                                .VCenter()
                                                .HorizontalTextAlignment(TextAlignment.Center)
                                                .Text("Abrir puerta peatonal")
                                                .GridRow(1)
                                                .GridColumn(0)
                                        }
                                        .GridColumn(0)
                                        .GridRow(1),

                                        new Grid("80*, 20*", "*")
                                        {
                                             new ImageButton("contenedor_one.png")
                                                .HCenter()
                                                .VCenter()
                                                .OnClicked(() => SetValor("Basura1", true))
                                                .GridRow(0)
                                                .GridColumn(0),

                                            new Label()
                                                .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                                .HCenter()
                                                .VCenter()
                                                .HorizontalTextAlignment(TextAlignment.Center)
                                                .Text("Abrir contenedor de basura 1")
                                                .GridRow(1)
                                                .GridColumn(0)
                                        }
                                        .GridColumn(0)
                                        .GridRow(2),


                                        new Grid("80*, 20*", "*")
                                        {
                                             new ImageButton("contenedor_two.png")
                                                .HCenter()
                                                .VCenter()
                                                .OnClicked(() => SetValor("Basura2", true))
                                                .GridRow(0)
                                                .GridColumn(0),

                                            new Label()
                                                .HCenter()
                                                .VCenter()
                                                .HorizontalTextAlignment(TextAlignment.Center)
                                                .Text("Abrir contenedor de basura 2")
                                                .GridRow(1)
                                                .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                                .GridColumn(0)
                                        }
                                        .GridColumn(0)
                                        .GridRow(3),

                                        new Grid("80*, 20*", "*")
                                        {
                                             new ImageButton("alarma.png")
                                                .HCenter()
                                                .VCenter()
                                                .OnClicked(() => SetValor("Alarma", true))
                                                .GridRow(0)
                                                .GridColumn(0),

                                            new Label()
                                                .HCenter()
                                                .VCenter()
                                                .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                                .HorizontalTextAlignment(TextAlignment.Center)
                                                .Text("Activar la alarma")
                                                .GridRow(1)
                                                .GridColumn(0)
                                        }
                                        .GridColumn(0)
                                        .GridRow(4)

                                    }
                                    .GridRow(1)
                                    .GridColumn(1)
                                    .HCenter()
                                    .VCenter()
                                    .RowSpacing(10)
                                    .ColumnSpacing(10)

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
                        },

                        new Tab("HISTORIAL")
                        {
                            new Historial()
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
            Thread.Sleep(2000);
            switch (Campo)
            {
                case "Vehicular":
                    actores.Vehicular = !Valor;
                    break;
                case "Peatonal":
                    actores.Peatonal = !Valor;
                    break;
                case "Basura1":
                    actores.Basura1 = !Valor;
                    break;
                case "Basura2":
                    actores.Basura2 = !Valor;
                    break;
                case "Alarma":
                    actores.Alarma = !Valor;
                    Mensaje = Valor ? "Se activó la alarma." : "Se desactivó la alarma.";
                    break;
            }
            await client.UpdateAsync("NumSerie", actores);
        }

        //public void VerificarPreferences()
        //{
        //    if (Preferences.Get("NumSerie", null) != null && Preferences.Get("Correo", null) != null)
        //    {
        //        Pagina.IsVisible = true;
        //        Aviso.IsVisible = false;
        //    }
        //}

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
                    HistorialModel historial = new HistorialModel
                    {
                        Fecha = EpochToday,
                        NumeroCasa = Preferences.Get("NumCasa", null),
                        Observaciones = Accion
                    };

                    await CrossCloudFirestore
                       .Current
                       .Instance
                       .Collection(Preferences.Get("NumSerie", null))
                       .Document("Usuarios")
                       .Collection("Historial")
                       .AddAsync(historial);

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