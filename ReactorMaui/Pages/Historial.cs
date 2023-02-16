using MauiReactor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using Microsoft.Extensions.Logging.Abstractions;
using CommunityToolkit.Maui.Behaviors;

namespace ReactorMaui.Pages
{

    public class HistorialState
    {
        public DateTime Date { get; set; }
        public bool IsCalendarVisible { get; set; }
        public DatePicker date { get; set; }
    }
    public class Historial: Component<HistorialState>
    {
        public override VisualNode Render()
        {
            return new ContentPage()
            {
                new DatePicker()
                    
            };
        }   
    }
}
