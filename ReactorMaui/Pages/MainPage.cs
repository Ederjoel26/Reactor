using MauiReactor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Pages
{
    public class MainPageState
    {
        public int Counter { get; set; }
    }

    public class MainPage : Component<MainPageState>
    {
        public override VisualNode Render()
        {
            return new NavigationPage()
            {
                new Shell
                {
                    new TabBar()
                    {
                        new Tab("Welcome to reactor")
                        {
                            new ContentPage("MAUI Reactor")
                            {
                                new VerticalStackLayout
                                {
                                    new Image("dotnet_bot.png")
                                        .HeightRequest(200)
                                        .HCenter()
                                        .Set(Microsoft.Maui.Controls.SemanticProperties.DescriptionProperty, "Cute dot net bot waving hi to you!"),

                                    new Label("Hello, World!")
                                        .FontSize(32)
                                        .HCenter(),

                                    new Label("Welcome to MauiReactor: MAUI with superpowers!")
                                        .FontSize(18)
                                        .HCenter(),

                                    new Button(State.Counter == 0 ? "Click me" : $"Clicked {State.Counter} times!")
                                        .OnClicked(()=>SetState(s => s.Counter ++))
                                        .HCenter(),

                                    new Button("Click me to remove 1 to the counter")
                                        .OnClicked(()=>SetState(s => s.Counter --))
                                        .HCenter()
                                        .VCenter(),

                                    new Button("Restart counter")
                                        .OnClicked(()=>SetState(s => s.Counter = 0))
                                        .HCenter()
                                        .VCenter(),

                                }
                                .VCenter()
                                .Spacing(25)
                                .Padding(30, 0)
                            }
                        },

                        new Tab("Animations")
                        {
                            new AnimationPage()
                        },

                        new Tab("Animation bot", "dotnet_bot.png")
                        {
                            new AnimationBotPage()
                        },

                        new Tab("Rick and Morty")
                        {
                            new RickAndMorty()
                        }
                    }
                }
                
            };
        }
    }
}