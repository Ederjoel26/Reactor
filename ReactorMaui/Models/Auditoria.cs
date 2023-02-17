using Microsoft.Maui.Storage;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Models
{
    public static class Auditoria
    {
        public static async void SubirAccionHistorial(string Accion)
        {
            int EpochToday = EpochConvertidor.DateToEpoch(DateTime.Now);
            try
            {
                HistorialModel historial = new HistorialModel
                {
                    Fecha = EpochToday,
                    NumeroCasa = Preferences.Get("NumCasa", null),
                    Observaciones = Accion
                };

                await CrossCloudFirestore
                   .Current
                   .Instance
                   .Collection(Preferences.Get("NumSerie", null))
                   .Document("Usuarios")
                   .Collection("Historial")
                   .AddAsync(historial);
            }
            catch
            {
                Alerta.DesplegarAlerta("Algo salio mal, intentelo más tarde.");
            }
            
        }
    }
}
