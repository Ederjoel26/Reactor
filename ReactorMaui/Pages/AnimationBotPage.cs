using MauiReactor;
using MauiReactor.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Pages
{
    public class AnimationBotPageState
    {
        public double Toggle { get; set; } = 0.0;
    }
    public class AnimationBotPage : Component<AnimationBotPageState>
    {
        public override VisualNode Render()
        {
            return new ContentPage("Animation bot dotnet")
            {
                new StackLayout()
                {
                    new Image("dotnet_bot.png")
                        .Rotation(State.Toggle),

                    new AnimationController()
                    {
                        new SequenceAnimation()
                        {
                            new DoubleAnimation()
                                .StartValue(-30)
                                .TargetValue(30)
                                .Duration(TimeSpan.FromSeconds(1))
                                .Easing(Easing.Linear)
                                .OnTick(v => SetState(s => s.Toggle = v))
                        }
                        .Loop(true)
                        .RepeatForever()
                    }
                    .IsEnabled(true)
                }
                .VCenter()
                .HCenter()
                .Margin(10)
            };
        }
    }
}
