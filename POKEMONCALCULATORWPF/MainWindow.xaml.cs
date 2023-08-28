using POKEMONCALCULATORWPF.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void RandomTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            await RandomTeamRefresh();
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

            foreach (Pokemon p in applicationData.PokemonTeam)
            {
                team += p.Name + " ";
            }
            return team;
        }

        private async Task RandomTeamRefresh() {

            if (isBtnRandomTeamBusy) return;

            isBtnRandomTeamBusy = true;
            RandomTeamBtn.Background = Brushes.Red;
            applicationData.PokemonTeam = new ObservableCollection<Pokemon>(await MainPokemonCalc.GetRandomPokemonTeam());
            RefreshWindow();
            RandomTeamBtn.Background = Brushes.Gray;
            isBtnRandomTeamBusy = false;
        }

        private async Task LoadProperties()
        {
            foreach (Pokemon p in applicationData.PokemonTeam)
            {
                p.FrName = p.ToFrString();
            }
        }

        private async void teamListBox_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshListView();
        }

        private void RefreshWindow()
        {
            //teamListBox.ItemsSource = applicationData.PokemonTeam;
            teamListImageView.ItemsSource = applicationData.PokemonTeam; // actualisation
            RefreshListView();
        }

        private async void RefreshListView()
        {
            await RandomTeamRefresh();
            await LoadProperties();
            teamListImageView.SelectedIndex = -1;
            teamListImageView.SelectedIndex = 0; // actualisation
        }
    }
}
