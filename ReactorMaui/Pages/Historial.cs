using MauiReactor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using Microsoft.Extensions.Logging.Abstractions;
using CommunityToolkit.Maui.Behaviors;
using ReactorMaui.Models;
using Plugin.CloudFirestore;
using System.Collections.ObjectModel;
using Microsoft.Maui.Storage;

namespace ReactorMaui.Pages
{

    public class HistorialState
    {
        public DateTime Date { get; set; }
        public ObservableCollection<string> historialList { get; set; } = new ObservableCollection<string>();
    }
    public class Historial : Component<HistorialState>
    {
        public MauiControls.DatePicker datePicker;
        public MauiControls.ListView listView;
        public override VisualNode Render()
        {
            return new ContentPage("Historial")
            {
                new Grid("5*, 90*, 5*", "5*, 90*, 5*")
                {
                    new Grid("20*, 80*", "*")
                    {
                        new Frame()
                        {
                            new Grid("20*,80*", "*")
                            {
                                new Label()
                                    .Text("Seleccione una fecha:")
                                    .FontSize(15)
                                    .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                    .HCenter()
                                    .VCenter(),

                                new DatePicker(dp => datePicker = dp)
                                    .OnLoaded(LlenarListView)
                                    .OnDateSelected(LlenarListView)
                                    .HCenter()
                                    .VCenter()
                                    .GridColumn(0)
                                    .GridRow(1)
                            }
                        }
                        .BorderColor(Color.Parse("black"))
                        .GridRow(0)
                        .GridColumn(0),

                        new Frame()
                        {
                            new ScrollView()
                            {
                                new ListView(lv => listView = lv)
                                    .ItemsSource(State.historialList)
                            }
                        }
                        .GridColumn(0)
                        .GridRow(1)
                        .BorderColor(Color.Parse("black"))
                    }
                    .GridColumn(1)
                    .GridRow(1)
                    .RowSpacing(10)
                }
            }
            .BackgroundImageSource("fondo_historial.png");
        }

        public async void LlenarListView()
        {
            bool estaRegistrado = await Permisos.VerificarRegistro();

            if (!estaRegistrado)
            {
                datePicker.IsEnabled = false;
                return;
            }

            DateTime day = datePicker.Date;
            int Fecha = EpochConvertidor.DateToEpoch(day);
            int FechaLimiteDia = Fecha + 86399;

            var documentHistorial = await CrossCloudFirestore
                                        .Current
                                        .Instance
                                        .Collection(Preferences.Get("NumSerie", null))
                                        .Document("Usuarios")
                                        .Collection("Historial")
                                        .GetAsync();

            SetState(s => s.historialList.Clear());

            documentHistorial.Documents.ToList().ForEach(document =>
            {
                HistorialModel registro = document.ToObject<HistorialModel>();
                if (registro.Fecha >= Fecha && registro.Fecha <= FechaLimiteDia)
                {
                    string fechaAccion = EpochConvertidor.EpochToHour(registro.Fecha);

                    SetState(s => s.historialList.Add($"{fechaAccion} Casa: {registro.NumeroCasa}, {registro.Observaciones}"));
                }
            });

            if (State.historialList.Count == 0)
            {
                SetState(s => s.historialList.Add("Aún no hay registro de actividad."));
            }

            listView.ItemsSource = State.historialList;
        }
    }
}
