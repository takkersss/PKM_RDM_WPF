
using POKEMON_CALCULATOR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonCalculator
{
    public enum TypeP
    {
        Steel, Fighting, Dragon, Water, Electric, Fairy, Fire, Ice, Bug,
        Normal, Grass, Poison, Psychic, Rock, Ground, Ghost, Dark, Flying
    }

    public class Pokemon
    {
        private string name;
        private List<NameLanguage> names;
        private List<Types> types;

        public Pokemon() { }

        public Pokemon(string nom, List<Types> types, List<NameLanguage> names)
        {
            this.Name = nom;
            this.Types = types;
            this.Names = names;
        }

        public string Name { get => name; set => name = value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower(); }
        public List<Types> Types { get => types; set => types = value; }
        public List<NameLanguage> Names { get => names; set => names = value; }

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
            string frName = names.Find(x => x.Language.Name == "fr").Name;
            return frName;
        }
    }
}