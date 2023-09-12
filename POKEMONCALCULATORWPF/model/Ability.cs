using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace POKEMONCALCULATORWPF.model
{
    public class Abilities
    {
        private NameUrl ability;
        private bool is_hidden;
        private int slot;
        private EffectChange effect;

        public Abilities()
        {
        }

        public NameUrl Ability { get => ability; set => ability = value; }
        public bool Is_hidden { get => is_hidden; set => is_hidden = value; }
        public int Slot { get => slot; set => slot = value; }
        public EffectChange Effect { get => effect; set => effect = value; }

        public override string? ToString()
        {
            return ability.Name;
        }

        public async Task GetEffectChange() //url de pokemonspecies à mettre à la place
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync(this.Ability.Url);
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    EffectChange effectC = JsonConvert.DeserializeObject<EffectChange>(contenu);
                    this.Effect = effectC;
                }
                else
                {
                    Console.WriteLine("problemo ability text requete");
                }
            }
        }
    }
}
