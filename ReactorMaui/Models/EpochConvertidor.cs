using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Models
{
    public static class EpochConvertidor
    {
        public static int DateToEpoch(DateTime date)
        {
            TimeSpan time = date - new DateTime(1970, 1, 1);
            return (int)time.TotalSeconds;
        }

        public static string EpochToDate(int epochSeconds)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(epochSeconds);
            return dateTimeOffset.ToString();
        }

        public static string EpochToHour(int epochSeconds)
        {
            string fechaCompleta = EpochToDate(epochSeconds);
            string hora = string.Empty;
            for(int i = 0; i < fechaCompleta.Length; i++)
            {
                if (fechaCompleta[i] == ':')
                {
                    if (fechaCompleta[i - 2] != ' ')
                    {
                        for(int x = i - 2; x < fechaCompleta.Length; x++)
                        {
                            hora += fechaCompleta[x];
                            if (fechaCompleta[x] == 'M') return hora;
                        }
                    }
                    for(int x = i - 1; x < fechaCompleta.Length; x++)
                    {
                        hora += fechaCompleta[x];
                        if (fechaCompleta[x] == 'M') return hora;
                    }
                }
            }
            return hora;
        }
    }
}
