using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMONCALCULATORWPF.model
{
    public class EffectChange
    {
        private List<EffectEntry> effect_entries;
        private NameUrl version_group;

        public EffectChange()
        {
        }

        public List<EffectEntry> Effect_entries { get => effect_entries; set => effect_entries = value; }
        public NameUrl Version_group { get => version_group; set => version_group = value; }

        public override string? ToString()
        {
            return base.ToString();
        }

        public string GetEnglishTextEffect()
        {
            EffectEntry effect_entry = Effect_entries.Find(x => x.Language.Name == "En");
            if(effect_entry == null)
            {
                return null;
            }
            else
            {
                return Effect_entries.Find(x => x.Language.Name == "En").Effect;
            }
        }
    }
}
