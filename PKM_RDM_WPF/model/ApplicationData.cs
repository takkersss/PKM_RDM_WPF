
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKM_RDM_WPF.model
{
    public class ApplicationData
    {
        public ObservableCollection<Pokemon> PokemonTeam { get; set; }
        public ObservableCollection<String> AllPokemonName { get; set; }

        public ObservableCollection<String> AllType { get; set; }
        public ObservableCollection<String> AllNature { get; set; }

        public ObservableCollection<String> AllPokemonNameFiltres { get; set; }

        public ObservableCollection<MoveVersion> MovesOfThePokemon { get; set; }

        public ApplicationData()
        {
            
        }
    }
}
