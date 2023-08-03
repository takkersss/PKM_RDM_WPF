using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMON_CALCULATOR
{
    public class Type
    {
        private string name, url;

        public Type() { }

        public Type(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }

        public string Name {
            get { return name; }
            set 
            {
                this.name = value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower();
            }
        }
        public string Url {
            get { return url; }
            set { url = value; }
        }
    }
}
