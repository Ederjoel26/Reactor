using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Models
{
    public class PushNotificationRequest
    {
        public string to { get; set; }
        public NotificationMessageBody notification { get; set; }
    }

    public class NotificationMessageBody
    {
        public string title { get; set; }
        public string body { get; set; }
    }
}
