using Newtonsoft.Json;
using POKEMON_CALCULATOR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static PokemonCalculator.Pokemon;

namespace PokemonCalculator
{
    class Program
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

        static async Task Main(string[] args)
        {
            string userEntry = "";
            string type1, type2, typeAtk, pkmName;
            do
            {
                Console.Clear();
                Console.WriteLine("1. Calculer multiplicateur");
                Console.WriteLine("=============================");
                /*Console.WriteLine("2. Obtenir faiblesses du type");
                Console.WriteLine("3. Obtenir resistances du type");
                Console.WriteLine("-----------------------------");*/
                Console.WriteLine("4. Obtenir attaques efficaces sur type");
                Console.WriteLine("5. Obtenir attaques peu efficace sur type");
                Console.WriteLine("=============================");
                Console.WriteLine("6. Obtenir les faiblesses du pokemon (par les 2 types)");
                Console.WriteLine("7. Obtenir les resistances du pokemon (par les 2 types)");
                Console.WriteLine("=============================");
                Console.WriteLine("8. Résumé d'un pokémon (grâce au nom)");
                Console.WriteLine("=============================");
                Console.WriteLine("9. Team Aléatoire");
                Console.WriteLine("=============================");
                Console.WriteLine("q. Quitter");
                Console.WriteLine("=============================");
                userEntry = Console.ReadLine();
                switch (userEntry)
                {
                    case "1":
                        Console.Clear();
                        AskPokemonTypes(out type1, out type2);
                        AskType(out typeAtk, false);
                        Console.WriteLine("Multiplicateur : " + CalculMultiplicator(type1, type2, typeAtk));
                        Console.ReadLine();
                        break;
                    /*
                    case "2":
                        Console.Clear();
                        AskType(out type1, true);
                        AfficheDictionnaire(GetFaiblessesAuType(type1));
                        Console.ReadLine();
                        break;
                    case "3":
                        Console.Clear();
                        AskType(out type1, true);
                        AfficheDictionnaire(GetResistancesAuType(type1));
                        Console.ReadLine();
                        break;
                    */
                    case "4":
                        Console.Clear();
                        AskType(out typeAtk, false);
                        AfficheListe(GetEfficaceSurType(typeAtk));
                        Console.ReadLine();
                        break;
                    case "5":
                        Console.Clear();
                        AskType(out typeAtk, false);
                        AfficheListe(GetPeuEfficaceSurType(typeAtk));
                        Console.ReadLine();
                        break;
                    case "6":
                        Console.Clear();
                        AskPokemonTypes(out type1, out type2);
                        AfficheDictionnaire(GetFaiblesses(type1, type2));
                        Console.ReadLine();
                        break;
                    case "7":
                        Console.Clear();
                        AskPokemonTypes(out type1, out type2);
                        AfficheDictionnaire(GetResistances(type1, type2));
                        Console.ReadLine();
                        break;
                    case "8":
                        Console.Clear();
                        Console.WriteLine("Nom du pokémon (en anglais) :");
                        pkmName = Console.ReadLine();
                        Console.Clear();
                        await GetPokemonByName(pkmName.ToLower());
                        ResumeUnPokemon(currentPokemon);
                        Console.ReadLine();
                        break;
                    case "9":
                        Console.Clear();
                        foreach (Pokemon p in await GetRandomPokemonTeam())
                        {
                            Console.WriteLine(p.ToFrString());
                        }
                        Console.ReadLine();
                        break;
                    case "q":
                        break;
                }

            } while (userEntry.ToLower() != "q");
        }

        public static double CalculMultiplicator(TypeP type1, TypeP type2, TypeP typeAtk)
        {
            return CalculMultiplicator(type1.ToString(), type2.ToString(), typeAtk.ToString());
        }

        public static double CalculMultiplicator(string type1, string type2, string typeAtk)
        {
            TypeP premierType = TexteToType(type1);
            TypeP deuxiemeType;
            TypeP atkType = TexteToType(typeAtk);

            if (!String.IsNullOrEmpty(type2))
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
            return (TypeP)Enum.Parse(typeof(TypeP), (type1.Substring(0, 1).ToUpper() + type1.Substring(1).ToLower()));
        }

        public static void AskPokemonTypes(out string type1, out string type2)
        {
            Console.WriteLine("1er type du pokemon :");
            type1 = Console.ReadLine();
            Console.WriteLine("2eme type du pokemon (enter si 1 seul type) :");
            type2 = Console.ReadLine();
        }

        public static void AskType(out string type, bool isPokemon)
        {
            if (isPokemon)
            {
                Console.WriteLine("Type du pokemon :");
            }
            else Console.WriteLine("Type de l'attaque :");
            type = Console.ReadLine();
        }

