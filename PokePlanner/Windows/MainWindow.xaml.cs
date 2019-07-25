using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PokeAPI;
using PokePlanner.Controls;
using PokePlanner.Mechanics;
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

            // load type data
            Types.Instance.LoadTypeData();

            Loaded += OnLoaded;
        }

        /// <summary>
        /// Returns all PokemonDisplays in this window.
        /// </summary>
        private IList<PokemonDisplay> AllDisplays => new List<PokemonDisplay>
        {
            display1, display2, display3, display4, display5, display6
        };

        /// <summary>
        /// Update types in the team display and effectiveness chart.
        /// </summary>
        public async void UpdateTypes()
        {
            typeChart.UpdateColumns();

            var updated = await UpdateTeamTypes();
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
        public async Task<bool[]> UpdateTeamTypes()
        {
            var typesUpdated = new bool[AllDisplays.Count];
            var versionGroup = await DataFetcher.GetNamedApiObject<VersionGroup>(settings.versionGroup);
            
            for (var i = 0; i < AllDisplays.Count; i++)
            {
                typesUpdated[i] = await AllDisplays[i].TrySetPokemon(versionGroup);
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

        /// <summary>
        /// Update move coverage in the HM chart for the current version group.
        /// </summary>
        public void UpdateHMCoverage()
        {
            var hmMoves = SessionCache.Instance.HMMoves;
            if (hmMoves != null && hmMoves.Any())
            {
                var canLearn = new bool[Constants.NUMBER_OF_HMS];

                var team = AllDisplays.Select(d => d.Pokemon).Where(p => p != null);
                foreach (var pokemon in team)
                {
                    var moves = pokemon.Moves.Select(m => m.Move.Name).ToArray();
                    for (var i = 0; i < hmMoves.Count; i++)
                    {
                        if (moves.Contains(hmMoves[i].Name))
                        {
                            canLearn[i] = true;
                        }
                    }
                }

                hmChart.UpdateHMCoverage(canLearn);
            }
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