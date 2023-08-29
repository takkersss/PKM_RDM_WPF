
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMONCALCULATORWPF.model
{
    public class ApplicationData
    {
        public ObservableCollection<Pokemon> PokemonTeam { get; set; }
        public ObservableCollection<String> AllPokemonName { get; set; }

        public ObservableCollection<String> AllPokemonNameFiltres { get; set; }

        //public static List<Pokemon> pokemonTeam = new List<Pokemon>();

        public ApplicationData()
        {
            
        }
    }
}
