using POKEMONCALCULATORWPF.model;
using System;
using System.Collections.Generic;
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

        public WindowSwitchPokemon(MainWindow owner, ApplicationData applicationData)
        {
            this.Owner = owner;
            appData = applicationData;
            this.DataContext = owner.DataContext;
            InitializeComponent();
        }

        public void RefreshShowedPokemon()
        {
            lvPokemonNameList.ItemsSource = appData.AllPokemonName; // actualisation
            SetPokemon();
        }

        private async void SetPokemon()
        {
            imgSelectedPokemon.Source = new BitmapImage(new Uri(selectedPokemon.Sprites.Front_default));
            foreach (string p in lvPokemonNameList.Items)
            {
                if (p.Equals(selectedPokemon.Name))
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
            e.Cancel = true;
            this.Hide();
        }

        private async void lvPokemonNameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            newSelectedPokemon = await MainPokemonCalc.GetPokemonByName(((string)lvPokemonNameList.SelectedItem));
            imgSelectedPokemon.Source = new BitmapImage(new Uri(newSelectedPokemon.Sprites.Front_default));
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            RefreshShowedPokemon();
        }

        private async void btnSwicthPokemon_Click(object sender, RoutedEventArgs e)
        {
            int index = appData.PokemonTeam.IndexOf(selectedPokemon);
            appData.PokemonTeam.Remove(selectedPokemon);
            appData.PokemonTeam.Insert(index, newSelectedPokemon);
            this.Owner.DataContext = appData;

            ((MainWindow)this.Owner).RefreshWindow(index);
            this.Hide();
        }
    }
}
