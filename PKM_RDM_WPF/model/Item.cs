using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PKM_RDM_WPF.utils.Utils;

namespace PKM_RDM_WPF.model
{
    public class Item
    {
        private List<EffectEntry> effect_entries;
        private string name;
        private OneSprite sprites;

        public Item(){}

        public Item(string name, string spritePath = null)
        {
            this.Name = name;
            this.Sprites = new OneSprite(spritePath);
        }

        public List<EffectEntry> Effect_entries { get => effect_entries; set => effect_entries = value; }
        public string Name { get => name; set => name = value; }
        public OneSprite Sprites { get => sprites; set => sprites = value; }

        public override string? ToString()
        {
            return ToNiceString(this.Name);
        }
    }
}
