using MauiReactor;
using Newtonsoft.Json;
using ReactorMaui.Components;
using ReactorMaui.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReactorMaui.Pages
{
    public class RickAndMortyState
    {

    }
    public class RickAndMorty : Component<RickAndMortyState>
    {
        private List<Character> characters = new List<Character>(); 

        public override VisualNode Render()
        {
            FetchData();

            return new ContentPage("Rick and morty")
            {
                new ScrollView()
                {
                    //idk what should I do here
                }
            };
        }

        public VisualNode RenderCards()
        {
            foreach (Character character in characters)
            {
                return new Card(character)
                    .CreateCard();
            }
            return null;
        }
        public async void FetchData()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://rickandmortyapi.com/api/character");
            HttpContent content = response.Content;
            string json = await content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(json);
            foreach (var res in result.results)
            {
                Character character = new Character
                {
                    Name = res.name,
                    Gender = res.gender,
                    Image = res.image,
                    Species = res.species,
                    Status = res.status
                };
                characters.Add(character);
            }
        }
    }
}
