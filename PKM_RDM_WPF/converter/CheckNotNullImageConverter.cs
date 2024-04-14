using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PKM_RDM_WPF.converter
{
    public class CheckNotNullImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Vérifie si la valeur est null et renvoie une valeur de secours si c'est le cas
            if (value == null)
            {
                return "img/question_mark.png"; // Remplacez cela par le chemin de votre image de secours
            }
            // Si la valeur n'est pas null, retourne la valeur d'origine
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
