using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PKM_RDM_WPF.model
{
    public class AllPokemon
    {
        public const string CHEMIN_ALL_POKEMON_NAME = "data/allPokemonName.json";
        [JsonProperty]
        private static int nB_Pokemon;
        private List<NameUrl> results;
        

        public AllPokemon()
        {
        }

        public static int NB_Pokemon { get => nB_Pokemon; set => nB_Pokemon = value; }
        public List<NameUrl> Results { get => results; set => results = value; }

        public List<string> GetAllPokemonName()
        {
            List<string> allPokemonName = new List<string>();
            foreach (NameUrl pokemon in Results)
            {
                allPokemonName.Add(pokemon.Name);
            }
            //throw new Exception(allPokemonName[1009]);
            return allPokemonName;
        }

    }
}
