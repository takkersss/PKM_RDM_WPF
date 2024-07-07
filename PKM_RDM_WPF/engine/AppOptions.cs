using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKM_RDM_WPF.engine
{
    public class AppOptions
    {
        private string currentTeamName;
        private bool moveSystemEnabledAtStart;
        private bool randStrongPokemons;
        private bool randSmartMoves;

        public AppOptions(){
            this.CurrentTeamName = "Team Name";
            this.MoveSystemEnabledAtStart = false;
            this.RandStrongPokemons = false;
            this.RandSmartMoves = true;
        }

        public AppOptions(string currTeamName)
        {
            this.CurrentTeamName = currTeamName;
            this.MoveSystemEnabledAtStart = false;
            this.RandStrongPokemons = false;
            this.RandSmartMoves = true;
        }

        public string CurrentTeamName { get => currentTeamName; set => currentTeamName = value; }
        public bool MoveSystemEnabledAtStart { get => moveSystemEnabledAtStart; set => moveSystemEnabledAtStart = value; }
        public bool RandStrongPokemons { get => randStrongPokemons; set => randStrongPokemons = value; }
        public bool RandSmartMoves { get => randSmartMoves; set => randSmartMoves = value; }
    }
}
