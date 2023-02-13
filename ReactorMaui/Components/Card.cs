using MauiReactor;
using ReactorMaui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorMaui.Components
{
    public class Card
    {
        Character character { get; set; }
        public Card(Character character)
        {
            this.character = character;
        }
        public VisualNode CreateCard()
        {
            return new Frame()
            {
                new Grid("50*, 50*", "*")
                {
                    new Frame()
                    {
                        new Image(character.Image)
                            .GridRow(0)
                            .GridColumn(0)
                            .HCenter()
                            .VCenter()
                    }
                    .BorderColor(Color.Parse("black"))
                    .HCenter()
                    .VCenter(),

                    new Grid("50*, 50*", "50*, 50*")
                    {
                        new Label(character.Name)
                            .GridRow(0)
                            .GridColumn(0)
                            .HCenter()
                            .VCenter(),

                        new Label(character.Species)
                            .GridRow(0)
                            .GridColumn(1)
                            .HCenter()
                            .VCenter(),

                        new Label(character.Gender)
                            .GridRow(1)
                            .GridColumn(0)
                            .HCenter()
                            .VCenter(),

                        new Label(character.Status)
                            .GridRow(1)
                            .GridColumn(1)
                            .HCenter()
                            .VCenter()
                    }
                }
            }
            .BorderColor(Color.Parse("black"));
        }
    }
}
