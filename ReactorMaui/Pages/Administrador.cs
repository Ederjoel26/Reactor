using MauiReactor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Pages
{
    public class AdministradorState
    {
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string NumSerie { get; set; }
        public string nuevoCorreo { get; set; }
        public string nuevaContraseña { get; set; }
        public string selectedItem { get; set; }    
        public ObservableCollection<string> casasList { get; set; } = new ObservableCollection<string>();
    }
    public class Administrador: Component<AdministradorState>
    {
        public MauiControls.ListView listaCasas { get; set; }
        public override VisualNode Render()
        {
            return new ContentPage("Administrador")
            {
                new Grid("5*, 90*, 5*", "10*, 80*, 10*")
                {
                    new Grid("60*, 40*", "*")
                    {
                        new Frame()
                        {
                            new Grid("12*, 12*, 12*, 12*, 12*, 12*, 12*, 12*", "10*, 80*, 10*")
                            {
                                new Label("Inicio de sesión")
                                    .HCenter()
                                    .VCenter()
                                    .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                    .FontSize(18)
                                    .GridColumn(1)
                                    .GridRow(0),

                                new Entry()
                                    .Placeholder("Ingresar correo electronico")
                                    .GridColumn(1)
                                    .GridRow(1)
                                    .HCenter()
                                    .HFill()
                                    .TextColor(Color.Parse("black"))
                                    .OnTextChanged(text => SetState(s => s.Correo = text))
                                    .VCenter(),

                                new Entry()
                                    .Placeholder("Ingresar contraseña")
                                    .GridColumn(1)
                                    .GridRow(2)
                                    .HCenter()
                                    .HFill()
                                    .TextColor(Color.Parse("black"))
                                    .OnTextChanged(text => SetState(s => s.Contraseña = text))
                                    .VCenter(),

                                new Button("Iniciar sesión")
                                        .HCenter()
                                        .VCenter()
                                        .GridColumn(1)
                                        .GridRow(3),

                                new Label("Cambio de datos")
                                    .HCenter()
                                    .VCenter()
                                    .FontSize(18)
                                    .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                    .GridColumn(1)
                                    .GridRow(4),

                                new Entry()
                                    .Placeholder("Ingresar nuevo correo electronico")
                                    .GridColumn(1)
                                    .GridRow(5)
                                    .HCenter()
                                    .HFill()
                                    .TextColor(Color.Parse("black"))
                                    .OnTextChanged(text => SetState(s => s.nuevoCorreo = text))
                                    .VCenter(),

                                new Entry()
                                    .Placeholder("Ingresar nueva contraseña")
                                    .GridColumn(1)
                                    .GridRow(6)
                                    .HCenter()
                                    .HFill()
                                    .TextColor(Color.Parse("black"))
                                    .OnTextChanged(text => SetState(s => s.nuevaContraseña = text))
                                    .VCenter(),

                                new Button("Guardar cambios")
                                        .HCenter()
                                        .VCenter()
                                        .GridColumn(1)
                                        .GridRow(7)
                            }
                        }
                        .GridRow(0)
                        .GridColumn(0)
                        .BorderColor(Color.Parse("black")),

                        new Frame()
                        {
                            new Grid("20*, 40*, 40*", "*")
                            {
                                new Label("Menu Administrador")
                                    .HCenter()
                                    .VCenter()
                                    .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                    .GridColumn(0)
                                    .FontSize(18)
                                    .GridRow(0),

                                new Button("Desactivar casa o usuarios")
                                    .HCenter()
                                    .VCenter()
                                    .HFill()
                                    .GridColumn(0)
                                    .GridRow(1)
                                    .OnClicked(async () => await Navigation.PushAsync<Desactivar>()),

                                new Button("Revisar historial")
                                    .HCenter()
                                    .VCenter()
                                    .HFill()
                                    .GridColumn(0)
                                    .GridRow(2)
                                    .OnClicked(async () => await Navigation.PushAsync<Historial>())
                            }
                            .RowSpacing(10)
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
            };
        }
    }
}
