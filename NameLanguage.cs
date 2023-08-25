using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMON_CALCULATOR
{
    public class NameLanguage
    {
        private string name;
        private NameUrl language;

        public NameLanguage()
        {
        }

        public NameLanguage(string name, NameUrl language)
        {
            this.Name = name;
            this.Language = language;
        }

        public string Name { get => name; set => name = value; }
        public NameUrl Language { get => language; set => language = value; }
    }
}
