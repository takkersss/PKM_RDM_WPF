using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMONCALCULATORWPF.model
{
    public class Abilities
    {
        private NameUrl ability;
        private bool is_hidden;
        private int slot;

        public Abilities()
        {
        }

        public NameUrl Ability { get => ability; set => ability = value; }
        public bool Is_hidden { get => is_hidden; set => is_hidden = value; }
        public int Slot { get => slot; set => slot = value; }

        public override string? ToString()
        {
            return ability.Name;
        }
    }
}
