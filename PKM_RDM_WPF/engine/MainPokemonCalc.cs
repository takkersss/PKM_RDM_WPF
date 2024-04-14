using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PKM_RDM_WPF.model;
using PKM_RDM_WPF.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static PKM_RDM_WPF.utils.Utils;

namespace PKM_RDM_WPF.engine
{
    public class MainPokemonCalc
    {
        public static Pokemon currentPokemon;

        public static double[,] table = new double[18, 18]
        {
            { 0.5, 2, 0.5, 1, 1, 0.5, 2, 0.5, 0.5, 0.5, 0.5, 0, 0.5, 0.5, 2, 1, 1, 0.5 }, // attaque sur Acier
            { 1, 1, 1, 1, 1, 2, 1, 1, 0.5, 1, 1, 1, 2, 0.5, 1, 1, 0.5, 2 }, // Combat
            { 1, 1, 2, 0.5, 0.5, 2, 0.5, 2, 1, 1, 0.5, 1, 1, 1, 1, 1, 1, 1 }, // Dragon
            { 0.5, 1, 1, 0.5, 2, 1, 0.5, 0.5, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1 }, // Eau
            { 0.5, 1, 1, 1, 0.5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 0.5 }, // Electrik
            { 2, 0.5, 0, 1, 1, 1, 1, 1, 0.5, 1, 1, 2, 1, 1, 1, 1, 0.5, 1 }, // Fee
            { 0.5, 1, 1, 2, 1, 0.5, 0.5, 0.5, 0.5, 1, 0.5, 1, 1, 2, 2, 1, 1, 1 }, // Feu
            { 2, 2, 1, 1, 1, 1, 2, 0.5, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1 }, // Glace
            { 1, 0.5, 1, 1, 1, 1, 2, 1, 1, 1, 0.5, 1, 1, 2, 0.5, 1, 1, 2 }, // Insecte
            { 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1 }, // Normal
            { 1, 1, 1, 0.5, 0.5, 1, 2, 2, 2, 1, 0.5, 2, 1, 1, 0.5, 1, 1, 2 }, // Plante
            { 1, 0.5, 1, 1, 1, 0.5, 1, 1, 0.5, 1, 0.5, 0.5, 2, 1, 2, 1, 1, 1 }, // Poison
            { 1, 0.5, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 0.5, 1, 1, 2, 2, 1 }, // Psy
            { 2, 2, 1, 2, 1, 1, 0.5, 1, 1, 0.5, 2, 0.5, 1, 1, 2, 1, 1, 0.5 }, // Roche
            { 1, 1, 1, 2, 0, 1, 1, 2, 1, 1, 2, 0.5, 1, 0.5, 1, 1, 1, 1 }, // Sol
            { 1, 0, 1, 1, 1, 1, 1, 1, 0.5, 0, 1, 0.5, 1, 1, 1, 2, 2, 1 }, // Spectre
            { 1, 2, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 0, 1, 1, 0.5, 0.5, 1 }, // Tenebres
            { 1, 0.5, 1, 1, 2, 1, 1, 2, 0.5, 1, 0.5, 1, 1, 2, 0, 1, 1, 1 } // Vol
        };

        public static double CalculMultiplicator(TypeP type1, TypeP type2, TypeP typeAtk)
        {
            return CalculMultiplicator(type1.ToString(), type2.ToString(), typeAtk.ToString());
        }

        public static double CalculMultiplicator(string type1, string type2, string typeAtk)
        {
            TypeP premierType = TexteToType(type1);
            TypeP deuxiemeType;
            TypeP atkType = TexteToType(typeAtk);

            if (!string.IsNullOrEmpty(type2))
            {
                deuxiemeType = TexteToType(type2);
            }
            else
            {
                return table[(int)premierType, (int)atkType];
            }
            return table[(int)premierType, (int)atkType] * table[(int)deuxiemeType, (int)atkType];
        }

        /*
        public static TypeP StringToType(string type)
        {
            return (TypeP)Enum.Parse(typeof(TypeP), type.ToLower());
        }
        */


