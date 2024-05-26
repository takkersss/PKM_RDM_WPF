using Newtonsoft.Json;
using PKM_RDM_WPF.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static PKM_RDM_WPF.utils.Utils;

namespace PKM_RDM_WPF.model
{
    public class MoveVersion
    {
        private NameUrl move;
        private List<MoveVersionDetails> version_group_details;

        // GET
        private Move moveGetted;

        public MoveVersion(){}

        public MoveVersion(NameUrl move, List<MoveVersionDetails> version_group_details)
        {
            this.Move = move;
            this.Version_group_details = version_group_details;
        }

        public NameUrl Move { get => move; set => move = value; }
        public List<MoveVersionDetails> Version_group_details { get => version_group_details; set => version_group_details = value; }
        public Move MoveGetted { get => moveGetted; set => moveGetted = value; }

        public async Task GetMoveAsync(HttpClient client)
        {
            HttpResponseMessage reponse = await client.GetAsync(this.Move.Url);
            if (reponse.IsSuccessStatusCode)
            {
                string contenu = await reponse.Content.ReadAsStringAsync();
                this.MoveGetted = JsonConvert.DeserializeObject<Move>(contenu);
            }
        }

        public override string? ToString()
        {
            return ToNiceString(this.Move.Name);
        }

        public string GetMoveName()
        {
            return ToNiceString(this.MoveGetted.Name);
        }

        // FILTERS
        public static List<MoveVersion> GetGoodPhysicalMoves(List<MoveVersion> allMoves)
        {
            allMoves = allMoves.FindAll(m => m.MoveGetted.isPhysical() && m.MoveGetted.isAGoodAttack());
            return allMoves;
        }

        public static List<MoveVersion> GetGoodSpecialMoves(List<MoveVersion> allMoves)
        {
            allMoves = allMoves.FindAll(m => m.MoveGetted.isSpecial() && m.MoveGetted.isAGoodAttack());
            return allMoves;
        }
    }
}
