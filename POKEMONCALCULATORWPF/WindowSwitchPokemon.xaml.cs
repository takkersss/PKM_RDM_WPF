using POKEMONCALCULATORWPF.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POKEMONCALCULATORWPF
{
    /// <summary>
    /// Logique d'interaction pour WindowSwitchPokemon.xaml
    /// </summary>
    public partial class WindowSwitchPokemon : Window
    {
        private ApplicationData appData;
        public Pokemon selectedPokemon, newSelectedPokemon;
        private bool isWindowBusy;

        public WindowSwitchPokemon(MainWindow owner, ApplicationData applicationData)
        {
            this.Owner = owner;
            appData = applicationData;
            this.DataContext = owner.DataContext;
            InitializeComponent();
        }

        public void RefreshShowedPokemon()
        {
            appData.AllPokemonNameFiltres = appData.AllPokemonName;
            SetPokemon(newSelectedPokemon);
            RefreshList();
        }

        private async void SetPokemon(Pokemon poke)
        {
            imgSelectedPokemon.Source = new BitmapImage(new Uri(poke.Sprites.Front_default));
            foreach (string p in lvPokemonNameList.Items)
            {
                if (p.Equals(poke.Name))
                {
                    // Élément trouvé, faites quelque chose avec lui
                    // Par exemple, sélectionnez-le et faites défiler la ListView
                    lvPokemonNameList.SelectedItem = p;
                    lvPokemonNameList.ScrollIntoView(p);

                    break; // Sortez de la boucle car l'élément a été trouvé
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            txtBoxRecherche.Text = "";
            e.Cancel = true;
            this.Hide();
        }

        private async void lvPokemonNameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((string)lvPokemonNameList.SelectedItem == null) || isWindowBusy) return;
            isWindowBusy = true;
            newSelectedPokemon = await MainPokemonCalc.GetPokemonByName(((string)lvPokemonNameList.SelectedItem));
            imgSelectedPokemon.Source = new BitmapImage(new Uri(newSelectedPokemon.Sprites.Front_default));
            isWindowBusy = false;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            newSelectedPokemon = selectedPokemon;
            RefreshShowedPokemon();
        }

        private void txtBoxRecherche_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtBoxRecherche.Text)){
                appData.AllPokemonNameFiltres = new ObservableCollection<string>(appData.AllPokemonName.ToList().FindAll(x => x.StartsWith(txtBoxRecherche.Text.Substring(0,1).ToUpper()+ txtBoxRecherche.Text.Substring(1, txtBoxRecherche.Text.Length-1).ToLower())));
                RefreshList();
            }
            else
            {
                appData.AllPokemonNameFiltres = appData.AllPokemonName;
                RefreshList(); // actualisation
            }
        }

        private async void btnSwicthPokemon_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowBusy) return;
            int index = appData.PokemonTeam.IndexOf(selectedPokemon);
            appData.PokemonTeam.Remove(selectedPokemon);
            appData.PokemonTeam.Insert(index, newSelectedPokemon);
            this.Owner.DataContext = appData;
            txtBoxRecherche.Text = "";

            ((MainWindow)this.Owner).ReSetWindowAndTeam(index);
            this.Hide();
        }

        private async void btnRandomPokemon_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowBusy) return;
            newSelectedPokemon = await MainPokemonCalc.GetRandomPokemon();
            SetPokemon(newSelectedPokemon);
            imgSelectedPokemon.Source = new BitmapImage(new Uri(newSelectedPokemon.Sprites.Front_default));

        }

        private void RefreshList()
        {
            lvPokemonNameList.ItemsSource = appData.AllPokemonNameFiltres; // actualisation
        }
    }
}
