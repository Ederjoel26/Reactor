using MauiReactor;
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
        public List<string> Correos { get; set; } = new List<string>();
        public List<string> Casas { get; set; } = new List<string>();
        public int? casaSeleccionadaDesactivar { get; set; }
        public int? casaSeleccionada { get;set; }
        public bool EstadoUsuarios { get; set; }
        public string IndexCasa { get; set; }
        public bool EstadoCasas { get; set; }
    }
    public class Desactivar : Component<DesactivarState>
    {
        public MauiControls.Picker pickerUsuarios;
        public MauiControls.Picker pickerCasasUsuarios;
        public MauiControls.Picker pickerCasas;
        public override VisualNode Render()
        {
            InicializarCasas();

            return new ContentPage()
            {
                new Grid("5*,90*,5*", "10*, 80*, 10*")
                {
                    new Grid("60*, 40*", "*")
                    {
                        new Frame()
                        {
                            new Grid("10*, 30*, 30*, 30*", "70*, 30*")
                            {
                                new Label("Actualizar datos usuario")
                                    .HorizontalTextAlignment(TextAlignment.Center)
                                    .FontSize(18)
                                    .HCenter()
                                    .VCenter()
                                    .GridRow(0)
                                    .GridColumnSpan(2),

                                new Picker()
                                    .ItemsSource(State.Casas)
                                    .GridColumnSpan(2)
                                    .GridRow(1)
                                    .HCenter()
                                    .VCenter()
                                    .HFill()
                                    .Title("Selecciona una casa")
                                    .OnSelectedIndexChanged(LlenarPicker),

                                new Switch()
                                    .IsToggled(State.EstadoUsuarios)
                                    .HCenter()
                                    .VCenter()
                                    .GridColumn(1)
                                    .GridRow(2),

                                new Picker(lv => pickerUsuarios = lv)
                                    .ItemsSource(State.Correos)
                                    .GridColumn(0)
                                    .GridRow(2)
                                    .IsEnabled(false)
                                    .Title("Selecciona un usuario")
                                    .HCenter()
                                    .HFill()
                                    .VCenter(),

                                new Button("Desactivar usuarios")
                                    .HCenter()
                                    .VCenter()
                                    .HFill()
                                    .GridRow(3)
                                    .GridColumnSpan(2)
                                        
                            }
                        }
                        .GridRow(0)
                        .GridColumn(0),

                        new Frame()
                        {
                            new Grid("10*, 45*, 45*", "70*, 30*")
                            {

                                new Label("Actualizar datos casa")
                                    .HCenter()
                                    .VCenter()
                                    .HFill()
                                    .HorizontalTextAlignment(TextAlignment.Center)
                                    .FontSize(18)
                                    .GridColumnSpan(2)
                                    .GridRow(0),

                                new Picker(p => pickerCasas = p)
                                    .ItemsSource(State.Casas)
                                    .GridColumn(0)
                                    .GridRow(1)
                                    .Title("Selecciona una casa")
                                    .HCenter()
                                    .VCenter()
                                    .HFill(),

                                new Switch()
                                    .IsToggled(State.EstadoCasas)
                                    .HCenter()
                                    .VCenter()
                                    .GridColumn(1)
                                    .GridRow(1),

                                new Button("Guardar cambios")
                                    .HCenter()
                                    .VCenter()
                                    .HFill()
                                    .GridRow(2)
                                    .GridColumnSpan(2)
                            }
                        }
                        .GridRow(1)
                        .GridColumn(0)
                    }
                    .GridRow(1)
                    .GridColumn(1) 
                    .RowSpacing(10)
                    .ColumnSpacing(10)
                }
            };
        }

        public async void LlenarPicker()
        {
            SetState(s => s.casaSeleccionada = pickerCasas.SelectedIndex);
            SetState(s => s.casaSeleccionada++);

            if (State.casaSeleccionada > 0 && State.casaSeleccionada <= 9)
            {
                SetState(s => s.IndexCasa = "0" + State.casaSeleccionada);
            }
            else
            {
                SetState(s => s.IndexCasa = State.casaSeleccionada.ToString());
            }

            var documentCasa = await CrossCloudFirestore
                                        .Current
                                        .Instance
                                        .Collection(Preferences.Get("NumSerie", null))
                                        .Document("Usuarios")
                                        .Collection("Usuarios")
                                        .Document(State.IndexCasa)
                                        .Collection("Casa")
                                        .GetAsync();

            SetState(s => s.Correos.Clear());
            documentCasa.Documents.ToList().ForEach(document =>
            {
                try
                {
                    CorreoModel correo = document.ToObject<CorreoModel>();
                    if(correo.Correo != null)
                    {
                        SetState(s => s.Correos.Add(correo.Correo));
                    }

                }catch
                {

                }
            });

            if (State.Correos.Count == 0)
            {
                SetState(s => s.Correos.Add("Aún no hay registro de actividad."));
            }

            pickerUsuarios.ItemsSource = State.Correos;
            pickerUsuarios.IsEnabled = true;
        }

        public void InicializarCasas()
        {
            for (int i = 1; i <= 100; i++)
            {
                SetState(s => s.Casas.Add($"Casa {i}"));
            }
        }
    }
}
