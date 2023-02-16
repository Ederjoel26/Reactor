﻿using CommunityToolkit.Maui.Alerts;
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
                new Grid("5*,90*,5*", "10*,80*,10")
                {
                    new Grid("50*,50*","*")
                    {
                        new Frame()
                        {
                            new Grid("5*,18*,18*,18*,18*,18*,5*","10*,80*,10*")
                            {
                                new Label()
                                    .Text("Validación del correo electronico")
                                    .HCenter()
                                    .VCenter()
                                    .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                    .GridRow(1)
                                    .GridColumn(1)
                                    .FontSize(15)
                                    .HorizontalTextAlignment(TextAlignment.Center),


                                new Entry()
                                    .Placeholder("Correo electronico")
                                    .HCenter()
                                    .VCenter()
                                    .OnTextChanged(text => SetState(s => s.Correo = text))
                                    .GridRow(2)
                                    .GridColumn(1),

                                new Button(btn => BotonEnviarVerificacion = btn)
                                    .Text("Enviar codigo de verificación")
                                    .HCenter()
                                    .VCenter()
                                    .OnClicked(() =>
                                    {
                                        BotonEnviarVerificacion.IsEnabled = false;
                                        BotonVerificar.IsEnabled = true;
                                        Correo.Enviar(State.Correo, "Codigo de verificación", CrearCodigo());
                                    })
                                    .GridRow(3)
                                    .GridColumn(1),

                                new Entry()
                                    .Placeholder("Codigo de verificacón")
                                    .HCenter()
                                    .VCenter()
                                    .OnTextChanged(text => SetState(s => s.CodigoVerificación = text))
                                    .GridRow(4)
                                    .GridColumn(1),

                                new Button(btn => BotonVerificar = btn)
                                    .Text("Verificar codigo")
                                    .HCenter()
                                    .VCenter()
                                    .IsEnabled(false)
                                    .OnClicked(() =>
                                    {
                                        BotonVerificar.IsEnabled = false;
                                        VerificarCodigo();
                                    })
                                    .GridRow(5)
                                    .GridColumn(1)
                            }
                            .RowSpacing(10)
                            .ColumnSpacing(10)
                        }
                        .GridColumn(0)
                        .GridRow(0)
                        .BorderColor(Color.Parse("black")),
                        
                        new Frame()
                        {
                            new Grid("5*,18*,18*,18*,18*,18*,5*","10*,80*,10*")
                            {
                                new Label()
                                    .Text("Validación de datos de la casa")
                                    .HCenter()
                                    .VCenter()
                                    .FontAttributes(Microsoft.Maui.Controls.FontAttributes.Bold)
                                    .GridRow(1)
                                    .GridColumn(1)
                                    .FontSize(15)
                                    .HorizontalTextAlignment(TextAlignment.Center),

                                new Picker()
                                    .ItemsSource(State.Casas)
                                    .Title("Selecciona una casa")
                                    .HCenter()
                                    .VCenter()
                                    .OnSelectedIndexChanged(selected => SetState(s => s.SelectedIndex = selected))
                                    .GridRow(2)
                                    .GridColumn(1),

                                new Entry()
                                    .Placeholder("Numero de serie")
                                    .HCenter()
                                    .VCenter()
                                    .OnTextChanged(text => SetState(s => s.NumSerie = text))
                                    .GridRow(3)
                                    .GridColumn(1),

                                new Entry()
                                    .Placeholder("Contraseña")
                                    .HCenter()
                                    .VCenter()
                                    .OnTextChanged(text => SetState(s => s.Contra = text))
                                    .GridRow(4)
                                    .GridColumn(1),

                                new Button(btn => BotonBloqueado = btn)
                                    .Text("Aplicación habilitada")
                                    .HCenter()
                                    .VCenter()
                                    .VCenter()
                                    .IsEnabled(false)
                                    .OnClicked(RegistrarAplicacion)
                                    .GridRow(5)
                                    .GridColumn(1),
                            }
                            .RowSpacing(10)
                            .ColumnSpacing(10)
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
                .HCenter()
                .VCenter()
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

                var documentCasa = await CrossCloudFirestore
                                         .Current
                                         .Instance
                                         .Collection(State.NumSerie)
                                         .Document("Usuarios")
                                         .Collection("Usuarios")
                                         .Document(IndexCasa)
                                         .Collection("Casa")
                                         .GetAsync();

                bool bandera = true;
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

                ContraModel contra = documentContra.ToObject<ContraModel>();

                if (contra.Contra != State.Contra.Trim())
                {
                    Alerta.DesplegarAlerta("Contraseña incorrecta");
                    return;
                }


                if(bandera == false)
                {
                    await CrossCloudFirestore
                                       .Current
                                       .Instance
                                       .Collection(State.NumSerie)
                                       .Document("Usuarios")
                                       .Collection("Usuarios")
                                       .Document(IndexCasa)
                                       .Collection("Casa")
                                       .AddAsync(new CorreoModel { Correo = State.Correo, Estatus = true });

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
