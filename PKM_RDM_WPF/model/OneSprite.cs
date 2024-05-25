using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PKM_RDM_WPF.model
{
    public class OneSprite
    {
        [JsonPropertyName("default")]
        private string url;

        public OneSprite(){}

        public OneSprite(string spriteUrl) {
            this.Default = spriteUrl;
        }

        public string Default { get => url; set => url = value; }
    }
}
