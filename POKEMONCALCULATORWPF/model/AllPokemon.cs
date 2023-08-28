using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMONCALCULATORWPF.model
{
    public class AllPokemon
    {
        private List<NameUrl> results;

        public AllPokemon()
        {
        }

        public  List<NameUrl> Results { get => results; set => results = value; }

    }
}