        public static TypeP TexteToType(string type1)
        {
            try
            {
                return (TypeP)Enum.Parse(typeof(TypeP), type1);
                // Utilisez typeEnum ici
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(type1);
            }
        }

        // FAIBLESSES

        public static Dictionary<TypeP, double> GetFaiblesses(TypeP type1, TypeP type2)
        {
            return GetFaiblesses(type1.ToString(), type2.ToString());
        }

        public static Dictionary<TypeP, double> GetFaiblesses(string type1, string type2)
        {
            if (string.IsNullOrEmpty(type2))
            {
                return GetFaiblessesAuType(type1);
            }

            Dictionary<TypeP, double> faiblesses = new Dictionary<TypeP, double>();
            Dictionary<TypeP, double> faiblessesType1 = GetFaiblessesAuType(type1);
            Dictionary<TypeP, double> faiblessesType2 = GetFaiblessesAuType(type2);

            // faiblesses = faiblessesType1 + faiblessesType2
            faiblesses = faiblessesType1;
            faiblesses = AddDictionaryToDictionary(faiblesses, faiblessesType2, false);

            Dictionary<TypeP, double> resistancesType1 = GetResistancesAuType(type1);
            Dictionary<TypeP, double> resistancesType2 = GetResistancesAuType(type2);

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

        public static Dictionary<TypeP, double> GetResistances(TypeP type1, TypeP type2)
        {
            return GetResistances(type1.ToString(), type2.ToString());
        }

        public static Dictionary<TypeP, double> GetResistances(string type1, string type2)
        {
            if (string.IsNullOrEmpty(type2))
            {
                return GetResistancesAuType(type1);
            }

            Dictionary<TypeP, double> resistances = new Dictionary<TypeP, double>();
            Dictionary<TypeP, double> resType1 = GetResistancesAuType(type1);
            Dictionary<TypeP, double> resType2 = GetResistancesAuType(type2);
            Dictionary<TypeP, double> faiblesseType1 = GetFaiblessesAuType(type1);
            Dictionary<TypeP, double> faiblesseType2 = GetFaiblessesAuType(type2);

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

        // FAIBLESSES AU TYPE

        public static Dictionary<TypeP, double> GetFaiblessesAuType(TypeP type)
        {
            return GetFaiblessesAuType(type.ToString());
        }

        public static Dictionary<TypeP, double> GetFaiblessesAuType(string type)
        {
            Dictionary<TypeP, double> faiblesses = new Dictionary<TypeP, double>();
            double[] extractedRow = new double[table.GetLength(1)];
            for (int i = 0; i < table.GetLength(1); i++)
            {
                extractedRow[i] = table[(int)TexteToType(type), i];
            }

            TypeP[] values = (TypeP[])Enum.GetValues(typeof(TypeP)); // Tableau des valeurs de Type
            for (int i = 0; i < extractedRow.Length; i++)
            {
                double typeM = extractedRow[i];
                if (typeM > 1)
                {
                    faiblesses.Add(values[i], 2);
                }
            }

            return faiblesses;
        }

        // RESISTANCES AU TYPE

        public static Dictionary<TypeP, double> GetResistancesAuType(TypeP type)
        {
            return GetResistancesAuType(type.ToString());
        }

        public static Dictionary<TypeP, double> GetResistancesAuType(string type)
        {
            Dictionary<TypeP, double> resistances = new Dictionary<TypeP, double>();
            double[] extractedRow = new double[table.GetLength(1)]; // On crée un tableau de longueur du nb de type
            for (int i = 0; i < table.GetLength(1); i++)
            {
                extractedRow[i] = table[(int)TexteToType(type), i]; // On remplit le tableau par la ligne des resistances du type souhaité
            }

            TypeP[] values = (TypeP[])Enum.GetValues(typeof(TypeP)); // Ligne des Types
            for (int i = 0; i < extractedRow.Length; i++)
            {
                double typeM = extractedRow[i];

                if (typeM == 0)
                {
                    resistances.Add(values[i], 0);
                }
                else if (typeM < 1)
                {
                    resistances.Add(values[i], 0.5);
                }
            }

            return resistances;
        }

        // EFFICACITE SUR TYPE

        public static List<TypeP> GetEfficaceSurType(TypeP type)
        {
            return GetEfficaceSurType(type.ToString());
        }

        public static List<TypeP> GetEfficaceSurType(string type)
        {
            List<TypeP> faiblesses = new List<TypeP>();
            double[] extractedRow = new double[table.GetLength(0)];
            for (int i = 0; i < table.GetLength(0); i++)
            {
                extractedRow[i] = table[i, (int)TexteToType(type)];
            }

            TypeP[] values = (TypeP[])Enum.GetValues(typeof(TypeP)); // Tableau des valeurs de Type
            for (int i = 0; i < extractedRow.Length; i++)
            {
                double typeM = extractedRow[i];
                if (typeM > 1)
                {
                    faiblesses.Add(values[i]);
                }
            }

            return faiblesses;
        }

        // PEU EFFICACE SUR TYPE

        public static List<TypeP> GetPeuEfficaceSurType(TypeP type)
        {
            return GetPeuEfficaceSurType(type.ToString());
        }

        public static List<TypeP> GetPeuEfficaceSurType(string type)
        {
            List<TypeP> faiblesses = new List<TypeP>();
            double[] extractedRow = new double[table.GetLength(0)];
            for (int i = 0; i < table.GetLength(0); i++)
            {
                extractedRow[i] = table[i, (int)TexteToType(type)];
            }

            TypeP[] values = (TypeP[])Enum.GetValues(typeof(TypeP)); // Tableau des valeurs de Type
            for (int i = 0; i < extractedRow.Length; i++)
            {
                double typeM = extractedRow[i];
                if (typeM < 1)
                {
                    faiblesses.Add(values[i]);
                }
            }

            return faiblesses;
        }

        // AFFICHAGE DE LISTE/DICTIONNARY FORMAT CONSOLE

        public static void AfficheListe(List<TypeP> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TypeP type = list[i];
                string txt = type.ToString();
                Console.Write(txt);

                // Vérifier si c'est le dernier élément
                if (i < list.Count - 1)
                {
                    Console.Write(", ");
                }
            }
        }

        public static void AfficheDictionnaire(Dictionary<TypeP, double> d)
        {
            int i = 0;
            foreach (KeyValuePair<TypeP, double> paire in d)
            {
                Console.Write(paire.Key + " x" + paire.Value);
                // Vérifier si c'est le dernier élément
                if (i < d.Count - 1)
                {
                    Console.Write(", ");
                }
                i++;
            }
        }

        public static string DictionnaireToString(Dictionary<TypeP, double> d)
        {
            int i = 0;
            string txt = "";
            foreach (KeyValuePair<TypeP, double> paire in d)
            {
                txt += paire.Key + " x" + paire.Value;
                // Vérifier si c'est le dernier élément
                if (i < d.Count - 1)
                {
                    txt += ", ";
                }
                i++;
            }
            return txt;
        }


        public static Dictionary<TypeP, double> ListToDictionary(List<TypeP> list, bool isResistance)
        {
            double multiplicator = isResistance ? 0.5 : 2;
            Dictionary<TypeP, double> dictionary = new Dictionary<TypeP, double>();

            foreach (TypeP type in list)
            {
                if (dictionary.ContainsKey(type))
                {
                    dictionary[type] *= multiplicator;
                }
                else
                {
                    dictionary[type] = multiplicator;
                }
            }
            return dictionary;
        }

        public static Dictionary<TypeP, double> AddListToDictionary(List<TypeP> list, Dictionary<TypeP, double> dictionary, bool isResistance)
        {
            double multiplicator = isResistance ? 0.5 : 2;

            foreach (TypeP type in list)
            {
                if (dictionary.ContainsKey(type))
                {
                    dictionary[type] *= multiplicator;
                }
                else
                {
                    dictionary[type] = multiplicator;
                }
            }
            return dictionary;
        }

        public static Dictionary<TypeP, double> AddDictionaryToDictionary(Dictionary<TypeP, double> sourceDictionary, Dictionary<TypeP, double> targetDictionary, bool isResistance)
        {
            double multiplicator = isResistance ? 0.5 : 2;

            foreach (KeyValuePair<TypeP, double> kvp in sourceDictionary)
            {
                TypeP type = kvp.Key;

                if (targetDictionary.ContainsKey(type))
                {
                    targetDictionary[type] *= multiplicator;
                }
                else
                {
                    targetDictionary[type] = multiplicator;
                }
            }

            return targetDictionary;
        }

        public static bool TypeFaibleA(TypeP typeDefense, TypeP typeAttaque)
        {
            List<TypeP> list = DictionaryToTypeList(GetResistancesAuType(typeDefense));
            if (list.Contains(typeAttaque))
            {
                return true;
            }
            else return false;
        }

        public static List<TypeP> DictionaryToTypeList(Dictionary<TypeP, double> d)
        {
            List<TypeP> types = new List<TypeP>();
            foreach (KeyValuePair<TypeP, double> kvp in d)
            {
                TypeP type = kvp.Key;
                types.Add(type);
            }
            return types;
        }


        // REQUETES POKEAPI

        public static async Task<List<Pokemon>> GetRandomPokemonTeam()
        {
            List<Pokemon> equipePokemonAleatoire = new List<Pokemon>();

            while (equipePokemonAleatoire.Count < 6)
            {
                Pokemon p = await GetRandomPokemon();
                if (!equipePokemonAleatoire.Any(x => x.Id == p.Id)) // Eviter les doublons
                {
                    equipePokemonAleatoire.Add(p);
                }
            }

            return equipePokemonAleatoire;
        }

        public static async Task<Pokemon> GetRandomPokemon()
        {
            Random random = new Random();
            int idPokemonAleatoire = random.Next(1, AllPokemon.NB_Pokemon + 1); // Il y a actuellement 1020 Pokémon répertoriés dans l'API mais ~300 formes de +

            Pokemon pokemonAleatoire = await GetPokemonById(idPokemonAleatoire);
            if (pokemonAleatoire != null)
            {
                if(pokemonAleatoire.OtherForms.Count > 0)
                {
                    bool switchForm = random.Next(0, 100) < 40; // 40% de chance de changer la forme
                    if (switchForm)
                    {
                        int randomPok = random.Next(0, pokemonAleatoire.OtherForms.Count);
                        pokemonAleatoire = pokemonAleatoire.OtherForms[randomPok];
                    }
                }
                return pokemonAleatoire;
            }
            else return null;
        }

        public static async Task<Pokemon> GetPokemonById(int id)
        {
            return await GetPokemon($"https://pokeapi.co/api/v2/pokemon/{id}", $"error getting pokemon/{id}");
        }

        public static async Task<Pokemon> GetPokemonByName(string nom)
        {
            return await GetPokemon($"https://pokeapi.co/api/v2/pokemon/{nom.ToLower()}", $"error getting pokemon by name ({nom.ToLower()})");
        }

        public static async Task<Pokemon> GetPokemon(string url, string errorMessage)
        {
            if (!IsInternetConnected())
            {
                ShowConnexionError(errorMessage);
                return null;
            }

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Pokemon pokemon = JsonConvert.DeserializeObject<Pokemon>(content);
                    await pokemon.SetRetrievedData();
                    currentPokemon = pokemon;
                    return pokemon;
                }
                else
                {
                    ShowConnexionError(errorMessage);
                    return null;
                }
            }
        }


