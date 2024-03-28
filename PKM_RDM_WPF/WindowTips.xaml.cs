using System;
using System.Collections.Generic;
using System.Linq;
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

namespace PKM_RDM_WPF
{
    /// <summary>
    /// Logique d'interaction pour WindowTips.xaml
    /// </summary>
    public partial class WindowTips : Window
    {
        public WindowTips(Window owner)
        {
            this.Owner = owner;
            InitializeComponent();
        }
    }
}
