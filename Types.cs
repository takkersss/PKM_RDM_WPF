using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMON_CALCULATOR
{
    public class Types
    {
        private int slot;
        private Type type;

        public Types() { }

        public Types(int slot, Type type)
        {
            Slot = slot;
            Type = type;
        }

        public int Slot { get => slot; set => slot = value; }
        public Type Type { get => type; set => type = value; }
    }
}
