using Newtonsoft.Json;
using PokemonCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace POKEMON_CALCULATOR
{
    public class Pokemon
    {
        private string name;
        private int id;
        private Sprite sprites;
        private PokemonSpecies pSpecies;
        private List<Types> types;

        public Pokemon()
        {
        }

        public string Name { get => name; set => name = value; }
        public int Id { get => id; set => id = value; }
        public Sprite Sprites { get => sprites; set => sprites = value; }
        public PokemonSpecies PSpecies { get => pSpecies; set => pSpecies = value; }
        public List<Types> Types { get => types; set => types = value; }

        public async Task SetSpecies()
        {
            pSpecies = await GetPokemonSpeciesById(Id);
        }

        public string GetType(int type)
        {
            String leType;

            if (HasTwoTypes())
            {
                leType = this.Types[type - 1].Type.Name;
            }
            else leType = this.Types[0].Type.Name;

            return leType;
        }

        public bool HasTwoTypes()
        {
            return this.Types.Count == 2 ? true : false;
        }

        public string ToFrString()
        {
            string frName = PSpecies.Names.Find(x => x.Language.Name == "fr").Name;
            return frName;
        }

        public static async Task<PokemonSpecies> GetPokemonSpeciesById(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon-species/{id}");
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    PokemonSpecies especePokemon = JsonConvert.DeserializeObject<PokemonSpecies>(contenu);
                    return especePokemon;
                }
                else
                {
                    Console.WriteLine($"Erreur lors de la récupération du Pokémon avec l'ID {id}.");
                    return null;
                }
            }
        }
    }
}
