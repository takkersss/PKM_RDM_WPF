using Newtonsoft.Json;
using PKM_RDM_WPF.engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using static PKM_RDM_WPF.engine.MainPokemonCalc;
using static PKM_RDM_WPF.utils.Utils;
using static System.Windows.Forms.AxHost;

namespace PKM_RDM_WPF.model
{
    public class Pokemon
    {
        public const string CHEMIN_DOSSIER = "teams";
        public static readonly string[] EVS_NAME = new string[] { "HP", "Atk", "Def", "SpA", "SpD", "Spe" };
        public const int MAX_EV_DISTRIBUTION = 508;

        private string name;
        private int id;
        private int order;
        private Sprite sprites;
        private PokemonSpecies pSpecies;
        private List<Types> types;
        private Stat[] stats;
        private List<Abilities> abilities;
        private List<MoveVersion> moves;

        public string Name { get => name; set => name = value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower(); }
        public int Id { get => id; set => id = value; }
        public Sprite Sprites { get => sprites; set => sprites = value; }
        public PokemonSpecies PSpecies { get => pSpecies; set => pSpecies = value; }
        public List<Types> Types { get => types; set => types = value; }
        public Stat[] Stats { get => stats; set => stats = value; }
        public int Order { get => order; set => order = value; }


        // WPF
        private List<Pokemon> otherForms;
        private string typeChartResume, wantedAbility, teamName;
        private int bst;
        private Nature wantedNature;
        private TypeP teraType;
        private int[] evs, ivs;
        private List<TypeP> resistancesX2, resistancesX4, faiblessesX2, faiblessesX4, immunites;
        private string[] fourMoves;
        private Item wantedItem;

        //public string FrName { get => frName; set => frName = value; }
        public string TypeChartResume { get => typeChartResume; set => typeChartResume = value; }
        public List<TypeP> ResistancesX2 { get => resistancesX2; set => resistancesX2 = value; }
        public List<TypeP> ResistancesX4 { get => resistancesX4; set => resistancesX4 = value; }
        public List<TypeP> FaiblessesX2 { get => faiblessesX2; set => faiblessesX2 = value; }
        public List<TypeP> FaiblessesX4 { get => faiblessesX4; set => faiblessesX4 = value; }
        public List<TypeP> Immunites { get => immunites; set => immunites = value; }
        public List<Abilities> Abilities { get => abilities; set => abilities = value; }
        public int Bst { get => bst; set => bst = value; }
        public string WantedAbility { get => wantedAbility; set => wantedAbility = value; }
        public TypeP TeraType { get => teraType; set => teraType = value; }
        public int[] Evs { get => evs; set => evs = value; }
        public int[] Ivs { get => ivs; set => ivs = value; }
        public Nature WantedNature { get => wantedNature; set => wantedNature = value; }
        public string TeamName { get => teamName; set => teamName = value; }
        public List<MoveVersion> Moves { get => moves; set => moves = value; }
        public string[] FourMoves {get => fourMoves; set{fourMoves = value;} }
        public List<Pokemon> OtherForms { get => otherForms; set => otherForms = value; }
        public Item WantedItem { get => wantedItem; set => wantedItem = value; }

        public Pokemon()
        {
            this.FourMoves = new string[4] {"", "", "", ""};
        }

        // Récupération pokemonSpecies et forms
        public async Task SetRetrievedData()
        {
            // SET SPECIE
            pSpecies = await GetPokemonSpeciesById(Id);

            // SET FORMS
            this.OtherForms = new List<Pokemon>();
            if (pSpecies != null && pSpecies.Varieties != null && pSpecies.Varieties.Count > 1)
            {
                foreach (Variety variety in pSpecies.Varieties)
                {
                    if (!variety.Is_default)
                    {
                        Pokemon form = await MainPokemonCalc.GetPokemon(variety.Pokemon.Url, "error getting pokemon form by url");
                        this.OtherForms.Add(form);
                    }
                }
            }
        }

        // Parser le pokémon en json et le sauvegarder en tant que fichier
        public void Serialize(string teamName)
        {
            this.TeamName = teamName;
            string json = JsonConvert.SerializeObject(this);
            string cheminFichier = $"{CHEMIN_DOSSIER}/{teamName}/{this.Name}.json";

            if (!Directory.Exists(CHEMIN_DOSSIER))
            {
                Directory.CreateDirectory(CHEMIN_DOSSIER);
            }
            File.WriteAllText(cheminFichier, json);
        }

