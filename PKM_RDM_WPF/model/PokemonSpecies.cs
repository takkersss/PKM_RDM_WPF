
using System;
using System.Collections.Generic;
using System.Linq;

namespace PKM_RDM_WPF.model
{
    public class PokemonSpecies
    {
        private string name;
        private List<NameLanguage> names;

        public PokemonSpecies() { }

        public PokemonSpecies(string nom, List<Types> types, List<NameLanguage> names)
        {
            this.Name = nom;
            this.Names = names;
        }

        public string Name { get => name; set => name = value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower(); }
        public List<NameLanguage> Names { get => names; set => names = value; }

    }
}