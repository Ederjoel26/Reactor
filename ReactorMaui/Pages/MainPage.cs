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
                        new Tab("SERVICIOS", "servicios.png")
                        {
                            new ContentPage("Servicios")
                            {
                                new Grid("10*, 80*, 10*", "10*, 80*, 10*")
                                {
                                    new Frame()
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
                                    .GridRow(1)
                                    .GridColumn(1)
                                    .BorderColor(Color.Parse("black"))
                                }
                            }
                            .BackgroundImageSource("fondo_mainpage.png")
                        },

                        new Tab("ALERTAS", "alerta.png")
                        {
                            new Alarma()
                        },

                        new Tab("CONFIGURACIÓN", "configuracion.png")
                        {
                            new Configuracion()
                        },

                        new Tab("ADMINISTRADOR", "administrador.png")
                        {
                            new Administrador()
                        },

                        new Tab("HISTORIAL")
                        {
                            new Historial()
                        },

                        new Tab("CORREO")
                        {
                            new DesactivarCorreo()
                        },

                        new Tab("CASAS")
                        {
                            new DesactivarCasa()
                        }
                    }
                }
            };
        }

        public async void SetValor(string Campo, bool Valor)
        {
            bool estaRegistrado = await Permisos.VerificarRegistro();

            if (!estaRegistrado)
            {
                return;
            }

            Alerta.DesplegarAlerta("Instrucción enviada.");

            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = Constantes.AuthSecretRealtime,
                BasePath = Constantes.BasePathRealtime
            };

            IFirebaseClient client = new FirebaseClient(config);

            FirebaseResponse res = await client.GetAsync(Preferences.Get("NumSerie", null));
            Actores actores = res.ResultAs<Actores>();

            string Mensaje = string.Empty;

            switch (Campo)
            {
                case "Vehicular":
                    actores.Vehicular = Valor;
                    Mensaje = "Abrió la puerta vehicular.";
                    break;
                case "Peatonal":
                    actores.Peatonal = Valor;
                    Mensaje = "Abrió la puerta peatonal.";
                    break;
                case "Basura1":
                    actores.Basura1 = Valor;
                    Mensaje = "Abrió el contenedor de basura 1.";
                    break;
                case "Basura2":
                    actores.Basura2 = Valor;
                    Mensaje = "Abrió el contenedor de basura 2.";
                    break;
                case "Alarma":
                    actores.Alarma = Valor;
                    Mensaje = Valor ? "Se activó la alarma." : "Se desactivó la alarma.";
                    break;
            }

            await client.UpdateAsync(Preferences.Get("NumSerie", null), actores);

            Auditoria.SubirAccionHistorial(Mensaje);
        }
    }
}