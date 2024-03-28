using PKM_RDM_WPF.model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PKM_RDM_WPF.converter
{
    public class StringToCatImgConverter : IValueConverter
    {
        string[] moovCategoryImgPaths =
        {
            "img/moov_cat/PhysicalIC.png", "img/moov_cat/SpecialIC.png", "img/moov_cat/StatusIC.png"
        };


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imageName;
            if (value is string)
            {
                string type = (string)value;
                imageName = GetImagePath(type);
            }
            else if (value is string) { string type = (string)value; imageName = GetImagePath(type); }
            else throw new Exception("error");
            string imagePath = $"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName().Name};component/{imageName}";
            return new BitmapImage(new Uri(imagePath));

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetImagePath(string text)
        {
            // Logique pour mapper la chaîne à un chemin d'accès d'image
            switch (text)
            {
                case "Status":
                    return moovCategoryImgPaths[2];
                case "Physical":
                    return moovCategoryImgPaths[0]; 
                case "Special":
                    return moovCategoryImgPaths[1];
            }
            return "img/question_mark.png";
        }
    }
}
