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
        public MauiControls.Button btnCambiar { get; set; }
        public override VisualNode Render()
        {
            return new ContentPage("Administrador")
            {
                new Grid("5*, 90*, 5*", "10*, 80*, 10*")
                {
                    new Grid("50*, 50*", "*")
                    {
                        new Frame()
                        {
                            new Grid("25*, 25*, 25*, 25*", "10*, 80*, 10*")
                            {
                                new Label("Inicio de sesión")
                                    .HCenter()
                                    .VCenter()
                                    .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                    .FontSize(20)
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
                                        .GridRow(3)
                                        .OnClicked(VerificarAdministrador),

                                
                            }
                        }
                        .GridRow(0)
                        .GridColumn(0)
                        .BorderColor(Color.Parse("black")),

                        new Frame()
                        {
                            new Grid("25*, 25*, 25*, 25*", "10*, 80*, 10*")
                            {
                                new Label("Cambio de datos")
                                    .HCenter()
                                    .VCenter()
                                    .FontSize(20)
                                    .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                    .GridColumn(1)
                                    .GridRow(0),

                                new Entry()
                                    .Placeholder("Ingresar nuevo correo electronico")
                                    .GridColumn(1)
                                    .GridRow(1)
                                    .HCenter()
                                    .HFill()
                                    .TextColor(Color.Parse("black"))
                                    .OnTextChanged(text => SetState(s => s.nuevoCorreo = text))
                                    .VCenter(),

                                new Entry()
                                    .Placeholder("Ingresar nueva contraseña")
                                    .GridColumn(1)
                                    .GridRow(2)
                                    .HCenter()
                                    .HFill()
                                    .TextColor(Color.Parse("black"))
                                    .OnTextChanged(text => SetState(s => s.nuevaContraseña = text))
                                    .VCenter(),

                                new Button(btn => btnCambiar = btn)
                                        .HCenter()
                                        .VCenter()
                                        .GridColumn(1)
                                        .GridRow(3)
                                        .IsEnabled(false)
                                        .Text("Guardar cambios.")
                                        .OnClicked(CambioDatosAdmin)
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

        public async void VerificarAdministrador()
        {
            if(!await Permisos.VerificarRegistro())
            {
                return;
            }
            try
            {
                var documentAdministrador = await CrossCloudFirestore
                                              .Current
                                              .Instance
                                              .Collection(Preferences.Get("NumSerie", null))
                                              .Document("Administrador")
                                              .GetAsync();

                AdministradorModel administrador = documentAdministrador.ToObject<AdministradorModel>();
                if (administrador.Correo != State.Correo.Trim() || administrador.Contraseña != State.Contraseña.Trim())
                {
                    Alerta.DesplegarAlerta("Los datos ingresados son incorrectos.");
                    return;
                }
                btnCambiar.IsEnabled = true;
                Alerta.DesplegarAlerta("Administrador validado");
                DesactivarCasa.cpDesactivarCasas.IsVisible= true;
                DesactivarCorreo.cpDesactivarUsuario.IsVisible = true;
                Historial.cpHistorial.IsVisible = true;
            }
            catch
            {
                Alerta.DesplegarAlerta("Algo salio mal, intentelo más tarde.");
            }
            
        }

        public async void CambioDatosAdmin()
        {
            try
            {
                await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection(Preferences.Get("NumSerie", null))
                    .Document("Administrador")
                    .SetAsync(new AdministradorModel
                    {
                        Contraseña = State.nuevaContraseña.Trim(),
                        Correo = State.nuevoCorreo.Trim()
                    });
                Alerta.DesplegarAlerta("Datos actualizados con éxito.");
            }
            catch
            {
                Alerta.DesplegarAlerta("Algo salio mal, intentelo más tarde.");
            }
            
        }
    }
}
