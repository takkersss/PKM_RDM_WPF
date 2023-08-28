using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMONCALCULATORWPF.model
{
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
