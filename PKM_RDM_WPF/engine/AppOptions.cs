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

        public AppOptions(){
            this.CurrentTeamName = "Team Name";
            this.moveSystemEnabledAtStart = false;
        }

        public AppOptions(string currTeamName)
        {
            this.CurrentTeamName = currTeamName;
        }

        public string CurrentTeamName { get => currentTeamName; set => currentTeamName = value; }
        public bool MoveSystemEnabledAtStart { get => moveSystemEnabledAtStart; set => moveSystemEnabledAtStart = value; }
    }
}
