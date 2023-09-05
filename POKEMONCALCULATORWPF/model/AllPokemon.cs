using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMONCALCULATORWPF.model
{
    public class AllPokemon
    {
        private static int nB;
        public const string CHEMIN_ALL_POKEMON_NAME = "data/allPokemonName.json";

        private List<NameUrl> results;
        

        public AllPokemon()
        {
        }

        public List<NameUrl> Results { get => results; set => results = value; }
        public static int NB { get => nB; set => nB = value; }

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
