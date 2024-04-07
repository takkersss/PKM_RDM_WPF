
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKM_RDM_WPF.model;

namespace PKM_RDM_WPF.engine
{
    public class ApplicationData
    {
        public ObservableCollection<Pokemon> PokemonTeam { get; set; }
        public ObservableCollection<string> AllPokemonName { get; set; }

        public ObservableCollection<string> AllType { get; set; }
        public ObservableCollection<string> AllNature { get; set; }

        public ObservableCollection<string> AllPokemonNameFiltres { get; set; }

        public ObservableCollection<MoveVersion> MovesOfThePokemon { get; set; }

        public ApplicationData()
        {

        }
    }
}