        public static async Task<string> GetPokemonNameById(int id, string? lang)
        {
            if (!IsInternetConnected()) { ShowConnexionError($"error getting name of pokemon/{id}"); return null; }
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon/{id}");
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    dynamic especePokemon = JsonConvert.DeserializeObject(contenu);
                    return especePokemon.name;
                }
                else
                {
                    ShowConnexionError($"error getting name of pokemon/{id}");
                    return null;
                }
            }
        }

        public static async Task<int> GetPokemonIdByName(string nom)
        {
            if (!IsInternetConnected()) { ShowConnexionError($"error getting id of {nom.ToLower()}"); return -1; }
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon/{nom.ToLower()}");
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    dynamic especePokemon = JsonConvert.DeserializeObject(contenu);
                    return especePokemon.id;
                }
                else
                {
                    ShowConnexionError($"error getting id of {nom.ToLower()}");
                    return -1;
                }
            }
        }

        public static async Task<AllPokemon> GetAllPokemonNameUrl()
        {
            if (!IsInternetConnected()) { ShowConnexionError("error getting name of all pokemon"); return null; }
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon?limit=20000");
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    AllPokemon allPokemon = JsonConvert.DeserializeObject<AllPokemon>(contenu);
                    return allPokemon;
                }
                else
                {
                    ShowConnexionError("error getting name of all pokemon");
                    return null;
                }
            }

        }

        public static async Task GetPokemonCount()
        {
            if (!IsInternetConnected()) { ShowConnexionError("error getting pokemon count"); return; }
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync("https://pokeapi.co/api/v2/pokemon-species"); // 1025
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(contenu);
                    int count = (int)json["count"];
                    AllPokemon.NB_Pokemon = count;
                }
                else
                {
                    ShowConnexionError("error getting pokemon count");
                }
            }
        }

        // Get the pokemon specie by it's id (in Pokemon)
        public static async Task<PokemonSpecies> GetPokemonSpeciesById(int id) //url de pokemonspecies à mettre à la place
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

        // ITEMS
        public static async Task<AllItems> GetAllItems()
        {
            if (!IsInternetConnected()) { ShowConnexionError("error getting items, connexion issue"); return null; }
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync("https://pokeapi.co/api/v2/item-attribute/5/");
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    AllItems allItems = JsonConvert.DeserializeObject<AllItems>(contenu);

                    // Fetch to the missings batlle items
                    List<NameUrl> missingsItems = await GetMissingBattleItems(client);
                    allItems.Items.AddRange(missingsItems);

                    // Fetch to the missings berries
                    List<NameUrl> missingsBerries = await GetBerries(client);
                    allItems.Items.AddRange(missingsBerries);

                    return allItems;
                }
                else
                {
                    ShowConnexionError("error getting all items");
                    return null;
                }
            }
        }

        public static async Task<List<NameUrl>> GetMissingBattleItems(HttpClient client)
        {
            HttpResponseMessage reponse = await client.GetAsync("https://pokeapi.co/api/v2/item-category/12/");
            if (reponse.IsSuccessStatusCode)
            {
                string contenu = await reponse.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(contenu);
                List<NameUrl> items = json["items"].ToObject<List<NameUrl>>();

                // We take only the items that missing in the previous fetch
                items.RemoveAll(item =>
                {
                    int id = item.GetIdInUrl();
                    return id < 580;
                });

                return items;
            }
            else
            {
                ShowConnexionError("error getting all items");
                return null;
            }
        }

        public static async Task<List<NameUrl>> GetBerries(HttpClient client)
        {
            HttpResponseMessage reponse = await client.GetAsync("https://pokeapi.co/api/v2/item-attribute/7/");
            if (reponse.IsSuccessStatusCode)
            {
                string contenu = await reponse.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(contenu);
                List<NameUrl> items = json["items"].ToObject<List<NameUrl>>();

                // We take only the items that missing in the previous fetch
                items.RemoveAll(item =>
                {
                    int id = item.GetIdInUrl();
                    return id > 189;
                });

                return items;
            }
            else
            {
                ShowConnexionError("error getting all items");
                return null;
            }
        }

        public static async Task<Item> GetItemByUrl(string url)
        {
            if (!IsInternetConnected()) { ShowConnexionError("error getting item, connexion issue"); return null; }
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync(url);
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    Item item = JsonConvert.DeserializeObject<Item>(contenu);
                    return item;
                }
                else
                {
                    ShowConnexionError("error getting item");
                    return null;
                }
            }
        }

        public static bool IsInternetConnected()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }

        public static string ToDynamicPath(string path)
        {
            return $"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName().Name};component/{path}";
        }
    }
}


