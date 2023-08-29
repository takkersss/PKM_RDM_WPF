using Newtonsoft.Json;
using POKEMONCALCULATORWPF.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
                // Type Aleatoire
                TypeP teraType = (TypeP)valeursEnum.GetValue(random.Next(valeursEnum.Length));

                // Ability Aleatoire
                String ability = p.Abilities[random.Next(0, p.Abilities.Count)].Ability.Name;

                team +=p.Name + "\nAbility: " + ability;
                team += "\nTera Type: " + teraType.ToString() +"\n" + "\n";
            }
            return team;
        }

        private async Task NewRandomTeam() {

            if (isBtnRandomTeamBusy) return;

            isBtnRandomTeamBusy = true;
            RandomTeamBtn.Background = Brushes.Red;
            applicationData.PokemonTeam = new ObservableCollection<Pokemon>(await MainPokemonCalc.GetRandomPokemonTeam());
            RefreshWindow(0);
            RandomTeamBtn.Background = Brushes.Gray;
            isBtnRandomTeamBusy = false;
        }

        private void LoadProperties()
        {
            foreach (Pokemon p in applicationData.PokemonTeam)
            {
                p.SetFrName();
                p.SetDoubleTypesSpecifiations();
            }
        }

        // Se lance au chargement de la fenetre (listview)
        private async void teamListBox_Loaded(object sender, RoutedEventArgs e)
        {
            applicationData.AllPokemonName = new ObservableCollection<String>(await MainPokemonCalc.GetAllPokemonName());
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
                    teamListImageView.ItemsSource = applicationData.PokemonTeam; // actualisation
                }
                else
                {
                    NewRandomTeam();
                }
            }
            else
            {
                throw new Exception("directory doesn't exist");
            }
        }

        public void RefreshWindow(int index)
        {
            //teamListBox.ItemsSource = applicationData.PokemonTeam;
            LoadProperties();
            teamListImageView.ItemsSource = applicationData.PokemonTeam; // actualisation
            teamListImageView.SelectedIndex = -1;
            teamListImageView.SelectedIndex = index; // actualisation
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
    }
}
