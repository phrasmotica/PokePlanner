using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using PokePlanner.Controls;
using PokePlanner.Properties;
using PokePlanner.Util;

namespace PokePlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// The settings file.
        /// </summary>
        private Settings settings;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < Constants.TEAM_SIZE; i++)
            {
                AllDisplays[i].Slot = i;
            }

            Loaded += OnLoaded;
        }

        /// <summary>
        /// Returns all PokemonDisplays in this window.
        /// </summary>
        public IList<PokemonDisplay> AllDisplays => new List<PokemonDisplay>
        {
            display1, display2, display3, display4, display5, display6
        };

        /// <summary>
        /// Update types in the team display and effectiveness chart.
        /// </summary>
        public async Task ValidateTeam(string newVersionGroup)
        {
            for (var slot = 0; slot < AllDisplays.Count; slot++)
            {
                var display = AllDisplays[slot];

                var valid = await TeamManager.Instance.Validate(slot, newVersionGroup);
                display.PokemonIsValid = valid;
                if (valid)
                {
                    await display.ShowPokemon();
                }
                else
                {
                    await display.HidePokemon();
                }

                typeChart.SetDefensiveMap(slot, TeamManager.Instance.Team[slot].Pokemon);
            }
        }

        /// <summary>
        /// Update types in the team display and effectiveness chart.
        /// </summary>
        public async void UpdateTypes(string oldVersionGroup, string newVersionGroup)
        {
            typeChart.UpdateColumns();

            var updated = await UpdateTeamTypes(oldVersionGroup, newVersionGroup);
            for (var slot = 0; slot < updated.Length; slot++)
            {
                typeChart.SetDefensiveMap(slot, TeamManager.Instance.Team[slot].Pokemon);
            }
        }

        /// <summary>
        /// Refreshes the types of each Pokemon for the selected version group.
        /// Returns true if a Pokemon's type was updated.
        /// </summary>
        public async Task<bool[]> UpdateTeamTypes(string oldVersionGroup, string newVersionGroup)
        {
            var typesUpdated = new bool[Constants.TEAM_SIZE];

            for (var i = 0; i < Constants.TEAM_SIZE; i++)
            {
                typesUpdated[i] = await AllDisplays[i].SetPokemon(oldVersionGroup, newVersionGroup);
            }

            return typesUpdated;
        }

        /// <summary>
        /// Update the HM chart for the current version group.
        /// </summary>
        public void UpdateHMs()
        {
            optionsPanel.hmChart?.UpdateHMs();
        }

        protected void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // set settings file
            settings = Settings.Default;

            LoadTeam();

            // set minimum window size
            MinWidth = Width;
            MinHeight = Height;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (IsLoaded)
            {
                SaveTeam();
            }
        }

        /// <summary>
        /// Load the saved team.
        /// </summary>
        private void LoadTeam()
        {
            Console.WriteLine(@"Loading saved team...");

            display1.Species = settings.pokemon1;
            display2.Species = settings.pokemon2;
            display3.Species = settings.pokemon3;
            display4.Species = settings.pokemon4;
            display5.Species = settings.pokemon5;
            display6.Species = settings.pokemon6;

            Console.WriteLine(@"Saved team loaded.");
        }

        /// <summary>
        /// Save the current team.
        /// </summary>
        private void SaveTeam()
        {
            Console.WriteLine(@"Saving current team...");

            settings.pokemon1 = display1.Species;
            settings.pokemon2 = display2.Species;
            settings.pokemon3 = display3.Species;
            settings.pokemon4 = display4.Species;
            settings.pokemon5 = display5.Species;
            settings.pokemon6 = display6.Species;
            settings.Save();

            Console.WriteLine(@"Current team saved.");
        }
    }
}