        public override string ToString()
        {
            return Name;
        }

        public bool HasTwoTypes()
        {
            return this.Types.Count == 2 ? true : false;
        }

        private bool IsSpecialAttacker()
        {
            if (Stats[3].Base_stat - 25 > Stats[1].Base_stat)
            {
                return true;
            }
            else return false;
        }

        // Apply weaknesses, strengths, etc..
        public void SetDoubleTypesSpecifiations()
        {
            ResistancesX2 = new List<TypeP>();
            ResistancesX4 = new List<TypeP>();
            FaiblessesX2 = new List<TypeP>();
            FaiblessesX4 = new List<TypeP>();
            Immunites = new List<TypeP>();

            Dictionary<TypeP, double> resistances, faiblesses;

            if (HasTwoTypes())
            {
                resistances = MainPokemonCalc.GetResistances(Types[0].Type.Name, Types[1].Type.Name);
                faiblesses = MainPokemonCalc.GetFaiblesses(Types[0].Type.Name, Types[1].Type.Name);
            }
            else
            {
                resistances = MainPokemonCalc.GetResistances(Types[0].Type.Name, "");
                faiblesses = MainPokemonCalc.GetFaiblesses(Types[0].Type.Name, "");
            }

            foreach (KeyValuePair <TypeP,double> kvp in resistances)
            {
                if (kvp.Value == 0.5)
                {
                    ResistancesX2.Add(kvp.Key);
                }
                if (kvp.Value == 0.25)
                {
                    ResistancesX4.Add(kvp.Key);
                }
                if (kvp.Value == 0)
                {
                    Immunites.Add(kvp.Key);
                }
            }
            foreach (KeyValuePair<TypeP, double> kvp in faiblesses)
            {
                if (kvp.Value == 2)
                {
                    FaiblessesX2.Add(kvp.Key);
                }
                if (kvp.Value == 4)
                {
                    FaiblessesX4.Add(kvp.Key);
                }
            }
        }

        // Get the index of the best BASE STAT
        private int GetBestStatIndex()
        {
            int max = 0;
            int index = -1; // Initialisez l'index à une valeur invalide

            for (int i = 0; i < Stats.Length; i++)
            {
                if (Stats[i].Base_stat > max)
                {
                    max = Stats[i].Base_stat;
                    index = i; // Mettez à jour l'index de la valeur maximale
                }
            }

            return index;

        }

        // Get the index of the worst BASE STAT
        private int GetWorstStatIndex()
        {
            int min = 1000;
            int index = -1; // Initialisez l'index à une valeur invalide

            for (int i = 0; i < Stats.Length; i++)
            {
                if (Stats[i].Base_stat < min)
                {
                    min = Stats[i].Base_stat;
                    index = i; // Mettez à jour l'index de la valeur maximale
                }
            }

            return index;
        }

        // Sort the from the best BASE STAT to the worst
        private int[] GetDescendingStatsIndex()
        {
            int[] tableauBaseStats = new int[6];

            for (int i = 0; i < Stats.Length; i++)
            {
                tableauBaseStats[i] = Stats[i].Base_stat;
            }

            int[] indices = Enumerable.Range(0, tableauBaseStats.Length).ToArray();

            // Créez des paires (valeur, index)
            var paires = tableauBaseStats.Select((valeur, index) => new { Valeur = valeur, Index = index }).ToArray();

            // Triez les paires par valeur (du plus grand au plus petit)
            Array.Sort(paires, (a, b) => b.Valeur.CompareTo(a.Valeur));

            // Créez un tableau des indices triés
            int[] indicesTries = paires.Select(pair => pair.Index).ToArray();

            return indicesTries;
        }

        #region EVs & IVs
        // Application of Evs & IVs
        public void SetEvsAndIvs()
        {
            //FourMoves = new string[4] { "dfg", "dfg", "dfg", "dfg"};

            Evs = new int[6] { 0, 0, 0, 0, 0, 0 };
            Ivs = new int[6] { 31, 31, 31, 31, 31, 31 };

            int[] baseStats = GetDescendingStatsIndex();
            Evs[baseStats[0]] = 252; // On met les 2 stats les plus fortes à 252
            Evs[baseStats[1]] = 252;
            DistributeRemainingEVs();

            if (IsSpecialAttacker())
            {
                Ivs[1] = 0;
            }

        }

