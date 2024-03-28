using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKM_RDM_WPF.model
{
    public class Stat : IComparable
    {
        private int base_stat;
        private int effort;
        private NameUrl statUrl;

        public Stat()
        {}

        public int Base_stat { get => base_stat; set => base_stat = value; }
        public int Effort { get => effort; set => effort = value; }
        public NameUrl StatUrl { get => statUrl; set => statUrl = value; }

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1; // Si l'objet est nul, l'objet courant est considéré comme plus grand.

            // Assurez-vous que l'objet à comparer est de type Stat.
            if (obj is Stat otherStat)
            {
                // Compare les base_stat des deux objets.
                return otherStat.Base_stat.CompareTo(this.Base_stat);
            }
            else
            {
                throw new ArgumentException("L'objet à comparer n'est pas de type Stat.");
            }
        }
    }
}
