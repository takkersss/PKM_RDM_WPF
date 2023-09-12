using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMONCALCULATORWPF.model
{
    public class EffectEntry
    {
        private string effect;
        private NameUrl language;

        public EffectEntry()
        {
        }

        public string Effect { get => effect; set => effect = value; }
        public NameUrl Language { get => language; set => language = value; }
    }
}
