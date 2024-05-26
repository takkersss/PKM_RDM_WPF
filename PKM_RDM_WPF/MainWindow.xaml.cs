using Newtonsoft.Json;
using PKM_RDM_WPF.model;
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
using static PKM_RDM_WPF.utils.Utils;
using Microsoft.VisualBasic.Devices;
using PKM_RDM_WPF.engine;
using MaterialDesignThemes.Wpf;

namespace PKM_RDM_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isWindowBusy; // isRandomizingTeam
        WindowSwitchPokemon winSwitch;
        private Pokemon currentPokemon; //private string currentTeamName = "Team Name";
        private AppOptions appOptions;

        private AllPokemon allPokemon;
        private AllItems allItems;

        // RANDOMIZATION PARAMETERS
        private bool randStrongPokemons = false;

        // MOVE SYSTEM
        private bool moovSystemEnabled = false; // ne sert qu'une fois !
        private TextBox currentMoveSlotSelected;
        private bool isSwitchingMove = false;
        private bool isLoadingNewRandomTeam = false;

        // SORT MOVE SYSTEM
        private bool moovSortIsPowerAsc = true;
        //private bool moovSortIsNameAsc = false;
        //private bool moovSortIsTypeAsc = false;

        public MainWindow()
        {
            InitializeComponent();
            Show();
            winSwitch = new WindowSwitchPokemon(this, applicationData);
        }

        // RANDOM team button
        private async void RandomTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!MainPokemonCalc.IsInternetConnected()) { ShowConnexionError("You must be connected to internet"); return; }
            await NewRandomTeam();
            tbTeamName.Text = "";
        }

        private async Task NewRandomTeam() {

            if (isWindowBusy) return;
            LoadingIcon(); // Notifier du chargement
            isLoadingNewRandomTeam = true;
            applicationData.PokemonTeam = new ObservableCollection<Pokemon>(await MainPokemonCalc.GetRandomPokemonTeam(randStrongPokemons));
            ReSetWindowAndTeam(0);
        }

        // SET properties of a pokemon
        private async Task SetPokemonProperties(Pokemon p)
        {
            // p.SetFrName();
            p.SetDoubleTypesSpecifiations();
            if (p.Abilities.Count > 1)
            {
                if (p.Abilities.First().Ability.Name == p.Abilities.Last().Ability.Name)
                {
                    p.Abilities.Remove(p.Abilities.Last());
                }
            }
            p.Bst = p.Stats.Sum(stat => stat.Base_stat);
            p.SetEvsAndIvs();
            p.ChooseBestNature();
            Random r = new Random();
            p.WantedAbility = p.Abilities[r.Next(0, p.Abilities.Count)].Ability.Name;
            p.TeraType = (TypeP)Enum.Parse(typeof(TypeP), applicationData.AllType[r.Next(0, 18)]);
            p.ChooseBestItemButRandom(applicationData.AllItems.ToList()); // todo: Item en fonction des stats

            // MOVES
            if (moovSystemEnabled)
            {
                await LoadPokemonMovepool(p);
                p.RandomizeFourMoves(); // todo: Moovs en fonctions de l'item
            }
        }

        // LOAD properties of the pokemons of the team
        private async void LoadProperties()
        {
            foreach (Pokemon p in applicationData.PokemonTeam)
            {
                await SetPokemonProperties(p);
            }
            currentPokemon = applicationData.PokemonTeam[0];
            ActualizeFourMovesDisplay();
            LoadingIcon(false);
            isLoadingNewRandomTeam = false;

            await LoadTooltip(); // en dernier
        }

        // LOAD properties for one pokemon of the team
        private async void LoadPropertiesForOnePokemon(int pIndex)
        {
            await SetPokemonProperties(applicationData.PokemonTeam[pIndex]);
            currentPokemon = applicationData.PokemonTeam[0];
            ActualizeFourMovesDisplay();
            LoadingIcon(false);
            isLoadingNewRandomTeam = false;

            await LoadTooltip(); // en dernier
        }


        // LOAD tooltip for abilities
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

        // LOAD all pokemon names for switch window
        private async void LoadAllPokemonNames()
        {
            if (File.Exists(AllPokemon.CHEMIN_ALL_POKEMON_NAME)) // Si le fichier existe, on regarde si on peut le mettre à jour
            {
                string jsonContenu = File.ReadAllText(AllPokemon.CHEMIN_ALL_POKEMON_NAME);

                // Désérialiser le contenu JSON en un objet
                // à optimiser : appel await MainPokemonCalc.GetPokemonCount(); x2
                allPokemon = JsonConvert.DeserializeObject<AllPokemon>(jsonContenu);
                await MainPokemonCalc.GetPokemonCount();

                //MessageBox.Show($"{AllPokemon.NB_Pokemon} - {allPokemon.Results.Count}");
                if (AllPokemon.NB_Pokemon != allPokemon.Results.Count)
                {
                    //MessageBox.Show(AllPokemon.NB_POKEMON + " " + allPokemonData.Results.Count);
                    allPokemon = await MainPokemonCalc.GetAllPokemonNameUrl();
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
                allPokemon = JsonConvert.DeserializeObject<AllPokemon>(File.ReadAllText(AllPokemon.CHEMIN_ALL_POKEMON_NAME));
            }
            applicationData.AllPokemonName = new ObservableCollection<string>(allPokemon.GetAllPokemonName());
        }

        private async Task LoadAllItems()
        {
            if (File.Exists(AllItems.CHEMIN_ALL_ITEMS)) // todo : mettre items automatiquement à jour
            {
                string jsonContenu = File.ReadAllText(AllItems.CHEMIN_ALL_ITEMS);

                // Désérialiser le contenu JSON en un objet
                allItems = JsonConvert.DeserializeObject<AllItems>(jsonContenu);
            }
            else // Si le fichier n'existe pas, on le crée et on lui assigne les données
            {
                if (!Directory.Exists("data"))
                {
                    Directory.CreateDirectory("data");
                }
                AllItems items = await AllItems.GetBattleItems();
                await items.RetrieveItems(); // todo : éviter bloquer affichage
                string jsonItems = JsonConvert.SerializeObject(items);
                File.WriteAllText(AllItems.CHEMIN_ALL_ITEMS, jsonItems);
                allItems = items;
            }
            applicationData.AllItems = new ObservableCollection<Item>(allItems.ItemsGetted);
        }

        // Se lance au chargement de la fenetre (listview)
        private async void teamListBox_Loaded(object sender, RoutedEventArgs e)
        {
            await MainPokemonCalc.GetPokemonCount(); // à optimiser
            LoadAllPokemonNames();
            await LoadAllItems();
            ReadAppOptionsName();
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
                    string[] fichiersDansDossier = Directory.GetFiles($"{Pokemon.CHEMIN_DOSSIER}/{teams.ToList().Find(x => x == appOptions.CurrentTeamName)}");

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
            // Chargement des options au start de la page
            LoadAppOptions();
        }

        private void LoadAppOptions()
        {
            tbTeamName.Text = appOptions.CurrentTeamName;
            spMoveInterface.IsEnabled = appOptions.MoveSystemEnabledAtStart;
            moovSystemEnabled = appOptions.MoveSystemEnabledAtStart;
            cbEnableMovepool.IsChecked = appOptions.MoveSystemEnabledAtStart;
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
            WriteAppOptionsInData();
        }

        #region APP OPTIONS
        // APP OPTIONS
        private void WriteAppOptionsInData()
        {
            string appDataPath = "data/appData.json";

            // Créez le répertoire "data" s'il n'existe pas
            Directory.CreateDirectory(Path.GetDirectoryName(appDataPath));

            // Utilisez StreamWriter pour écrire le contenu dans le fichier
            using (StreamWriter writer = new StreamWriter(appDataPath))
            {
                writer.Write(JsonConvert.SerializeObject(appOptions));
            }
        }
        private void ReadAppOptionsName()
        {
            if (!File.Exists("data/appData.json")){
                appOptions = new AppOptions();
            }
            else
            {
                string optsStr = File.ReadAllText("data/appData.json");
                AppOptions opts = JsonConvert.DeserializeObject<AppOptions>(optsStr);

                appOptions = opts;
            }
        }
        #endregion APP OPTIONS

        private void tbTeamName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbTeamName.Text == "Team Name") tbTeamName.Text = "";
        }

        // REFRESH window
        public void ReSetWindowAndTeam(int index)
        {
            LoadProperties();
            RefreshWindow(index);
        }

        // REFRESH window - switchPokemon
        public void ReSetPokemonAndTeam(int index)
        {
            LoadPropertiesForOnePokemon(index);
            RefreshWindow(index);
        }

        public void RefreshWindow(int index)
        {
            teamListImageView.ItemsSource = applicationData.PokemonTeam; 
            cbTera.ItemsSource = applicationData.AllType;
            cbNature.ItemsSource = applicationData.AllNature;
            cbItem.ItemsSource = applicationData.AllItems;
            //MessageBox.Show(applicationData.AllItems.ToList().First().Name);
            teamListImageView.SelectedIndex = index; 
        }

        // BUTTON ouverture switch window
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

        #region SAVE TEAM
        // BUTTON save team
        private void SaveTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowBusy) return;
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
            appOptions.CurrentTeamName = tbTeamName.Text.ToLower();
            WriteAppOptionsInData();

        }
        #endregion SAVE TEAM

        #region LOAD TEAM
        // BUTTON load team
        private void LoadTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowBusy) return;
            string teamsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Pokemon.CHEMIN_DOSSIER);

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = teamsDirectory;
            dialog.Multiselect = false;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                string teamName = Path.GetFileName(dialog.FileName);
                appOptions.CurrentTeamName = teamName;
                tbTeamName.Text = appOptions.CurrentTeamName;
                string[] fichiersDansDossier = Directory.GetFiles(dialog.FileName);
                SwitchPokemonTeam(fichiersDansDossier);
            }
        }
        #endregion LOAD TEAM

        #region SHOWDOWN EXPORT
        // COPY team button
        private void CopyTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isWindowBusy) return;
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
            string team = "";

            foreach (Pokemon p in applicationData.PokemonTeam)
            {
                team += p.Name + " @ " + p.WantedItem;
                team += "\nAbility: " + p.WantedAbility;
                team += "\nTera Type: " + p.TeraType.ToString();
                team += "\nEVs: " + p.GetEvsTextForShowdown();
                team += "\n" + p.WantedNature.GetOnlyNatureName() + " Nature";
                if (!String.IsNullOrEmpty(p.GetIvsTextForShowdown()))
                {
                    team += "\n" + p.GetIvsTextForShowdown();
                }
                foreach (string move in p.FourMoves)
                {
                    team += "\n- " + move;
                }
                team += "\n\n";
            }
            return team;
        }
        #endregion SHOWDOWN EXPORT

        private bool canUpdate;

        // Event lors du changement (SWITCH) du pokémon sélectionné
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
            if (currentPokemon.WantedItem != null)
            {
                /*cbItem.DataContext = applicationData.AllItems;
                /*MessageBox.Show(applicationData.AllItems.First().ToString());*/
                cbItem.SelectedIndex = GetIndexOfWantedItem(currentPokemon);
            }
            UpdateShowedEVs();
            UpdateRemainingEvsText();
            // Charger les moov
            if (moovSystemEnabled)
            {
                if (currentPokemon.Moves.First().MoveGetted != null) ReloadDisplayPokemonMovepool(); // crash : vérifier moovSystemEnabled
                else { 
                    LoadCurrentPokemonMovepool();
                }
            }
            //SwitchTooltip();

            // MOVES
            sp_FourMoves.DataContext = teamListImageView.SelectedItem; // Permet de mettre à jour les 4 moves (patch behinding)
            if (getFocusedTextBox() == null) ReloadDisplayPokemonMovepool();
            canUpdate = true;
        }

        #region ABILITY
        // Ability
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
        private void cbAbility_MouseEnter(object sender, MouseEventArgs e)
        {
            SwitchTooltip();
        }
        #endregion ABILITY

        #region TERA
        //TERA
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
        #endregion TERA

        #region NATURE
        // Nature
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
        #endregion NATURE

        #region ITEM
        // ITEM
        private int GetIndexOfWantedItem(Pokemon p)
        {
            int index = applicationData.AllItems.IndexOf(applicationData.AllItems.ToList().Find(x => x.Name == p.WantedItem.Name));
            return index;
        }

        private void cbItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentPokemon == null) return;
            if (cbItem.SelectedIndex == -1) return;
            currentPokemon.WantedItem = applicationData.AllItems[cbItem.SelectedIndex];
        }
        #endregion ITEM

        #region EVs & IVs
        // EVs et IVs
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

        private void UpdateRemainingEvsText()
        {
            lbRemainingEVsValue.Content = currentPokemon.GetRemainingEvs().ToString();
        }
        #endregion EVs & IVs

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

        // Open help window
        private void miHelp_Click(object sender, RoutedEventArgs e)
        {
            WindowTips winAbout = new WindowTips(this);
            winAbout.ShowDialog();
        }

        #region MOVES
        // MOVES System
        private void cbEnableMovepool_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(cbEnableMovepool.IsChecked.ToString() + ' ' + isLoadingNewRandomTeam.ToString() + ' ' + isWindowBusy.ToString());
            if (isWindowBusy || isLoadingNewRandomTeam) { cbEnableMovepool.IsChecked = !cbEnableMovepool.IsChecked; return; }
            if (!MainPokemonCalc.IsInternetConnected()) { ShowConnexionError("You must be connected to internet"); cbEnableMovepool.IsChecked = !cbEnableMovepool.IsChecked; return; }
            moovSystemEnabled = (bool)cbEnableMovepool.IsChecked;
            spMoveInterface.IsEnabled = moovSystemEnabled;

            if (moovSystemEnabled) // Permet de le faire qu'une fois
            {
                if (currentPokemon.Moves.First().MoveGetted == null)
                {
                    lvPossibleMoves.ItemsSource = null;
                    lvPossibleMoves.Items.Clear();
                    LoadCurrentPokemonMovepool();
                }
            }

            appOptions.MoveSystemEnabledAtStart = moovSystemEnabled;
            WriteAppOptionsInData();
        }

        private async void LoadCurrentPokemonMovepool()
        {
            if (isLoadingNewRandomTeam) return;
            LoadingIcon();
            await currentPokemon.RetrievePossibleMoves();
            ReloadDisplayPokemonMovepool();
            LoadingIcon(false);
        }

        private async Task LoadPokemonsMovepool(List<Pokemon> pokemons)
        {
            foreach (Pokemon p in pokemons)
            {
                await p.RetrievePossibleMoves();
            }
            ReloadDisplayPokemonMovepool();
        }

        private async Task LoadPokemonMovepool(Pokemon p)
        {
            await p.RetrievePossibleMoves();
            ReloadDisplayPokemonMovepool();
        }

        private void ReloadDisplayPokemonMovepool(List<MoveVersion> moves = null)
        {
            if (applicationData.MovesOfThePokemon == new ObservableCollection<MoveVersion>(currentPokemon.Moves)) return;

            if(moves == null) applicationData.MovesOfThePokemon = new ObservableCollection<MoveVersion>(currentPokemon.Moves);
            else applicationData.MovesOfThePokemon = new ObservableCollection<MoveVersion>(moves);
            lvPossibleMoves.ItemsSource = applicationData.MovesOfThePokemon;
        }

        private void tb_Move_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (currentPokemon == null ||currentPokemon.Moves.First().MoveGetted == null || isSwitchingMove) return; // currentPokemon.Moves.All(mv => mv?.MoveGetted != null)

            TextBox tbMove = (TextBox)sender;

            UpdateMovepoolList(tbMove.Text);
            isSwitchingMove = false;
        }

        private void lvAcceptMove_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            MoveVersion selectedMove = (MoveVersion)lvPossibleMoves.SelectedItem;
            if (selectedMove == null) return;
            TextBox moveTb = null;
            if (getFocusedTextBox() == null) 
            {
                moveTb = getTextBoxMove();
                if (moveTb == null) {
                    if(currentMoveSlotSelected != null)
                    {
                        moveTb = currentMoveSlotSelected;
                    }
                }
            }
            else if(currentMoveSlotSelected != null)
            {
                moveTb = currentMoveSlotSelected;
            }
            if(moveTb != null)
            {
                isSwitchingMove = true;
                moveTb.Text = ToNiceString(selectedMove.MoveGetted.Name);
                moveTb.Focus();
            }

        }

        private TextBox getTextBoxMove()
        {
            if (string.IsNullOrWhiteSpace(tb_FourMove0.Text))
            {
                return tb_FourMove0;
            }
            else if (string.IsNullOrWhiteSpace(tb_FourMove1.Text))
            {
                return tb_FourMove1;
            }
            else if (string.IsNullOrWhiteSpace(tb_FourMove2.Text))
            {
                return tb_FourMove2;
            }
            else if (string.IsNullOrWhiteSpace(tb_FourMove3.Text))
            {
                return tb_FourMove3;
            }
            else return null;
        }

        private TextBox getFocusedTextBox()
        {
            if (tb_FourMove0.IsFocused)
            {
                return tb_FourMove0;
            }
            else if (tb_FourMove1.IsFocused)
            {
                return tb_FourMove1;
            }
            else if (tb_FourMove2.IsFocused)
            {
                return tb_FourMove2;
            }
            else if (tb_FourMove3.IsFocused)
            {
                return tb_FourMove3;
            }
            else return null;
        }

        private void tb_Move_GotFocus(object sender, RoutedEventArgs e)
        {
            if (currentPokemon.Moves.First().MoveGetted == null) return;
            TextBox tbMove = (TextBox)sender;

            if (getFocusedTextBox() != null && getFocusedTextBox().Text == tbMove.Text)
            {
                ReloadDisplayPokemonMovepool();
            }
            else {
                UpdateMovepoolList(tbMove.Text);         
            }

            currentMoveSlotSelected = tbMove;
            isSwitchingMove = false;
        }

        private void UpdateMovepoolList(string filterText)
        {
            List<MoveVersion> moves;
            if (filterText != null && filterText != "")
            {
                moves = currentPokemon.Moves.FindAll(x => ToNiceString(x.MoveGetted.Name).StartsWith(ToNiceString(filterText)));
                ReloadDisplayPokemonMovepool(moves);
            }
        }

        private void ActualizeFourMovesDisplay()
        {
            tb_FourMove0.Text = currentPokemon.FourMoves[0];
            tb_FourMove1.Text = currentPokemon.FourMoves[1];
            tb_FourMove2.Text = currentPokemon.FourMoves[2];
            tb_FourMove3.Text = currentPokemon.FourMoves[3];
            ReloadDisplayPokemonMovepool();
        }

        // Button reload Moovepool
        private void btn_ReloadPanel(object sender, RoutedEventArgs e)
        {
            if (isWindowBusy) return;
            ReloadDisplayPokemonMovepool();
        }

        // Button Four Moves
        private void btn_RandomMovepool(object sender, RoutedEventArgs e) // todo MAJ VISUELLE
        {
            RandomizeButtonFourMoves();
        }

        // Button
        private void btn_SmartRandomMovepool(object sender, RoutedEventArgs e)
        {
            RandomizeButtonFourMoves(true);
        }

        private void RandomizeButtonFourMoves(bool smart = false)
        {
            if (isWindowBusy) return;

            if(smart) currentPokemon.RandomizeFourMoves(true);
            else currentPokemon.RandomizeFourMoves();
            ActualizeFourMovesDisplay();
        }

        // Boutons SORT Moovepool by power
        private void tbMovePowerHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(isWindowBusy) return;
            List<MoveVersion> moves = applicationData.MovesOfThePokemon.ToList();
            if (moovSortIsPowerAsc)
            {
                moves = moves.OrderByDescending(move => move.MoveGetted.Power).ToList();
                moovSortIsPowerAsc = false;
            }
            else
            {
                moves = moves.OrderBy(move => move.MoveGetted.Power).ToList();
                moovSortIsPowerAsc = true;
            }

            ReloadDisplayPokemonMovepool(moves);
        }

        // SORT of the moves by attack type
        private void tbMoveTypeHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isWindowBusy) return;

            List<MoveVersion> moves = applicationData.MovesOfThePokemon.ToList();

            // Tri personnalisé
            moves = moves.OrderBy(move =>
            {
                // Vérifier si le mouvement appartient à l'un des types du Pokémon
                if (currentPokemon.Types.Count > 0 &&
                    (move.MoveGetted.Type.Name == currentPokemon.Types[0].Type.Name ||
                    (currentPokemon.Types.Count > 1 && currentPokemon.Types[1] != null && move.MoveGetted.Type.Name == currentPokemon.Types[1].Type.Name)))
                {
                    return 0;
                }
                else return 1;
            }).ThenBy(move => move.MoveGetted.Type.Name).ToList();

            ReloadDisplayPokemonMovepool(moves);
        }

        #endregion MOVES

        // Loading Icon
        private void LoadingIcon(bool isLoading = true)
        {
            if (isLoading) iconLoading.Visibility = Visibility.Visible;
            else iconLoading.Visibility = Visibility.Hidden;

            iconLoading.Spin = isLoading;
            isWindowBusy = isLoading;
        }

        // Randomization Options
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            randStrongPokemons = (bool)((CheckBox)sender).IsChecked;
            //MessageBox.Show(randStrongPokemons.ToString());
        }
    }
}
