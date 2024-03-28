using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKM_RDM_WPF.model
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
    public class EffectEntryList
    {
        public List<EffectEntry> EffectEntries { get; set; }

        public string GetEnglishTextEffect()
        {
            if (EffectEntries == null) return null;
            EffectEntry effect_entry = EffectEntries.Find(x => x.Language.Name == "En");
            if (effect_entry == null)
            {
                return null;
            }
            else
            {
                return EffectEntries.Find(x => x.Language.Name == "En").Effect;
            }
        }
    }
}
