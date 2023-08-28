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

        public WindowSwitchPokemon(Window owner, ApplicationData appDataaa)
        {
            this.Owner = owner;
            appData = appDataaa;
            InitializeComponent();
        }

        private void lvPokemonNameList_Loaded(object sender, RoutedEventArgs e)
        {
            lvPokemonNameList.ItemsSource = appData.AllPokemonName;
            //MessageBox.Show(appData.AllPokemonName[1009]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
