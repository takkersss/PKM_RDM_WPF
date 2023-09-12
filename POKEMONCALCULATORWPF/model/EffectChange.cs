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
            return Effect_entries.Find(x => x.Language.Name == "En").Effect;
        }
    }
}
