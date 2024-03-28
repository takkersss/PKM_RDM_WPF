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
                this.name = value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower();
            }
        }
        public string Url { get => url; set => url = value; }
    }
}
