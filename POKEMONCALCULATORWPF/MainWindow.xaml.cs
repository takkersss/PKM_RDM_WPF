using Newtonsoft.Json;
using POKEMONCALCULATORWPF.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
                team += "\nTera Type: " + p.TeraType.ToString() +"\n" + "\n";
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
            teamListImageView.ItemsSource = applicationData.PokemonTeam; // actualisation
            cbTera.ItemsSource = applicationData.AllType; // actualisation
            teamListImageView.SelectedIndex = -1;
            teamListImageView.SelectedIndex = index; // actualisation;
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
            currentPokemon = (Pokemon)lv.SelectedItem;
            cbAbility.SelectedIndex = currentPokemon.GetIndexOfWantedAbility();
            cbTera.SelectedIndex = GetIndexOfWantedTera(currentPokemon);
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
    }
}
