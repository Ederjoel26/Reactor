using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReactorMaui.Models
{
    public static class Alerta
    {
        public static async void DesplegarAlerta(string texto)
        {
            ToastDuration duration = ToastDuration.Short;

            var toast = Toast.Make(texto, duration, 15);

            await toast.Show(new CancellationTokenSource().Token);
        }
    }
}
