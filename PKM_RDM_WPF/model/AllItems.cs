using Newtonsoft.Json;
using PKM_RDM_WPF.engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace PKM_RDM_WPF.model
{
    public class AllItems
    {
        /*private readonly static string[] NO_ITEMS = new string[] {"master-ball", "ultra-ball", "great-ball", "poke-ball", "safari-ball",
        "net-ball", "dive-ball", "nest-ball", "repeat-ball", "timer-ball", "luxury-ball", "premier-ball", "dusk-ball", "heal-ball",
        "quick-ball", "cherish-ball", "potion", "antidote", "burn-heal", "ice-heal", "awakening", "paralyze-heal", "full-restore", "max-potion",
        };*/

        private readonly static string[] NO_ITEMS = new string[] {"exp-share", "cleanse-tag", "amulet-coin"};

        public const string CHEMIN_ALL_ITEMS = "data/allItems.json";
        private List<NameUrl> items;
        private List<Item> itemsGetted;

        public AllItems(){}

        public List<NameUrl> Items { get => items; set => items = value; }
        public List<Item> ItemsGetted { get => itemsGetted; set => itemsGetted = value; }

        public async Task RetrieveItems()
        {
            // Créer une liste de tâches asynchrones
            List<Task> tasks = new List<Task>();
            ItemsGetted = new List<Item>();

            // SET MOVES
            using (HttpClient client = new HttpClient())
            {
                foreach (NameUrl nameItem in items)
                {
                    // Ajouter chaque tâche à la liste
                    tasks.Add(MainPokemonCalc.GetItemByUrl(nameItem.Url).ContinueWith(task =>
                    {
                        if (task.Result != null)
                        {
                            ItemsGetted.Add(task.Result);
                        }
                    }));
                }

                // Attendre la fin de toutes les tâches
                await Task.WhenAll(tasks);
            }
        }

        public static async Task<AllItems> GetBattleItems() // todo : manque les baies
        {
            AllItems allItem = await MainPokemonCalc.GetAllItems();
            allItem.Items.RemoveRange(0, 69); // Suppression items de non-combat

            List<string> toSuppItem = new List<string>(NO_ITEMS);
            int nbItemSupp = allItem.Items.RemoveAll(item => toSuppItem.Any(s => string.Equals(s, item.Name, StringComparison.OrdinalIgnoreCase))); // Suppression items de la NO LIST - not working ?

            for (int i = allItem.Items.Count - 1; i >= 0; i--) // Suppression items INCENSE
            {
                NameUrl item = allItem.Items[i];
                string iName = item.Name.ToLower();
                if (iName.EndsWith("-incense"))
                {
                    allItem.Items.Remove(item);
                }
            }
            MessageBox.Show(allItem.Items.Count.ToString(), "eh beh");
            return allItem;
        }

        /*public async void RetrieveAndSaveItems()
        {
            await RetrieveItems();
            string jsonItems = JsonConvert.SerializeObject(items);
            File.WriteAllText(CHEMIN_ALL_ITEMS, jsonItems);
        }*/

        /*public List<Item> GetBattleItems(){
            List<Item> battleItems = new Ite;

            return battleItems;
        }*/
    }
}
