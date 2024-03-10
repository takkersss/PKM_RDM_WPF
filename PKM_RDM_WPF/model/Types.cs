using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMONCALCULATORWPF.model
{
    public enum TypeP
    {
        Steel, Fighting, Dragon, Water, Electric, Fairy, Fire, Ice, Bug,
        Normal, Grass, Poison, Psychic, Rock, Ground, Ghost, Dark, Flying
    }

    public class Types
    {
        private int slot;
        private NameUrl type;

        public Types() { }

        public Types(int slot, NameUrl type)
        {
            Slot = slot;
            Type = type;
        }

        public int Slot { get => slot; set => slot = value; }
        public NameUrl Type { get => type; set => type = value; }   
    }
}
