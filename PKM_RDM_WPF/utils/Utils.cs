using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PKM_RDM_WPF.utils
{
    public static class Utils
    {
        public static JsonSerializerSettings IgnoreNullsJsonSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            return settings;
        }

        public static string ToNiceString(string chaine)
        {
            // Met la première lettre en majuscule et les autres en minuscule
            TextInfo textInfo = new CultureInfo("fr-FR", false).TextInfo;
            string chaineFormatee = textInfo.ToTitleCase(chaine.ToLower());

            // Remplace les tirets par des espaces
            chaineFormatee = chaineFormatee.Replace("-", " ");

            return chaineFormatee;
        }

        public static string[] PathToName(string[] paths)
        {
            string[] names = new string[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                names[i] = Path.GetFileName(paths[i]);
            }
            return names;
        }

        public static void ShowConnexionError(string txt)
        {
            MessageBox.Show(txt, "Connexion Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