        public static Dictionary<TypeP, double> GetFaiblesses(TypeP type1, TypeP type2)
        {
            return GetFaiblesses(type1.ToString(), type2.ToString());
        }

        public static Dictionary<TypeP, double> GetFaiblesses(string type1, string type2)
        {
            if (String.IsNullOrEmpty(type2))
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

        public static Dictionary<TypeP, double> GetResistances(TypeP type1, TypeP type2)
        {
            return GetResistances(type1.ToString(), type2.ToString());
        }

        public static Dictionary<TypeP, double> GetResistances(string type1, string type2)
        {
            if (String.IsNullOrEmpty(type2))
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
                    faiblesses.Add(values[i], 2); ;
                }
            }

            return faiblesses;
        }

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

        public static void ResumeUnPokemon(Pokemon p)
        {
            Console.WriteLine("Nom : " + p.Name);
            Console.Write("Types : ");
            foreach (Types types in currentPokemon.Types)
            {
                if (types == currentPokemon.Types.Last() && currentPokemon.HasTwoTypes())
                {
                    Console.Write(" , " + types.Type.Name);
                }
                else Console.Write(types.Type.Name);
            }
            Console.WriteLine("\n==================================");
            Console.WriteLine("------- FAIBLESSES -------");
            if (currentPokemon.HasTwoTypes()) AfficheDictionnaire(GetFaiblesses(p.GetType(1), p.GetType(2)));
            else AfficheDictionnaire(GetFaiblessesAuType(p.GetType(1)));

            Console.WriteLine("\n------- RESISTANCES -------");
            if (currentPokemon.HasTwoTypes()) AfficheDictionnaire(GetResistances(p.GetType(1), p.GetType(2)));
            else AfficheDictionnaire(GetResistancesAuType(p.GetType(1)));

            Console.WriteLine();
        }

        /*
        public static List<string> ConvertPokemonListToFr(List<Pokemon> pokemons)
        {
            List<Pokemon> pokemonsTrad = new List<Pokemon>();

            foreach (Pokemon pok in pokemons)
            {

                pokemonsTrad.Add();
            }
        }*/

        public static async Task<List<Pokemon>> GetRandomPokemonTeam()
        {
            List<Pokemon> equipePokemonAleatoire = new List<Pokemon>();
            Random random = new Random();

            for (int i = 0; i < 6; i++)
            {
                equipePokemonAleatoire.Add(await GetRandomPokemon());
            }

            return equipePokemonAleatoire;
        }

        public static async Task<Pokemon> GetRandomPokemon()
        {
            Random random = new Random();
            int idPokemonAleatoire = random.Next(1, 1020); // Il y a actuellement 1020 Pokémon répertoriés dans l'API

            Pokemon pokemonAleatoire = await GetPokemonById(idPokemonAleatoire);
            if (pokemonAleatoire != null)
            {
                return pokemonAleatoire;
            }
            else return null;
        }

        public static async Task<Pokemon> GetPokemonById(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon-species/{id}");
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    Pokemon pokemon = JsonConvert.DeserializeObject<Pokemon>(contenu);
                    return pokemon;
                }
                else
                {
                    Console.WriteLine("Une erreur s'est produite lors de la récupération des informations du Pokémon.");
                    return null;
                }
            }
        }

        public static async Task<Pokemon> GetPokemonByName(string nom)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon/{nom}");
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    Pokemon pokemon = JsonConvert.DeserializeObject<Pokemon>(contenu);
                    return pokemon;
                }
                else
                {
                    Console.WriteLine("Une erreur s'est produite lors de la récupération des informations du Pokémon.");
                    return null;
                }
            }
        }

        public static async Task<string> GetPokemonNameById(int id, string? lang)
        {
            int langId;
            if (lang != "en")
            {
                langId = 3;
            }
            else langId = 5;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon-species/{id}");
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    dynamic especePokemon = JsonConvert.DeserializeObject(contenu);
                    return especePokemon.name;
                }
                else
                {
                    Console.WriteLine($"Erreur lors de la récupération du nom du Pokémon avec l'ID {id}.");
                    return null;
                }
            }
        }

        public static async Task<int> GetPokemonIdByName(string nom)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage reponse = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon/{nom}");
                if (reponse.IsSuccessStatusCode)
                {
                    string contenu = await reponse.Content.ReadAsStringAsync();
                    dynamic especePokemon = JsonConvert.DeserializeObject(contenu);
                    return especePokemon.id;
                }
                else
                {
                    Console.WriteLine($"Erreur lors de la récupération de l'ID du Pokémon avec le nom {nom}.");
                    return 0;
                }
            }
        }
    }
}


