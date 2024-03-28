using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PKM_RDM_WPF.converter
{
    public class ProgressForegroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double progress = (double)value;
            Brush foreground;

            if (progress >= 100)
            {
                foreground = new SolidColorBrush(Color.FromRgb(152, 251, 152)); // Vert pastel
            }
            else if (progress > 75)
            {
                foreground = new SolidColorBrush(Color.FromRgb(255, 255, 153)); // Jaune pastel
            }
            else if (progress >= 60)
            {
                foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)); // Orange pastel
            }
            else
            {
                foreground = new SolidColorBrush(Color.FromRgb(255, 182, 193)); // Rose pastel
            }

            return foreground;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
