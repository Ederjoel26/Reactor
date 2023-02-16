using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Messaging;
using MauiReactor;
using Microsoft.Maui.Storage;
using Plugin.CloudFirestore;
using ReactorMaui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReactorMaui.Pages
{
    public class ConfiguracionState
    {
        public List<string> Casas { get; set; } = new List<string>();
        public string Correo { get; set; }
        public string Contra { get; set; } = string.Empty;
        public string NumSerie { get; set; } = string.Empty;
        public string CodigoVerificación { get; set; }
        public string CodigoCreado { get; set; }
        public bool BotonVerificar { get; set; } = false;
        public bool BotonBloqueado { get; set; } = false;
        public int? SelectedIndex { get; set; } = null;
    }

    public class Configuracion : Component<ConfiguracionState>
    {
        public MauiControls.Button BotonVerificar;
        public MauiControls.Button BotonBloqueado;
        public MauiControls.Button BotonEnviarVerificacion;
        public override VisualNode Render()
        {
            InicializarCasas();

            return new ContentPage("Configuracion")
            {
                new StackLayout()
                {
                    new Picker()
                        .ItemsSource(State.Casas)
                        .Title("Selecciona una casa")
                        .HCenter()
                        .VCenter()
                        .OnSelectedIndexChanged(selected => SetState(s => s.SelectedIndex = selected)),

                    new Entry()
                        .Placeholder("Correo electronico")
                        .HCenter()
                        .VCenter()
                        .OnTextChanged(text => SetState(s => s.Correo = text)),

                    new Button(btn => BotonEnviarVerificacion = btn)
                        .Text("Enviar codigo de verificación")
                        .HCenter()
                        .VCenter()
                        .OnClicked(() =>
                        {
                            BotonEnviarVerificacion.IsEnabled = false;
                            BotonVerificar.IsEnabled = true;
                            Correo.Enviar(State.Correo, "Codigo de verificación", CrearCodigo());      
                        }),

                    new Entry()
                        .Placeholder("Codigo de verificacón")
                        .HCenter()
                        .VCenter()
                        .OnTextChanged(text => SetState(s => s.CodigoVerificación = text)),

                    new Button(btn => BotonVerificar = btn)
                        .Text("Verificar codigo")
                        .HCenter()
                        .VCenter()
                        .IsEnabled(false)
                        .OnClicked(() =>
                        {      
                            BotonVerificar.IsEnabled = false;
                            VerificarCodigo();
                        }),

                    new Entry()
                        .Placeholder("Numero de serie")
                        .HCenter()
                        .VCenter()
                        .OnTextChanged(text => SetState(s => s.NumSerie = text)),

                    new Entry()
                        .Placeholder("Contraseña")
                        .HCenter()
                        .VCenter()
                        .OnTextChanged(text => SetState(s => s.Contra = text)),

                    new Button(btn => BotonBloqueado = btn)
                        .Text("Botton bloqueado")
                        .HCenter()
                        .VCenter()
                        .VCenter()
                        .IsEnabled(false)
                        .OnClicked(RegistrarAplicacion)
                }
            };
        }
        public void InicializarCasas()
        {
            for (int i = 1; i <= 100; i++)
            {
                SetState(s => s.Casas.Add($"Casa { i }"));
            }
        }

        public string CrearCodigo()
        {
            Random random = new Random();
            int numberRandom = random.Next(100000, 999999);
            string codigo = numberRandom.ToString();
            SetState(s => s.CodigoCreado = codigo);
            return codigo;
        }

        public void VerificarCodigo()
        {
            if(State.CodigoCreado == State.CodigoVerificación)
            {
                BotonBloqueado.IsEnabled = true;
            }else
            {
                Alerta.DesplegarAlerta("El codigo ingresado es incorrecto.");
                BotonVerificar.IsEnabled = true;
            }
        }

        public async void RegistrarAplicacion()
        {
            string IndexCasa = "";
            try
            {
                if (State.NumSerie == string.Empty || State.Contra == string.Empty || State.SelectedIndex == null)
                {
                    Alerta.DesplegarAlerta("Favor de llenar todos los datos solicitados");
                    return;
                }
                SetState(s => s.SelectedIndex++);

                if (State.SelectedIndex > 0 && State.SelectedIndex < 9)
                {
                    IndexCasa = "0" + State.SelectedIndex;
                }

                var documentContra = await CrossCloudFirestore
                                    .Current
                                    .Instance
                                    .Collection(State.NumSerie)
                                    .Document("Contra")
                                    .GetAsync();

                await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection(State.NumSerie)
                    .Document("Usuarios")
                    .Collection("Usuarios")
                    .Document(IndexCasa)
                    .Collection("Casa")
                    .AddAsync(new CorreoModel { Correo = State.Correo, Estatus = true });

                ContraModel contra = documentContra.ToObject<ContraModel>();

                if (contra.Contra != State.Contra.Trim())
                {
                    Alerta.DesplegarAlerta("Contraseña incorrecta");
                    return;
                }

                Alerta.DesplegarAlerta("Aplicación habilitada");

                Preferences.Set("NumSerie", State.NumSerie.Trim());
                Preferences.Set("Correo", State.Correo.Trim());
                Preferences.Set("NumCasa", IndexCasa);
                Preferences.Set("Contraseña", State.Contra.Trim());
                FirebaseMessaging.Instance.SubscribeToTopic(State.NumSerie);

            }
            catch
            {
                Alerta.DesplegarAlerta("Algo salio mal, favor de rectificar que la casa exista");
            }
        }    
    }  
}
