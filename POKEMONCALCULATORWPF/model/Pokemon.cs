using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static POKEMONCALCULATORWPF.model.MainPokemonCalc;

namespace POKEMONCALCULATORWPF.model
{
    public class Pokemon
    {
        private string name;
        private int id;
        private Sprite sprites;
        private PokemonSpecies pSpecies;
        private List<Types> types;
        private Stat[] stats;

        public Pokemon()
        {
        }

        public string Name { get => name; set => name = value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower(); }
        public int Id { get => id; set => id = value; }
        public Sprite Sprites { get => sprites; set => sprites = value; }
        public PokemonSpecies PSpecies { get => pSpecies; set => pSpecies = value; }
        public List<Types> Types { get => types; set => types = value; }
        public Stat[] Stats { get => stats; set => stats = value; }


        // WPF
        private string frName, typeChartResume;
        public string FrName { get => frName; set => frName = value; }
        public string TypeChartResume { get => typeChartResume; set => typeChartResume = value; }


        public async Task SetSpecies()
        {
            pSpecies = await GetPokemonSpeciesById(Id);
        }

        public bool HasTwoTypes()
        {
            return this.Types.Count == 2 ? true : false;
        }

        private string ToFrString()
        {
            string frName = PSpecies.Names.Find(x => x.Language.Name == "Fr").Name;
            return frName;
        }

        public void SetFrName() { FrName = ToFrString();}

        // Méthode pour récupérer l'espèce du pokemon à partir de son id
        public static async Task<PokemonSpecies> GetPokemonSpeciesById(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon-species/{id}");
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    PokemonSpecies especePokemon = JsonConvert.DeserializeObject<PokemonSpecies>(contenu);
                    return especePokemon;
                }
                else
                {
                    Console.WriteLine($"Erreur lors de la récupération du Pokémon avec l'ID {id}.");
                    return null;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        // FAIBLESSES

        public Dictionary<TypeP, double> GetFaiblesses()
        {
            if (Types[1] == null)
            {
                return MainPokemonCalc.GetFaiblessesAuType(Types[0].Type.Name);
            }

            Dictionary<TypeP, double> faiblesses = new Dictionary<TypeP, double>();
            Dictionary<TypeP, double> faiblessesType1 = MainPokemonCalc.GetFaiblessesAuType(this.Types[0].Type.Name);
            Dictionary<TypeP, double> faiblessesType2 = MainPokemonCalc.GetFaiblessesAuType(this.Types[1].Type.Name);

            // faiblesses = faiblessesType1 + faiblessesType2
            faiblesses = faiblessesType1;
            faiblesses = MainPokemonCalc.AddDictionaryToDictionary(faiblesses, faiblessesType2, false);

            Dictionary<TypeP, double> resistancesType1 = MainPokemonCalc.GetResistancesAuType(this.Types[0].Type.Name);
            Dictionary<TypeP, double> resistancesType2 = MainPokemonCalc.GetResistancesAuType(this.Types[1].Type.Name);

            foreach (var faiblesse in faiblessesType1)
            {
                if (resistancesType2.ContainsKey(faiblesse.Key))
                {
                    faiblesses.Remove(faiblesse.Key);
                }
            }

            foreach (var faiblesse in faiblessesType2)
            {
                if (resistancesType1.ContainsKey(faiblesse.Key))
                {
                    faiblesses.Remove(faiblesse.Key);
                }
            }

            return faiblesses;
        }

        // RESISTANCES

        public Dictionary<TypeP, double> GetResistances()
        {
            if (Types[1] == null)
            {
                return MainPokemonCalc.GetResistancesAuType(Types[0].Type.Name);
            }

            Dictionary<TypeP, double> resistances = new Dictionary<TypeP, double>();
            Dictionary<TypeP, double> resType1 = MainPokemonCalc.GetResistancesAuType(this.Types[0].Type.Name);
            Dictionary<TypeP, double> resType2 = MainPokemonCalc.GetResistancesAuType(this.Types[1].Type.Name);
            Dictionary<TypeP, double> faiblesseType1 = MainPokemonCalc.GetFaiblessesAuType(this.Types[0].Type.Name);
            Dictionary<TypeP, double> faiblesseType2 = MainPokemonCalc.GetFaiblessesAuType(this.Types[1].Type.Name);

            // On ajoute automatiquement les immunités des 2 types
            foreach (KeyValuePair<TypeP, double> item in resType2)
            {
                if (item.Value == 0)
                {
                    resistances.Add(item.Key, 0);
                }
            }
            foreach (KeyValuePair<TypeP, double> item in resType1)
            {
                if (item.Value == 0)
                {
                    resistances.Add(item.Key, 0);
                }
            }

            // Ajouter les résistances du type1 qui ne sont pas faibles contre le type2
            foreach (KeyValuePair<TypeP, double> res in resType1)
            {
                if (!faiblesseType2.ContainsKey(res.Key))
                {
                    resistances[res.Key] = res.Value;
                }
            }

            // Ajouter les résistances du type2 qui ne sont pas faibles contre le type1
            foreach (KeyValuePair<TypeP, double> res in resType2)
            {
                if (!faiblesseType1.ContainsKey(res.Key))
                {
                    if (resistances.ContainsKey(res.Key))
                    {
                        // Si la résistance est déjà présente, on multiplie les multiplicateurs
                        resistances[res.Key] *= res.Value;
                    }
                    else
                    {
                        // Sinon, on ajoute la résistance
                        resistances[res.Key] = res.Value;
                    }
                }
            }

            return resistances;
        }

        public void ResumePokemon()
        {
            string txt = "";
            txt += "------- FAIBLESSES -------";
            if (HasTwoTypes()) txt += MainPokemonCalc.DictionnaireToString(GetFaiblesses());
            else txt += MainPokemonCalc.DictionnaireToString(MainPokemonCalc.GetFaiblessesAuType(this.Types[0].Type.Name));

            txt += "------- RESISTANCES -------";
            if (HasTwoTypes()) txt += MainPokemonCalc.DictionnaireToString(GetResistances());
            else txt += MainPokemonCalc.DictionnaireToString(MainPokemonCalc.GetResistancesAuType(this.Types[0].Type.Name));

            TypeChartResume = txt;
        }


    }
}
