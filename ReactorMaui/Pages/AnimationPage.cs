using MauiReactor;
using MauiReactor.Animations;
using System;

namespace ReactorMaui.Pages;

public class AnimationPageState
{
    public bool ToggleOne { get; set; }
    public bool ToggleTwo { get; set; }
    public bool ToggleThree { get; set; }
    public double ToggleFour { get; set; } = 0.0;
}

public class AnimationPage : Component<AnimationPageState>
{
    public override VisualNode Render()
    {
        return new ContentPage("Animations Page")
        {
            new Grid("50*, 50*", "50*, 50*")
            {
                new Frame()
                {
                    new Image("dotnet_bot.png")
                        .HCenter()
                        .VCenter()
                }
                .OnTapped(() => SetState(s => s.ToggleOne = !s.ToggleOne))
                .HasShadow(true)
                .Scale(State.ToggleOne ? 1.0 : 0.5)
                .Opacity(State.ToggleOne ? 1.0 : 0.0)
                .Margin(10)
                .GridColumn(0)
                .GridRow(0)
                .Rotation(State.ToggleOne ? 0.0 : 180.0)
                .BorderColor(Color.Parse("black"))
                .WithAnimation(),

                new Frame()
                {
                    new Image("dotnet_bot.png")
                        .HCenter()
                        .VCenter()
                }
                .OnTapped(() => SetState(s => s.ToggleTwo = !s.ToggleTwo))
                .HasShadow(true)
                .GridColumn(1)
                .GridRow(0)
                .Margin(10)
                .Scale(State.ToggleTwo ? 1.0 : 0.5)
                .Opacity(State.ToggleTwo ? 1.0 : 0.0)
                .BorderColor(Color.Parse("black"))
                .WithAnimation(),

                new Frame()
                {
                    new Image("dotnet_bot.png")
                        .HCenter()
                        .VCenter()
                }
                .OnTapped(() => SetState(s => s.ToggleThree = !s.ToggleThree))
                .HasShadow(true)
                .GridColumnSpan(2)
                .GridRow(1)
                .Margin(10)
                .Scale(State.ToggleThree ? 1.0 : 1.2)
                .BorderColor(Color.Parse("black"))
                .WithAnimation(),
            }
        };
    }
}