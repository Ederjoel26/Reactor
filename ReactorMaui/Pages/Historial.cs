using Firebase.Components;
using MauiReactor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Google.Crypto.Tink.Signature;

namespace ReactorMaui.Pages
{

    public class HistorialState
    {
        public DateTime SelectedIndex { get; set; }
    }
    public class Historial: Component<HistorialState>
    {
        public override VisualNode Render()
        {
            return new ContentPage()
            {
                new StackLayout()
                {
                    new DatePicker()
                        .HCenter()
                        .VCenter()
                }
            };
        }
    }
}
