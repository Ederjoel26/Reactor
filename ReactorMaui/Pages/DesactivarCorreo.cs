using MauiReactor;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Plugin.CloudFirestore;
using ReactorMaui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Pages
{
    public class DesactivarState
    {
        public List<string> ListCasas { get; set; } = new List<string>();
        public ObservableCollection<string> Correos { get; set; } = new ObservableCollection<string>();
        public string IndexCasa { get; set; }
    }
    public class DesactivarCorreo : Component<DesactivarState>
    {
        public MauiControls.Picker pCasas { get; set; }
        public MauiControls.ListView lvCorreos { get; set; }    
        public MauiControls.Switch swCasaActiva { get; set; }
        public static MauiControls.ContentPage cpDesactivarUsuario { get; set; }
        public override VisualNode Render()
        {
            return new ContentPage(cp => cpDesactivarUsuario = cp)
            {
                new Grid("5*,90*,5*", "10*, 80*, 10*")
                {
                    new Frame()
                    {
                        new Grid("10*, 10*, 60*, 20*", "70*, 30*")
                        {
                            new Label("Actualizar correo")
                                .HCenter()
                                .VCenter()
                                .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                .FontSize(17)
                                .GridColumnSpan(2)
                                .GridRow(0),
                                

                            new Picker(p => pCasas = p)
                                .Title("Selecciona una casa")
                                .ItemsSource(State.ListCasas)
                                .OnLoaded(LlenarPickerCasas)
                                .OnSelectedIndexChanged(ItemSeleccionado)
                                .GridRow(1)
                                .GridColumn(0)
                                .HFill()
                                .VCenter(),

                            new Switch(sw => swCasaActiva = sw)
                                .HCenter()
                                .VCenter()
                                .GridColumn(1)
                                .GridRow(1)
                                .OnColor(Color.Parse("black")),

                            new Frame()
                            {
                                new ScrollView()
                                {
                                    new ListView(lv => lvCorreos = lv)
                                        .ItemsSource(State.Correos)
                                }
                            }
                            .GridRow(2) 
                            .GridColumnSpan(2)
                            .BorderColor(Color.Parse("black")),

                            new Button("Guardar cambios")
                                .HFill()
                                .VCenter()
                                .GridColumnSpan(2)
                                .GridRow(3)
                                .OnClicked(ActualizarItemSeleccioando)
                        }
                    }
                    .GridRow(1)
                    .GridColumn(1)
                    .BorderColor(Color.Parse("black"))
                }
            }.IsVisible(false);
        }

        public void LlenarPickerCasas()
        {
            for(int i = 1; i < 100; i++)
            {
                if(i > 0 && i < 10)
                {
                    SetState(s => s.ListCasas.Add($"Casa 0{i}"));
                }
                else
                {
                    SetState(s => s.ListCasas.Add($"Casa {i}"));
                }
            }

            pCasas.ItemsSource = State.ListCasas;
        }

        public async void ItemSeleccionado()
        {
            int length = pCasas.SelectedItem.ToString().Length;
            string item = pCasas.SelectedItem.ToString();
            SetState(s => s.IndexCasa = $"{item[length - 2]}{item[length - 1]}");
            try
            {
                var documentCorreos = await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection(Preferences.Get("NumSerie", null))
                    .Document("Usuarios")
                    .Collection("Usuarios")
                    .Document(State.IndexCasa)
                    .Collection("Casa")
                    .GetAsync();

                SetState(s => s.Correos.Clear());
                documentCorreos.Documents.ToList().ForEach(document =>
                {
                    CorreoModel correo = document.ToObject<CorreoModel>();
                    if(correo.Correo != null)
                    {
                        SetState(s => s.Correos.Add(correo.Correo));
                    }
                });

                if(State.Correos.Count == 0)
                {
                    SetState(s => s.Correos.Add("No hay correos en esta casa"));
                }

                lvCorreos.ItemsSource = State.Correos;
            }
            catch
            {
                Alerta.DesplegarAlerta("La casa seleccionada no existe");
            }
        }

        public async void ActualizarItemSeleccioando()
        {
            if (!await Permisos.VerificarRegistro())
            {
                return;
            }

            if(lvCorreos.SelectedItem == null)
            {
                Alerta.DesplegarAlerta("Selecciona un correo");
                return;
            }

            string id = string.Empty;
            CorreoModel correoCambiar = new CorreoModel();

            var documentCorreos = await CrossCloudFirestore
                .Current
                .Instance
                .Collection(Preferences.Get("NumSerie", null))
                .Document("Usuarios")
                .Collection("Usuarios")
                .Document(State.IndexCasa)
                .Collection("Casa")
                .GetAsync();

            documentCorreos.Documents.ToList().ForEach(document =>
            {
                try
                {
                    CorreoModel correo = document.ToObject<CorreoModel>();
                    if (correo.Correo.Trim() == lvCorreos.SelectedItem.ToString().Trim())
                    {
                        id = document.Id;
                        correoCambiar = correo;
                    }
                }
                catch
                {

                }
            });
            try
            {
                correoCambiar.Estatus = swCasaActiva.IsToggled;

            
                await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection(Preferences.Get("NumSerie", null))
                    .Document("Usuarios")
                    .Collection("Usuarios")
                    .Document(State.IndexCasa)
                    .Collection("Casa")
                    .Document(id)
                    .SetAsync(correoCambiar);

                Alerta.DesplegarAlerta("Cambios guardados correctamente");

            }
            catch
            {
                Alerta.DesplegarAlerta("Algo salio mal, intentelo más tarde.");
            }

        }
    }
}
