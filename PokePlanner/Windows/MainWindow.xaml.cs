using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using PokePlanner.Controls;
using PokePlanner.Mechanics;
using PokePlanner.Properties;

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

            // load type data
            Types.Instance.LoadTypeData();

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
        public async void UpdateTypes(string oldVersionGroup, string newVersionGroup)
        {
            typeChart.UpdateColumns();

            var updated = await UpdateTeamTypes(oldVersionGroup, newVersionGroup);
            for (var i = 0; i < updated.Length; i++)
            {
                var pokemon = AllDisplays[i].Pokemon;
                typeChart.SetDefensiveMap(i, pokemon);
            }
        }

        /// <summary>
        /// Refreshes the types of each Pokemon for the selected version group.
        /// Returns true if a Pokemon's type was updated.
        /// </summary>
        public async Task<bool[]> UpdateTeamTypes(string oldVersionGroup, string newVersionGroup)
        {
            var typesUpdated = new bool[AllDisplays.Count];

            for (var i = 0; i < AllDisplays.Count; i++)
            {
                typesUpdated[i] = await AllDisplays[i].TrySetPokemon(oldVersionGroup, newVersionGroup);
            }

            return typesUpdated;
        }

        /// <summary>
        /// Update the HM chart for the current version group.
        /// </summary>
        public void UpdateHMs()
        {
            hmChart?.UpdateHMs();
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