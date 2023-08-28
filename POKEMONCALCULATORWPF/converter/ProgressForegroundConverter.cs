using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace POKEMONCALCULATORWPF.converter
{
    public class ProgressForegroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double progress = (double)value;
            Brush foreground = Brushes.Green;

            if (progress >= 100d)
            {
                foreground = Brushes.Green;
            }
            else if (progress > 75)
            {
                foreground = Brushes.Yellow;
            }
            else if (progress >= 60)
            {
                foreground = Brushes.Orange;
            }else foreground = Brushes.Red;


            return foreground;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
