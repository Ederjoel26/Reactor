using MauiReactor;
using Microsoft.Maui.Storage;
using Plugin.CloudFirestore;
using ReactorMaui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Pages
{
    public class DesactivarCasaState
    {
        public ObservableCollection<string> cCasas = new ObservableCollection<string>();
        public string IndexCasa { get; set; }
    }
    public class DesactivarCasa : Component<DesactivarCasaState>
    {
        public static MauiControls.ContentPage cpDesactivarCasas { get; set; }
        public MauiControls.ListView lvCasas { get; set; }
        public MauiControls.Switch swCasaActiva { get; set; }
        public override VisualNode Render()
        {

            return new ContentPage(cp => cpDesactivarCasas = cp)
            {
                new Grid("5*, 90*, 5*", "10*, 80*, 10*")
                {
                    new Frame()
                    {
                        new Grid("10*, 70*, 10*, 10*", "*")
                        {
                            new Label("Actualizar casa")
                                .HCenter()
                                .VCenter()
                                .GridColumn(0)
                                .GridRow(0)
                                .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                .FontSize(18),
                            
                            new Frame()
                            {
                                new ScrollView()
                                {
                                    new ListView(lv => lvCasas = lv)
                                        .OnLoaded(LlenarLSCasas)
                                        .ItemsSource(State.cCasas)
                                        .OnItemSelected(CambiarSwitch)
                                    
                                }
                            }
                            .BorderColor(Color.Parse("black"))
                            .GridColumn(0)
                            .GridRow(1),
                            
                            new Switch(sw => swCasaActiva = sw)
                                .HCenter()
                                .VCenter()
                                .GridRow(2)
                                .GridColumn(0)
                                .OnColor(Color.Parse("#0066ff")),

                            new Button("Guardar cambios")
                                .HFill()
                                .VCenter()
                                .GridColumn(0)
                                .GridRow(3)
                                .OnClicked(CasaSeleccionada)
                        }
                    }
                    .GridRow(1)
                    .GridColumn(1)
                    .BorderColor(Color.Parse("black"))
                }
            }
            .IsVisible(false)
            .BackgroundImageSource("desactivar_casa.png")
            .Title("Actualizar estatus");
        }

        public void LlenarLSCasas()
        {
            for(int i = 1; i < 100; i++)
            {
                if(i > 0 && i < 10)
                {
                    SetState(s => s.cCasas.Add($"Casa 0{i}"));
                }
                else
                {
                    SetState(s => s.cCasas.Add($"Casa {i}"));
                }
            }

            lvCasas.ItemsSource = State.cCasas;
        }

        public async void CambiarSwitch()
        {
            if(!await Permisos.VerificarRegistro())
            {
                return;
            }

            int length = lvCasas.SelectedItem.ToString().Length;
            string item = lvCasas.SelectedItem.ToString();
            SetState(s => s.IndexCasa = $"{item[length - 2]}{item[length - 1]}");

            var documentCasa = await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection(Preferences.Get("NumSerie", null))
                    .Document("Usuarios")
                    .Collection("Usuarios")
                    .Document(State.IndexCasa)
                    .Collection("Casa")
                    .Document("Estatus")
                    .GetAsync();

            if(documentCasa.Data == null)
            {
                Alerta.DesplegarAlerta("La casa seleccionada no existe");
                swCasaActiva.IsToggled = false;
            }
            else
            {
                EstatusModel estatus = documentCasa.ToObject<EstatusModel>();
                swCasaActiva.IsToggled = estatus.Estatus;
            }
        }

        public async void CasaSeleccionada()
        {
            if(!await Permisos.VerificarRegistro())
            {
                return;
            }
            
            if(lvCasas.SelectedItem.ToString().Length <= 0)
            {
                Alerta.DesplegarAlerta("Seleccione una casa");
                return;
            }

            try
            {
                var documentCasa = await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection(Preferences.Get("NumSerie", null))
                    .Document("Usuarios")
                    .Collection("Usuarios")
                    .Document(State.IndexCasa)
                    .Collection("Casa")
                    .Document("Estatus")
                    .GetAsync();

                if (documentCasa.Data == null)
                {
                    Alerta.DesplegarAlerta("La casa seleccionada no existe.");
                    return;
                }
                await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection(Preferences.Get("NumSerie", null))
                    .Document("Usuarios")
                    .Collection("Usuarios")
                    .Document(State.IndexCasa)
                    .Collection("Casa")
                    .Document("Estatus")
                    .SetAsync(new EstatusModel { Estatus = swCasaActiva.IsToggled});

                Alerta.DesplegarAlerta("Cambios guardados correctamente.");
            }
            catch
            {
                Alerta.DesplegarAlerta("Algo salio mal, intentelo más tarde.");
            }
        }
    }
}
