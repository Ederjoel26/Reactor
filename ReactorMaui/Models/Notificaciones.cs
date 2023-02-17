using Microsoft.Extensions.Primitives;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Models
{
    public static class Notificaciones
    {
        public static async void MandarNotificacion(string Title, string Mensaje)
        {
            PushNotificationRequest notificacion = new PushNotificationRequest()
            {
                to = $"/topics/{Preferences.Get("NumSerie", null)}",
                notification = new NotificationMessageBody()
                {
                    title = Title,
                    body = Mensaje
                }
            };

            string url = "https://fcm.googleapis.com/fcm/send";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("key", "=" + Constantes.Autorizacion);

            string serializarRequest = JsonConvert.SerializeObject(notificacion);
            var response = await client.PostAsync(url, new StringContent(serializarRequest, Encoding.UTF8, "application/json"));

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Alerta.DesplegarAlerta("Alerta enviada.");
            }
        }
    }
}
