using Newtonsoft.Json;
using POKEMONCALCULATORWPF.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POKEMONCALCULATORWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isBtnRandomTeamBusy;
        WindowSwitchPokemon winSwitch;
        private Pokemon currentPokemon;

        public MainWindow()
        {
            InitializeComponent();
            Show();
            winSwitch = new WindowSwitchPokemon(this, applicationData);
        }

        private async void RandomTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            await NewRandomTeam();
        }

        private void CopyTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isBtnRandomTeamBusy) return;
            if (!string.IsNullOrEmpty(GetPokemonsNamesTeam()))
            {
                Clipboard.SetText(GetPokemonsNamesTeam()); // Copie le texte dans le presse-papiers
            }
            else
            {
                MessageBox.Show("Le champ de texte est vide. Rien à copier.");
            }
        }

        private string GetPokemonsNamesTeam()
        {
            string team ="";

            Random random = new Random();
            Array valeursEnum = Enum.GetValues(typeof(TypeP));


            foreach (Pokemon p in applicationData.PokemonTeam)
            {
                team +=p.Name + "\nAbility: " + p.WantedAbility;
                team += "\nTera Type: " + p.TeraType.ToString();
                team += "\nEVs: " + p.GetEvsTextForShowdown();

                if (!String.IsNullOrEmpty(p.GetIvsTextForShowdown()))
                {
                    team += "\n" + p.GetIvsTextForShowdown();
                }
                team += "\n\n";
            }
            return team;
        }

        private async Task NewRandomTeam() {

            if (isBtnRandomTeamBusy) return;

            isBtnRandomTeamBusy = true;
            RandomTeamBtn.Background = Brushes.Red;
            applicationData.PokemonTeam = new ObservableCollection<Pokemon>(await MainPokemonCalc.GetRandomPokemonTeam());
            ReSetWindowAndTeam(0);
            RandomTeamBtn.Background = Brushes.Gray;
            isBtnRandomTeamBusy = false;
        }

        private void LoadProperties()
        {
            foreach (Pokemon p in applicationData.PokemonTeam)
            {
                p.SetFrName();
                p.SetDoubleTypesSpecifiations();
                if (p.Abilities.Count > 1)
                {
                    if (p.Abilities.First().Ability.Name == p.Abilities.Last().Ability.Name)
                    {
                        p.Abilities.Remove(p.Abilities.Last());
                    }
                }
                p.Bst = 0;
                foreach (Stat stat in p.Stats)
                {
                    p.Bst += stat.Base_stat;
                }
                p.SetEvsAndIvs();
                Random r = new Random();
                p.WantedAbility = p.Abilities[r.Next(0, p.Abilities.Count)].Ability.Name;
                p.TeraType = (TypeP)Enum.Parse(typeof(TypeP), applicationData.AllType[r.Next(0,18)]);
            }
        }

        // Se lance au chargement de la fenetre (listview)
        private async void teamListBox_Loaded(object sender, RoutedEventArgs e)
        {
            applicationData.AllPokemonName = new ObservableCollection<String>(await MainPokemonCalc.GetAllPokemonName());
            applicationData.AllType = new ObservableCollection<string>(Enum.GetNames(typeof(TypeP)));
            if (Directory.Exists(Pokemon.CHEMIN_DOSSIER))
            {
                string[] fichiersDansDossier = Directory.GetFiles(Pokemon.CHEMIN_DOSSIER);

                if (fichiersDansDossier.Length == 6)
                {
                    List<Pokemon> pokemons = new List<Pokemon>();
                    foreach (string fichier in fichiersDansDossier)
                    {
                        string jsonContenu = File.ReadAllText(fichier);

                        // Désérialiser le contenu JSON en un objet
                        Pokemon p = JsonConvert.DeserializeObject<Pokemon>(jsonContenu);
                        pokemons.Add(p);
                    }
                    applicationData.PokemonTeam = new ObservableCollection<Pokemon>(pokemons);
                    RefreshWindow();
                }
                else
                {
                    NewRandomTeam();
                }
                currentPokemon = (Pokemon)teamListImageView.SelectedItem;
                //MessageBox.Show(currentPokemon.Name);
            }
            else
            {
                throw new Exception("directory doesn't exist");
            }
        }

        public void ReSetWindowAndTeam(int index)
        {
            LoadProperties();
            RefreshWindow();
        }

        public void RefreshWindow()
        {
            teamListImageView.ItemsSource = applicationData.PokemonTeam; // actualisation
            cbTera.ItemsSource = applicationData.AllType; // actualisation
            teamListImageView.SelectedIndex = -1;
            teamListImageView.SelectedIndex = 0; // actualisation;
        }

        private void lvOpenSwitchWindow(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                object dC = ((ListViewItem)sender).DataContext;
                winSwitch.selectedPokemon = ((Pokemon)dC);
                winSwitch.ShowDialog();
            }
        }

        private void SaveTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isBtnRandomTeamBusy) return;
            string[] fichiersDansDossier = Directory.GetFiles(Pokemon.CHEMIN_DOSSIER);
            foreach (string item in fichiersDansDossier) // On supprime les anciens fichiers
            {
                File.Delete(item);
            }
            foreach (Pokemon p in applicationData.PokemonTeam) // On sauvegarde les nouveaux
            {
                p.Serialize();
            }
        }

        private void teamListImageView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = ((ListView)sender);
            if (lv.SelectedItem == null) return;

            currentPokemon = applicationData.PokemonTeam[applicationData.PokemonTeam.ToList().FindIndex(x => x.Name == ((Pokemon)lv.SelectedItem).Name)];
            cbAbility.SelectedIndex = currentPokemon.GetIndexOfWantedAbility();
            cbTera.SelectedIndex = GetIndexOfWantedTera(currentPokemon);
            UpdateShowedEVs();
            //MessageBox.Show(obj.ToString());
        }

        private void cbAbility_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentPokemon == null) return;
            if(cbAbility.SelectedIndex == -1) return;
            currentPokemon.WantedAbility = currentPokemon.Abilities[cbAbility.SelectedIndex].Ability.Name;
            //cbAbility.SelectedIndex = currentPokemon.GetIndexOfWantedAbility();
        }

        private void cbTera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentPokemon == null) return;
            if (cbTera.SelectedIndex == -1) return;
            currentPokemon.TeraType = (TypeP)Enum.Parse(typeof(TypeP), applicationData.AllType[cbTera.SelectedIndex]);
        }

        private int GetIndexOfWantedTera(Pokemon p)
        {
            int index = applicationData.AllType.IndexOf(applicationData.AllType.ToList().Find(x => x == p.TeraType.ToString()));
            return index;
        }

        private void UpdateEvAndSlider(int index, int value, Slider slider)
        {
            slider.Value = currentPokemon.Evs[index];
        }
        private void tbEv_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (String.IsNullOrEmpty(tb.Text)) return;

            if (currentPokemon != null && currentPokemon.Evs != null)
            {
                int ev;
                if (int.TryParse(tb.Text, out ev))
                {

                    switch (tb.Name)
                    {
                        case "tbHpEv":
                            UpdateEvAndSlider(0, ev, slHpEv);
                            break;
                        case "tbAttEv":
                            UpdateEvAndSlider(1, ev, slAttEv);
                            break;
                        case "tbDefEv":
                            UpdateEvAndSlider(2, ev, slDefEv);
                            break;
                        case "tbSAttEv":
                            UpdateEvAndSlider(3, ev, slSAttEv);
                            break;
                        case "tbSDefEv":
                            UpdateEvAndSlider(4, ev, slSDefEv);
                            break;
                        case "tbSpeEv":
                            UpdateEvAndSlider(5, ev, slSpeEv);
                            break;
                    }
                }
                else
                {
                    // Gérez le cas où tb.Text n'est pas un entier valide.
                }
            }
        }

        private void slEv_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            if (currentPokemon == null) return;

            int totalEVs = GetSliderTotal();
            int maxTotalEVs = 508;

            if (totalEVs > maxTotalEVs)
            {
                // Calculez la différence entre la somme actuelle et 508
                int difference = totalEVs - maxTotalEVs;

                // Vérifiez si la valeur du slider dépasse la différence
                if (slider.Value > difference)
                {
                    // Reduisez la valeur du slider
                    slider.Value -= difference;
                }
                else
                {
                    // Réglez la valeur du slider à zéro pour éviter les valeurs négatives
                    slider.Value = 0;
                }
            }

            switch (slider.Name)
            {
                case "slHpEv":
                    tbHpEv.Text = slider.Value.ToString();
                    break;
                case "slAttEv":
                    tbAttEv.Text = slider.Value.ToString();
                    break;
                case "slDefEv":
                    tbDefEv.Text = slider.Value.ToString();
                    break;
                case "slSAttEv":
                    tbSAttEv.Text = slider.Value.ToString();
                    break;
                case "slSDefEv":
                    tbSDefEv.Text = slider.Value.ToString();
                    break;
                case "slSpeEv":
                    tbSpeEv.Text = slider.Value.ToString();
                    break;
            }
        }

        
        private void UpdateShowedEVs()
        {
            slHpEv.Value = currentPokemon.Evs[0];
            slAttEv.Value = currentPokemon.Evs[1];
            slDefEv.Value = currentPokemon.Evs[2];
            slSAttEv.Value = currentPokemon.Evs[3];
            slSDefEv.Value = currentPokemon.Evs[4];
            slSpeEv.Value = currentPokemon.Evs[5];

            tbHpEv.Text = currentPokemon.Evs[0].ToString();
            tbAttEv.Text = currentPokemon.Evs[1].ToString();
            tbDefEv.Text = currentPokemon.Evs[2].ToString();
            tbSAttEv.Text = currentPokemon.Evs[3].ToString();
            tbSDefEv.Text = currentPokemon.Evs[4].ToString();
            tbSpeEv.Text = currentPokemon.Evs[5].ToString();
        }

        private int GetSliderTotal()
        {
            return (int)(slHpEv.Value + slAttEv.Value + slDefEv.Value + slSAttEv.Value + slSDefEv.Value + slSpeEv.Value);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string txt = "";
            foreach (int z in currentPokemon.Evs)
            {
                txt += z.ToString() + ", ";
            }
            MessageBox.Show(currentPokemon.Name + " " + txt);

        }

        private void tbIv_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (String.IsNullOrEmpty(tb.Text)) { tb.Text = "0"; return; }

            int iv;
            if (int.TryParse(tb.Text, out iv))
            {
                if (iv > 31)
                {
                    tb.Text = "31";
                }
            }
        }
    }
}
