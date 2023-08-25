using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMON_CALCULATOR
{
    public class NameUrl
    {
        private string name, url;

        public NameUrl()
        {}

        public NameUrl(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }

        public string Name { get => name; set => name = value; }
        public string Url { get => url; set => url = value; }
    }
}
