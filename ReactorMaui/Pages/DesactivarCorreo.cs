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
        public bool CasaActiva { get; set; }
        public string IndexCasa { get; set; }
    }
    public class DesactivarCorreo : Component<DesactivarState>
    {
        public MauiControls.Picker pCasas { get; set; }
        public MauiControls.ListView lvCorreos { get; set; }    
        public override VisualNode Render()
        {
            return new ContentPage()
            {
                new Grid("5*,90*,5*", "10*, 80*, 10*")
                {
                    new Frame()
                    {
                        new Grid("20*, 60*, 20*", "70*, 30*")
                        {
                            new Picker(p => pCasas = p)
                                .Title("Selecciona una casa")
                                .ItemsSource(State.ListCasas)
                                .OnLoaded(LlenarPickerCasas)
                                .OnSelectedIndexChanged(ItemSeleccionado)
                                .GridRow(0)
                                .GridColumn(0)
                                .HFill()
                                .VCenter(),

                            new Switch()
                                .IsToggled(State.CasaActiva)
                                .HCenter()
                                .VCenter()
                                .GridColumn(1)
                                .GridRow(0),

                            new ScrollView()
                            {
                                new ListView(lv => lvCorreos = lv)
                                    .ItemsSource(State.Correos)
                                    .HCenter()
                            }
                            .HCenter()
                            .VCenter()
                            .GridRow(1) 
                            .GridColumnSpan(2),

                            new Button("Guardar cambios")
                                .HFill()
                                .VCenter()
                                .GridColumnSpan(2)
                                .GridRow(2)
                                .OnClicked(ActualizarItemSeleccioando)
                        }
                    }
                    .GridRow(1)
                    .GridColumn(1)
                    .BorderColor(Color.Parse("black"))
                }
            };
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

            correoCambiar.Estatus = State.CasaActiva;

            try
            {
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