        // Distribute the remaining EVs
        public void DistributeRemainingEVs()
        {
            int[] baseStats = GetDescendingStatsIndex();
            List<int> eligibleStats = new List<int>();

            // Remplissez eligibleStats avec les indices des statistiques non à 252 et non les deux statistiques les plus élevées
            for (int i = 0; i < Evs.Length; i++)
            {
                if (Evs[i] != 252 && i != baseStats[0] && i != baseStats[1])
                {
                    eligibleStats.Add(i);
                }
            }

            if (eligibleStats.Contains(5)) // Vérifiez la statistique de vitesse
            {
                if (Evs[GetWorstStatIndex()] != eligibleStats.IndexOf(5))
                {
                    Evs[eligibleStats[eligibleStats.IndexOf(5)]] += 4; // Ajoutez 4 EVs à la statistique de vitesse
                }
            }
            else if (eligibleStats.Contains(4)) // Vérifiez la statistique spedef
            {
                if (Evs[GetWorstStatIndex()] != eligibleStats.IndexOf(4))
                {
                    Evs[eligibleStats[eligibleStats.IndexOf(4)]] += 4; // Ajoutez 4 EVs à la statistique de vitesse
                }
            }
            else if (eligibleStats.Count > 0) // Si aucune des deux options précédentes n'est possible, choisissez une statistique aléatoire
            {
                Random random = new Random();
                int randomStatIndex = random.Next(0, eligibleStats.Count);
                Evs[eligibleStats[randomStatIndex]] += 4; // Ajoutez 4 EVs à une statistique aléatoire
            }
        }

        // Get the remaining EVs that can be distributed
        public int GetRemainingEvs()
        {
            int distribuedEvs = 0;
            foreach (int i in Evs)
            {
                distribuedEvs += i;
            }
            return MAX_EV_DISTRIBUTION - distribuedEvs;
        }

        public int GetTotalEvs()
        {
            int totalEvs = 0;
            foreach (int ev in Evs)
            {
                totalEvs += ev;
            }
            return totalEvs;
        }

        #endregion EVs & IVs

        #region SHOWDOWN FORMAT EXPORT
        // SHOWDOWN FORMAT EXPORT
        public string GetEvsTextForShowdown()
        {
            string txt = "";
            for (int i = 0; i < Evs.Length; i++)
            {
                if (Evs[i] == 0) continue;
                txt += Evs[i] + " " + EVS_NAME[i];
                if(i != Evs.Length-1)
                {
                    txt += " / ";
                }
            }
            return txt;
        }

        public string GetIvsTextForShowdown()
        {
            string txt = "";
            int nbIv31 = 0;

            foreach (int iv in Ivs)
            {
                if (iv == 31)
                {
                    nbIv31 += 1;
                }
            }

            for (int i = 0; i < Ivs.Length; i++)
            {
                if (Ivs[i] == 31)
                {
                    continue; // Ignore les IVs égaux à 31
                }

                txt += Ivs[i] + " " + EVS_NAME[i];

                if (i != Ivs.Length - 1 && Ivs[i + 1] != 31)
                {
                    txt += " / ";
                }
            }

            if (nbIv31 != 6)
            {
                txt = "IVs: " + txt;
            }

            return txt;
        }
        #endregion SHOWDOWN FORMAT EXPORT

        // NATURE
        public void ChooseBestNature()
        {
            string bestStat, worstStat;
            Random r = new Random();

            if(GetBestStatIndex() != 0 && GetWorstStatIndex() != 5) // Si le pokemon est offensif
            {
                bestStat = EVS_NAME[5]; // On augmente la vitesse
                if (IsSpecialAttacker())
                {
                    worstStat = EVS_NAME[1];
                }
                else worstStat = EVS_NAME[3];
            }
            else if(GetBestStatIndex() == 0 || GetBestStatIndex() == 2 || GetBestStatIndex() == 4) // Si pokemon defensif on augmente defense ou spedef
            {
                if (Stats[2].Base_stat > Stats[4].Base_stat)
                {
                    bestStat = EVS_NAME[2];
                }else bestStat = EVS_NAME[4];
                worstStat = EVS_NAME[5];
            }
            else if(GetBestStatIndex() != 0 && GetWorstStatIndex() != 0)
            {
                bestStat = EVS_NAME[GetBestStatIndex()];
                worstStat = EVS_NAME[GetWorstStatIndex()];
            }
            else
            {
                WantedNature = new Nature(Nature.NATURES[1]);
                return;
            }

            string resume = $"(+{bestStat}, -{worstStat})";
            WantedNature = new Nature(Nature.NATURES.ToList().Find(x => x.Contains(resume)));
        }

