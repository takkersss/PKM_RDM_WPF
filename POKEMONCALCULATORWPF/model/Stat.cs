using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POKEMONCALCULATORWPF.model
{
    public class Stat
    {
        private int base_stat;
        private int effort;
        private NameUrl statUrl;

        public Stat()
        {}

        public int Base_stat { get => base_stat; set => base_stat = value; }
        public int Effort { get => effort; set => effort = value; }
        public NameUrl StatUrl { get => statUrl; set => statUrl = value; }
    }
}
