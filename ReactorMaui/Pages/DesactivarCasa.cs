using MauiReactor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Pages
{
    public class DesactivarCasaState
    {

    }
    public class DesactivarCasa : Component<DesactivarCasaState>
    {
        public override VisualNode Render()
        {
            return new ContentPage()
            {
                new Grid("5*, 90*, 5*", "10*, 80*, 10*")
                {
                    new Frame()
                    {

                    }
                    .GridRow(1)
                    .GridColumn(1)
                    .BorderColor(Color.Parse("black"))
                }
            };
        }
    }
}
