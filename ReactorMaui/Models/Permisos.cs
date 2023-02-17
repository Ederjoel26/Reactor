using Microsoft.Maui.Storage;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Models
{
    public static class Permisos
    {
        public async static Task<bool> VerificarRegistro()
        {
            try
            {
                var documentPrivada = await CrossCloudFirestore
                                        .Current
                                        .Instance
                                        .Collection(Preferences.Get("NumSerie", null))
                                        .Document("Estatus")
                                        .GetAsync();

                var documentContra = await CrossCloudFirestore
                                        .Current
                                        .Instance
                                        .Collection(Preferences.Get("NumSerie", null))
                                        .Document("Contra")
                                        .GetAsync();

                var documentCasa = await CrossCloudFirestore
                                        .Current
                                        .Instance
                                        .Collection(Preferences.Get("NumSerie", null))
                                        .Document("Usuarios")
                                        .Collection("Usuarios")
                                        .Document(Preferences.Get("NumCasa", null))
                                        .Collection("Casa")
                                        .GetAsync();

                EstatusModel estatusPrivada = documentPrivada.ToObject<EstatusModel>();
                ContraModel contra = documentContra.ToObject<ContraModel>();

                bool bandera = false;
                documentCasa.Documents.ToList().ForEach(document =>
                {
                    try
                    {
                        CorreoModel correo = document.ToObject<CorreoModel>();
                        if (correo.Correo.Trim() == Preferences.Get("Correo", null) && correo.Estatus)
                        {
                            bandera = true;
                        }
                    }
                    catch
                    {

                    }
                });

                if (!estatusPrivada.Estatus)
                {
                    Alerta.DesplegarAlerta("La privada no está habilitada.");
                    return false;
                }

                if (contra.Contra != Preferences.Get("Contraseña", null))
                {
                    Alerta.DesplegarAlerta("La contraseña es incorrecta.");
                    return false;
                }

                if (!bandera)
                {
                    Alerta.DesplegarAlerta("El correo no es valido o no está habilitado");
                    return false;
                }
            }
            catch
            {
                Alerta.DesplegarAlerta("No esta registrado, vaya al apartado de configuración.");
                return false;
            }

            return true;
        }
    }
}

