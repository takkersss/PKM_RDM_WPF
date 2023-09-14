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
        private EffectEntryList effectEntries;

        public Abilities()
        {
        }

        public NameUrl Ability { get => ability; set => ability = value; }
        public bool Is_hidden { get => is_hidden; set => is_hidden = value; }
        public int Slot { get => slot; set => slot = value; }
        public EffectChange Effect { get => effect; set => effect = value; }
        public EffectEntryList EffectEntries { get => effectEntries; set => effectEntries = value; }

        public override string? ToString()
        {
            return ability.Name;
        }

        public async Task GetEffectChange() 
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync(this.Ability.Url);
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    EffectChange effectC = JsonConvert.DeserializeObject<EffectChange>(contenu);
                    EffectEntryList effectEntries = JsonConvert.DeserializeObject<EffectEntryList>(contenu);
                    this.Effect = effectC;
                    this.EffectEntries = effectEntries;
                }
                else
                {
                    Console.WriteLine("problemo ability text requete");
                }
            }
        }

        public string GetDescription()
        {
            if (!String.IsNullOrWhiteSpace(Effect.GetEnglishTextEffect()))
            {
                return Effect.GetEnglishTextEffect();
            }else return EffectEntries.GetEnglishTextEffect();
        }
    }
}
