using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKM_RDM_WPF.model
{
    public class Nature
    {
        public static readonly string[] NATURES = new string[]
{
    "Adamant (+Atk, -SpA)",
    "Bashful",
    "Bold (+Def, -Atk)",
    "Brave (+Atk, -Spe)",
    "Calm (+SpD, -Atk)",
    "Careful (+SpD, -SpA)",
    "Docile",
    "Gentle (+SpD, -Def)",
    "Hardy",
    "Hasty (+Spe, -Def)",
    "Impish (+Def, -SpA)",
    "Jolly (+Spe, -SpA)",
    "Lax (+Def, -SpD)",
    "Lonely (+Atk, -Def)",
    "Mild (+SpA, -Def)",
    "Modest (+SpA, -Atk)",
    "Naive (+Spe, -SpD)",
    "Naughty (+Atk, -SpD)",
    "Quiet (+SpA, -Spe)",
    "Quirky",
    "Rash (+SpA, -SpD)",
    "Relaxed (+Def, -Spe)",
    "Sassy (+SpD, -Spe)",
    "Serious",
    "Timid (+Spe, -Atk)"
};

        private string name;

        public Nature()
        {
        }
        public Nature(string name)
        {
            this.Name = name;
        }

        public string Name { get => name; set => name = value; }

        public override string? ToString()
        {
            return Name;
        }

        public string GetOnlyNatureName()
        {
            // Divisez la chaîne en utilisant '(' comme séparateur
            string[] parties = this.Name.Split('(');

            // La première partie (index 0) contient le nom de la nature
            string nomNature = parties[0].Trim();

            return nomNature;
        }
    }
}
