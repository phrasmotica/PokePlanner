using System;
using System.Windows;
using PokePlanner.Mechanics;
using PokePlanner.Properties;

#if DEBUG
using System.Collections.Generic;
using System.Linq;
using PokeAPI;
using PokePlanner.Controls;
using PokePlanner.Util;
#endif

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

#if DEBUG
        /// <summary>
        /// Returns all PokemonDisplays in this window.
        /// </summary>
        private IEnumerable<PokemonDisplay> AllDisplays => new HashSet<PokemonDisplay>
        {
            display1, display2, display3, display4, display5, display6
        };

        /// <summary>
        /// Refreshes the types of each Pokemon for the selected version group.
        /// </summary>
        public async void UpdateTeamTypes(string versionGroupName)
        {
            var versionGroup = await DataFetcher.GetNamedApiObject<VersionGroup>(versionGroupName);
            var generation = await versionGroup.Generation.GetObject();

            foreach (var display in AllDisplays)
            {
                var pokemon = await DataFetcher.GetNamedApiObject<Pokemon>(display.Species);
                var pastTypes = await pokemon.GetPastTypes(generation);
                if (pastTypes.Length > 1)
                {
                    display.SetTypes(pastTypes[0], pastTypes[1]);
                }
                else if (pastTypes.Any())
                {
                    display.SetTypes(pastTypes[0]);
                }
            }
        }
#endif

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

            SaveTeam();
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