        // ABILITY
        public int GetIndexOfWantedAbility()
        {
            int index = this.Abilities.IndexOf(this.Abilities.Find(x => x.Ability.Name == WantedAbility));
            return index;
        }

        // MOVEPOOL
        public async Task RetrievePossibleMoves()
        {
            // Créer une liste de tâches asynchrones
            List<Task> tasks = new List<Task>();

            // SET MOVES
            using (HttpClient client = new HttpClient())
            {
                foreach (MoveVersion mv in Moves)
                {
                    // Ajouter chaque tâche à la liste
                    tasks.Add(mv.GetMoveAsync(client));
                }

                // Attendre la fin de toutes les tâches
                await Task.WhenAll(tasks);
            }

            //MessageBox.Show("Moovpool retrieved");
        }

        public void RandomizeFourMoves()
        {
            int movesNb = this.Moves.Count;

            Random random = new Random();

            int appliedMoves = 0;
            while (appliedMoves != 4)
            {
                int randomIndex = random.Next(0, movesNb);
                MoveVersion selectedMove = this.Moves[randomIndex];

                // Vérifiez si le mouvement sélectionné n'est pas déjà présent dans les mouvements du Pokémon
                if (!this.FourMoves.Contains(ToNiceString(selectedMove.MoveGetted.Name)))
                {
                    this.FourMoves[appliedMoves] = ToNiceString(selectedMove.MoveGetted.Name);
                    appliedMoves++;
                }
            }
        }

        // ITEM
        public void ChooseBestItemButRandom(List<Item> items)
        {
            Random r = new Random();
            int applyThisItem = r.Next(100); // Générer un nombre aléatoire entre 0 et 99

            if(this.Bst <= 420)
            {
                if(applyThisItem < 25) // 25% chance of getting eviolite if base stat under 421
                {
                    this.WantedItem = items.First(x => x.Name.ToLower().Contains("eviolite"));
                    return;
                }
            }

            List<string> itemsList;
            bool result;

            // HP
            itemsList = new List<string> { "sitrus-berry", "leftovers", "aguav-berry" };
            if (ApplyItemWithChanceStat(0, r, 40, itemsList, items)) return;

            // Atk
            itemsList = new List<string> { "choice-band", "life-orb", "choice-scarf" };
            if (ApplyItemWithChanceStat(1, r, 40, itemsList, items)) return;

            // Def
            itemsList = new List<string> { "rocky-helmet", "sitrus-berry", "covert-cloak", "lum-berry" };
            if (ApplyItemWithChanceStat(2, r, 40, itemsList, items)) return;

            // Spe Atk
            itemsList = new List<string> { "choice-specs", "life-orb", "choice-scarf" };
            if (ApplyItemWithChanceStat(3, r, 40, itemsList, items)) return;

            // Spe Def
            itemsList = new List<string> { "assault-vest", "sitrus-berry", "covert-cloak", "lum-berry" };
            if (ApplyItemWithChanceStat(4, r, 40, itemsList, items)) return;

            // Speed
            itemsList = new List<string> { "metronome" };
            itemsList.Add(IsSpecialAttacker() ? "choice-specs" : "choice-band");
            if (ApplyItemWithChanceStat(5, r, 40, itemsList, items)) return;

            // Sinon full aléatoire
            this.WantedItem = items[r.Next(items.Count)];
        }

        private bool ApplyItemWithChanceStat(int iStat, Random r, int applyItemChance, List<string> itemsName, List<Item> items)
        {
            if (GetBestStatIndex() == iStat)
            {
                string itemName = ApplyItem(r, applyItemChance, itemsName);
                if (!String.IsNullOrEmpty(itemName))
                {
                    this.WantedItem = items.First(x => x.Name.ToLower().Contains(itemName));
                    return true;
                }
            }
            return false;
        }

        private string ApplyItem(Random r, int applyItemChance, List<string> itemsName)
        {
            int applyThisItem = r.Next(100);

            if (applyThisItem < applyItemChance)
            {
                string itemName;
                int itemChance = r.Next(itemsName.Count);

                itemName = itemsName[itemChance]; // item aléatoire entre tous

                ///MessageBox.Show(itemName);
                return itemName;
            }
            return null;
        }
    }
}
