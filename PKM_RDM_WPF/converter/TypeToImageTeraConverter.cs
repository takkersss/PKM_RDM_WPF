using POKEMONCALCULATORWPF.model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace POKEMONCALCULATORWPF.converter
{
    public class TypeToImageTeraConverter : IValueConverter
    {
        string[] typeImages =
{
    "img/tera/SteelIC_Tera.png", "img/tera/FightingIC_Tera.png", "img/tera/DragonIC_Tera.png", "img/tera/WaterIC_Tera.png",
    "img/tera/ElectricIC_Tera.png", "img/tera/FairyIC_Tera.png", "img/tera/FireIC_Tera.png", "img/tera/IceIC_Tera.png",
    "img/tera/BugIC_Tera.png", "img/tera/NormalIC_Tera.png", "img/tera/GrassIC_Tera.png", "img/tera/PoisonIC_Tera.png",
    "img/tera/PsychicIC_Tera.png", "img/tera/RockIC_Tera.png", "img/tera/GroundIC_Tera.png", "img/tera/GhostIC_Tera.png",
    "img/tera/DarkIC_Tera.png", "img/tera/FlyingIC_Tera.png" //, "img/tera/StellarIC_Tera.png"
};


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imageName;
            if (value is TypeP)
            {
                TypeP type = (TypeP)value;
                imageName = GetImagePathFromType(type.ToString());
            }
            else if (value is string) { string type = (string)value; imageName = GetImagePathFromType(type); }
            else throw new Exception("error");
            string imagePath = $"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName().Name};component/{imageName}";
            return new BitmapImage(new Uri(imagePath));

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetImagePathFromType(string type)
        {
            int index = Array.IndexOf(Enum.GetNames(typeof(TypeP)), type); // Obtenir l'indice du nom dans l'énumération
            if (index >= 0 && index < typeImages.Length)
            {
                return typeImages[index];
            }
            return null; // ou une valeur par défaut si nécessaire
        }
    }
}
