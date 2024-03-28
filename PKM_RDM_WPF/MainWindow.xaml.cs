using Newtonsoft.Json;
using POKEMONCALCULATORWPF.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Data;
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Text.RegularExpressions;

namespace POKEMONCALCULATORWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isBtnRandomTeamBusy;
        WindowSwitchPokemon winSwitch;
        private Pokemon currentPokemon;
        private string currentTeamName = "Team Name";
        private AllPokemon allPokemonData;

        public MainWindow()
        {
            InitializeComponent();
            Show();
            winSwitch = new WindowSwitchPokemon(this, applicationData);
        }

        private async void RandomTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!MainPokemonCalc.IsInternetConnected()) { ShowConnexionError("You must be connected to internet"); return; }
            await NewRandomTeam();
            tbTeamName.Text = "";
        }

        private void CopyTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isBtnRandomTeamBusy) return;
            if (!string.IsNullOrEmpty(GetPokemonsNamesTeam()))
            {
                Clipboard.SetText(GetPokemonsNamesTeam()); // Copie le texte dans le presse-papiers
            }
            else
            {
                MessageBox.Show("Le champ de texte est vide. Rien à copier.");
            }
        }

        private string GetPokemonsNamesTeam()
        {
            string team ="";

            foreach (Pokemon p in applicationData.PokemonTeam)
            {
                team +=p.Name + "\nAbility: " + p.WantedAbility;
                team += "\nTera Type: " + p.TeraType.ToString();
                team += "\nEVs: " + p.GetEvsTextForShowdown();
                team += "\n" + p.WantedNature.GetOnlyNatureName() + " Nature";
                if (!String.IsNullOrEmpty(p.GetIvsTextForShowdown()))
                {
                    team += "\n" + p.GetIvsTextForShowdown();
                }
                team += "\n\n";
            }
            return team;
        }

        private async Task NewRandomTeam() {

            if (isBtnRandomTeamBusy) return;

            isBtnRandomTeamBusy = true;
            Brush baseBackgroundColor = RandomTeamBtn.Background.CloneCurrentValue();
            RandomTeamBtn.Background = Brushes.Red; // Notifier du chargement
            applicationData.PokemonTeam = new ObservableCollection<Pokemon>(await MainPokemonCalc.GetRandomPokemonTeam());
            ReSetWindowAndTeam(0);
            RandomTeamBtn.Background = baseBackgroundColor;
            isBtnRandomTeamBusy = false;
        }

        private async void LoadProperties()
        {
            foreach (Pokemon p in applicationData.PokemonTeam)
            {
                p.SetFrName();
                p.SetDoubleTypesSpecifiations();
                if (p.Abilities.Count > 1)
                {
                    if (p.Abilities.First().Ability.Name == p.Abilities.Last().Ability.Name)
                    {
                        p.Abilities.Remove(p.Abilities.Last());
                    }
                }
                p.Bst = 0;
                foreach (Stat stat in p.Stats)
                {
                    p.Bst += stat.Base_stat;
                }
                p.SetEvsAndIvs();
                Random r = new Random();
                p.WantedAbility = p.Abilities[r.Next(0, p.Abilities.Count)].Ability.Name;
                p.TeraType = (TypeP)Enum.Parse(typeof(TypeP), applicationData.AllType[r.Next(0,18)]);
                p.ChooseBestNature();
            }
            await LoadTooltip(); // en dernier
        }

        private async Task LoadTooltip() {
            foreach (Pokemon p in applicationData.PokemonTeam)
            {
                // await en dernier
                foreach (Abilities ability in p.Abilities)
                {
                    if(ability.Effect == null)
                    {
                        await ability.GetEffectChange();
                    }
                }
            }
        }

        private async void LoadAllPokemonName()
        {
            if (File.Exists(AllPokemon.CHEMIN_ALL_POKEMON_NAME)) // Si le fichier existe, on regarde si on peut le mettre à jour
            {
                string jsonContenu = File.ReadAllText(AllPokemon.CHEMIN_ALL_POKEMON_NAME);
                
                // Désérialiser le contenu JSON en un objet
                #warning todo : répétition pas opti, refaire système NB
                allPokemonData = JsonConvert.DeserializeObject<AllPokemon>(jsonContenu);
                await MainPokemonCalc.GetPokemonCount();

                if (AllPokemon.NB_Pokemon != allPokemonData.Results.Count)
                {
                    //MessageBox.Show(AllPokemon.NB_POKEMON + " " + allPokemonData.Results.Count);
                    allPokemonData = await MainPokemonCalc.GetAllPokemonNameUrl();
                }
            }
            else // Si le fichier n'existe pas, on le crée et on lui assigne les données
            {
                if (!Directory.Exists("data"))
                {
                    Directory.CreateDirectory("data");
                }

                string json = JsonConvert.SerializeObject(await MainPokemonCalc.GetAllPokemonNameUrl());
                File.WriteAllText(AllPokemon.CHEMIN_ALL_POKEMON_NAME, json);
                allPokemonData = JsonConvert.DeserializeObject<AllPokemon>(File.ReadAllText(AllPokemon.CHEMIN_ALL_POKEMON_NAME));
            }
            applicationData.AllPokemonName = new ObservableCollection<string>(allPokemonData.GetAllPokemonName());
        }

        // Se lance au chargement de la fenetre (listview)
        private async void teamListBox_Loaded(object sender, RoutedEventArgs e)
        {
            await MainPokemonCalc.GetPokemonCount(); // On fait une requete pour avoir le nb de pokemon
            LoadAllPokemonName();
            ReadCurrentTeamName();
            applicationData.AllType = new ObservableCollection<string>(Enum.GetNames(typeof(TypeP)));
            applicationData.AllNature = new ObservableCollection<string>(Nature.NATURES);
            if (Directory.Exists(Pokemon.CHEMIN_DOSSIER))
            {
                string[] teams = Directory.GetDirectories(Pokemon.CHEMIN_DOSSIER);
                teams = PathToName(teams);
                if (teams.Length == 0) // Si pas de team, on en crée une nouvelle
                {
                    NewRandomTeam();
                    currentPokemon = (Pokemon)teamListImageView.SelectedItem;
                }
                else
                {
                    string[] fichiersDansDossier = Directory.GetFiles($"{Pokemon.CHEMIN_DOSSIER}/{teams.ToList().Find(x => x == currentTeamName)}");

                    if (fichiersDansDossier.Length == 6)
                    {
                        SwitchPokemonTeam(fichiersDansDossier);
                    }
                    else
                    {
                        NewRandomTeam();
                    }
                    currentPokemon = (Pokemon)teamListImageView.SelectedItem;
                    //MessageBox.Show(currentPokemon.Name);
                }
            }
            else // Si le dossier n'existe pas, on le crée
            {
                Directory.CreateDirectory(Pokemon.CHEMIN_DOSSIER);
                NewRandomTeam();
                currentPokemon = (Pokemon)teamListImageView.SelectedItem;
            }
            tbTeamName.Text = currentTeamName;
        }

        private static string[] PathToName(string[] paths)
        {
            string[] names = new string[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                names[i] = Path.GetFileName(paths[i]);
            }
            return names;
        }

        private void SwitchPokemonTeam(string[] filesInFolder)
        {
            List<Pokemon> pokemons = new List<Pokemon>();
            foreach (string fichier in filesInFolder)
            {
                string jsonContenu = File.ReadAllText(fichier);

                // Désérialiser le contenu JSON en un objet
                Pokemon p = JsonConvert.DeserializeObject<Pokemon>(jsonContenu);
                pokemons.Add(p);
            }
            applicationData.PokemonTeam = new ObservableCollection<Pokemon>(pokemons);
            RefreshWindow(0);
            SwitchTooltip();
            WriteTeamNameInData();
        }

        private void WriteTeamNameInData()
        {
            string appDataPath = "data/appData.json";

            // Créez le répertoire "data" s'il n'existe pas
            Directory.CreateDirectory(Path.GetDirectoryName(appDataPath));

            // Utilisez StreamWriter pour écrire le contenu dans le fichier
            using (StreamWriter writer = new StreamWriter(appDataPath))
            {
                writer.Write(currentTeamName);
            }
        }
        private void ReadCurrentTeamName()
        {
            if (!File.Exists("data/appData.json")) return;
            string tn = File.ReadAllText("data/appData.json");
            if (!String.IsNullOrWhiteSpace(tn))
            {
                currentTeamName = tn;
            }
        }

        public void ReSetWindowAndTeam(int index)
        {
            LoadProperties();
            RefreshWindow(index);
        }

        public void RefreshWindow(int index)
        {
            teamListImageView.ItemsSource = applicationData.PokemonTeam; 
            cbTera.ItemsSource = applicationData.AllType;
            cbNature.ItemsSource = applicationData.AllNature;
            teamListImageView.SelectedIndex = index; 
        }

        private void lvOpenSwitchWindow(object sender, MouseButtonEventArgs e)
        {
            if (!MainPokemonCalc.IsInternetConnected()) { ShowConnexionError("You must be connected to internet"); return; }
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                object dC = ((ListViewItem)sender).DataContext;
                winSwitch.selectedPokemon = ((Pokemon)dC);
                winSwitch.ShowDialog();
            }
        }

        private void SaveTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isBtnRandomTeamBusy) return;
            if(String.IsNullOrWhiteSpace(tbTeamName.Text)) { MessageBox.Show("You must enter a team name", "NULL EXCEPTION", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if(tbTeamName.Text.Length > 30) { MessageBox.Show("Team name must not exceed 30 characters", "LENGTH EXCEPTION", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if(!Regex.IsMatch(tbTeamName.Text, "^[a-zA-Z0-9_]+$")){ MessageBox.Show("Team name must consist of letters or numbers", "FORMAT EXCEPTION", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            string[] teams = Directory.GetDirectories(Pokemon.CHEMIN_DOSSIER);
            teams = PathToName(teams);
            if (teams.Contains(tbTeamName.Text.ToLower()))
            {
                MessageBoxResult r = MessageBox.Show("Do you want to override an existing team ?", "Team Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r == MessageBoxResult.No)
                {
                    //tbTeamName.Text = currentTeamName;
                    return;
                }
            }

            // On crée le directory de la team si elle n'existe pas
            string teamPath = Pokemon.CHEMIN_DOSSIER + "/" + tbTeamName.Text.ToLower();
            if (!Directory.Exists(teamPath))
            {
                Directory.CreateDirectory(teamPath);
            }

            string[] fichiersDansDossier = Directory.GetFiles(teamPath);
            foreach (string item in fichiersDansDossier) // On supprime les anciens fichiers
            {
                File.Delete(item);
            }
            foreach (Pokemon p in applicationData.PokemonTeam) // On sauvegarde les nouveaux
            {
                p.Serialize(tbTeamName.Text.ToLower());
            }
            currentTeamName = tbTeamName.Text.ToLower();
            WriteTeamNameInData();

        }

        private void LoadTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isBtnRandomTeamBusy) return;
            string teamsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Pokemon.CHEMIN_DOSSIER);

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = teamsDirectory;
            dialog.Multiselect = false;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                string teamName = Path.GetFileName(dialog.FileName);
                currentTeamName = teamName;
                tbTeamName.Text = currentTeamName;
                string[] fichiersDansDossier = Directory.GetFiles(dialog.FileName);
                SwitchPokemonTeam(fichiersDansDossier);
            }
        }

        private bool canUpdate;

        private void teamListImageView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            canUpdate = false;
            ListView lv = ((ListView)sender);
            if (lv.SelectedItem == null) return;
            
            currentPokemon = applicationData.PokemonTeam[applicationData.PokemonTeam.ToList().FindIndex(x => x.GetHashCode() == ((Pokemon)lv.SelectedItem).GetHashCode())];
            cbAbility.SelectedIndex = currentPokemon.GetIndexOfWantedAbility();
            cbTera.SelectedIndex = GetIndexOfWantedTera(currentPokemon);
            if(currentPokemon.WantedNature != null)
            {
                cbNature.SelectedIndex = GetIndexOfWantedNature(currentPokemon);
            }
            UpdateShowedEVs();
            UpdateRemainingEvsText();
            //SwitchTooltip();
            //MessageBox.Show(currentPokemon.Name);
            canUpdate = true;
        }

        private void cbAbility_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentPokemon == null) return;
            if(cbAbility.SelectedIndex == -1) return;
            currentPokemon.WantedAbility = currentPokemon.Abilities[cbAbility.SelectedIndex].Ability.Name;
            cbAbility.SelectedIndex = currentPokemon.GetIndexOfWantedAbility();

            //SwitchTooltip();
        }
        private void SwitchTooltip()
        {
            //tooltips
            if(currentPokemon == null) return;
            if(currentPokemon.Abilities[cbAbility.SelectedIndex].Effect != null || currentPokemon.Abilities[cbAbility.SelectedIndex].EffectEntries != null)
            {
                tbAbilityTooltip.Text = currentPokemon.Abilities[cbAbility.SelectedIndex].GetDescription();
            }
        }

        private void cbTera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentPokemon == null) return;
            if (cbTera.SelectedIndex == -1) return;
            currentPokemon.TeraType = (TypeP)Enum.Parse(typeof(TypeP), applicationData.AllType[cbTera.SelectedIndex]);
        }

        private int GetIndexOfWantedTera(Pokemon p)
        {
            int index = applicationData.AllType.IndexOf(applicationData.AllType.ToList().Find(x => x == p.TeraType.ToString()));
            return index;
        }
        private int GetIndexOfWantedNature(Pokemon p)
        {
            int index = applicationData.AllNature.IndexOf(applicationData.AllNature.ToList().Find(x => x == p.WantedNature.Name));
            return index;
        }

        private void cbNature_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentPokemon == null) return;
            if (cbNature.SelectedIndex == -1) return;
            currentPokemon.WantedNature.Name = applicationData.AllNature[cbNature.SelectedIndex];
        }

        private void UpdateEvAndSlider(int index, int value, Slider slider)
        {
            currentPokemon.Evs[index] = value;
            slider.Value = currentPokemon.Evs[index];
        }
        private void tbEv_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (canUpdate != true) return;   
            TextBox tb = (TextBox)sender;
            if (String.IsNullOrEmpty(tb.Text)) return;

            if (currentPokemon != null && currentPokemon.Evs != null)
            {
                int ev;
                if (int.TryParse(tb.Text, out ev))
                {

                    switch (tb.Name)
                    {
                        case "tbHpEv":
                            UpdateEvAndSlider(0, ev, slHpEv);
                            break;
                        case "tbAttEv":
                            UpdateEvAndSlider(1, ev, slAttEv);
                            break;
                        case "tbDefEv":
                            UpdateEvAndSlider(2, ev, slDefEv);
                            break;
                        case "tbSAttEv":
                            UpdateEvAndSlider(3, ev, slSAttEv);
                            break;
                        case "tbSDefEv":
                            UpdateEvAndSlider(4, ev, slSDefEv);
                            break;
                        case "tbSpeEv":
                            UpdateEvAndSlider(5, ev, slSpeEv);
                            break;
                    }
                    UpdateRemainingEvsText();
                }
                else
                {
                    // Gérez le cas où tb.Text n'est pas un entier valide.
                }
            }
        }

        private void slEv_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (canUpdate != true) return;   
            Slider slider = (Slider)sender;
            if (currentPokemon == null) return;

            int totalEVs = GetSliderTotal();

            if (totalEVs > Pokemon.MAX_EV_DISTRIBUTION)
            {
                // Calculez la différence entre la somme actuelle et 508
                int difference = totalEVs - Pokemon.MAX_EV_DISTRIBUTION;

                // Vérifiez si la valeur du slider dépasse la différence
                if (slider.Value > difference)
                {
                    // Reduisez la valeur du slider
                    slider.Value -= difference;
                }
                else
                {
                    // Réglez la valeur du slider à zéro pour éviter les valeurs négatives
                    slider.Value = 0;
                }
            }

            switch (slider.Name)
            {
                case "slHpEv":
                    tbHpEv.Text = slider.Value.ToString();
                    break;
                case "slAttEv":
                    tbAttEv.Text = slider.Value.ToString();
                    break;
                case "slDefEv":
                    tbDefEv.Text = slider.Value.ToString();
                    break;
                case "slSAttEv":
                    tbSAttEv.Text = slider.Value.ToString();
                    break;
                case "slSDefEv":
                    tbSDefEv.Text = slider.Value.ToString();
                    break;
                case "slSpeEv":
                    tbSpeEv.Text = slider.Value.ToString();
                    break;
            }
            UpdateRemainingEvsText();
        }

        
        private void UpdateShowedEVs()
        {
            slHpEv.Value = currentPokemon.Evs[0];
            slAttEv.Value = currentPokemon.Evs[1];
            slDefEv.Value = currentPokemon.Evs[2];
            slSAttEv.Value = currentPokemon.Evs[3];
            slSDefEv.Value = currentPokemon.Evs[4];
            slSpeEv.Value = currentPokemon.Evs[5];

            tbHpEv.Text = currentPokemon.Evs[0].ToString();
            tbAttEv.Text = currentPokemon.Evs[1].ToString();
            tbDefEv.Text = currentPokemon.Evs[2].ToString();
            tbSAttEv.Text = currentPokemon.Evs[3].ToString();
            tbSDefEv.Text = currentPokemon.Evs[4].ToString();
            tbSpeEv.Text = currentPokemon.Evs[5].ToString();
        }

        private int GetSliderTotal()
        {
            return (int)(slHpEv.Value + slAttEv.Value + slDefEv.Value + slSAttEv.Value + slSDefEv.Value + slSpeEv.Value);
        }

        private void tbIv_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (String.IsNullOrEmpty(tb.Text)) { tb.Text = "0"; return; }

            int iv;
            if (int.TryParse(tb.Text, out iv))
            {
                if (iv > 31)
                {
                    tb.Text = "31";
                }
            }
        }

        public static void ShowConnexionError(string txt)
        {
            MessageBox.Show(txt,"Connexion Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void UpdateRemainingEvsText()
        {
            lbRemainingEVsValue.Content = currentPokemon.GetRemainingEvs().ToString();
        }

        // Exit app
        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Open about window
        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            WindowAbout winAbout = new WindowAbout(this);
            winAbout.ShowDialog();
        }

        private void miHelp_Click(object sender, RoutedEventArgs e)
        {
            WindowTips winAbout = new WindowTips(this);
            winAbout.ShowDialog();
        }

        private void cbAbility_MouseEnter(object sender, MouseEventArgs e)
        {
            SwitchTooltip();
        }

        private void tbTeamName_GotFocus(object sender, RoutedEventArgs e)
        {
            tbTeamName.Text = "";
        }
    }
}
