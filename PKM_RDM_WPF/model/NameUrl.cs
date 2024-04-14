using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKM_RDM_WPF.model
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

        public string Name
        {
            get { return name; }
            set
            {
                this.name = value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower(); // ToNiceString(value)
            }
        }
        public string Url { get => url; set => url = value; }

        public int GetIdInUrl() // Must be / at the end
        {
            string[] segments = this.Url.Split('/');
            string lastSegment = segments[segments.Length - 2];

            // Convertit le dernier segment en entier
            if (int.TryParse(lastSegment, out int id))
            {
                return id;
            }
            else
            {
                // Si la conversion échoue, retourne -1 ou lance une exception selon vos besoins
                return -1; // Ou lancez une exception ici
            }
        }
    }
}
