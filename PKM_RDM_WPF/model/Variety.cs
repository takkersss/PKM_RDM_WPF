using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKM_RDM_WPF.model
{
    public class Variety
    {
        private bool is_default;
        private NameUrl pokemon;
        //private Pokemon pokemonGetted;

        public Variety()
        {
        }

        public bool Is_default { get => is_default; set => is_default = value; }
        public NameUrl Pokemon { get => pokemon; set => pokemon = value; }
        //public Pokemon PokemonGetted { get => pokemonGetted; set => pokemonGetted = value; }
    }
}
