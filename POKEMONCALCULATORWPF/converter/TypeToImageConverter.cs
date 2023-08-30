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
    public class TypeToImageConverter : IValueConverter
    {
        string[] typeImages =
        {
            "img/SteelIC_SV.png", "img/FightingIC_SV.png", "img/DragonIC_SV.png", "img/WaterIC_SV.png",
            "img/ElectricIC_SV.png", "img/FairyIC_SV.png", "img/FireIC_SV.png", "img/IceIC_SV.png",
            "img/BugIC_SV.png", "img/NormalIC_SV.png", "img/GrassIC_SV.png", "img/PoisonIC_SV.png",
            "img/PsychicIC_SV.png", "img/RockIC_SV.png", "img/GroundIC_SV.png", "img/GhostIC_SV.png",
            "img/DarkIC_SV.png", "img/FlyingIC_SV.png"
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
