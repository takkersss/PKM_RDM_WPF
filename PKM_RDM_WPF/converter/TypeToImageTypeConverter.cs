using PKM_RDM_WPF.model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PKM_RDM_WPF.converter
{
    public class TypeToImageTypeConverter : IValueConverter
    {
        string[] typeImages =
{
    "img/type/SteelIC_SV.png", "img/type/FightingIC_SV.png", "img/type/DragonIC_SV.png", "img/type/WaterIC_SV.png",
    "img/type/ElectricIC_SV.png", "img/type/FairyIC_SV.png", "img/type/FireIC_SV.png", "img/type/IceIC_SV.png",
    "img/type/BugIC_SV.png", "img/type/NormalIC_SV.png", "img/type/GrassIC_SV.png", "img/type/PoisonIC_SV.png",
    "img/type/PsychicIC_SV.png", "img/type/RockIC_SV.png", "img/type/GroundIC_SV.png", "img/type/GhostIC_SV.png",
    "img/type/DarkIC_SV.png", "img/type/FlyingIC_SV.png"
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
