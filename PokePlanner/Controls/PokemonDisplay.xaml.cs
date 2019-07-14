using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using PokeAPI;
using PokePlanner.Mechanics;
using PokePlanner.Properties;
using PokePlanner.Util;
using Type = PokePlanner.Mechanics.Type;

namespace PokePlanner.Controls
{
    /// <summary>
    /// Interaction logic for PokemonDisplay.xaml.
    /// </summary>
    public partial class PokemonDisplay
    {
        /// <summary>
        /// Time in seconds after last content change before performing the search.
        /// </summary>
        public const int SEARCH_DELAY = 1;

        /// <summary>
        /// Timer for updating the display.
        /// </summary>
        private readonly DispatcherTimer timer;

        /// <summary>
        /// The main window.
        /// </summary>
        private readonly MainWindow mainWindow;

        /// <summary>
        /// The settings file.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// Whether the display should be updated after the search box text changes.
        /// </summary>
        private bool shouldUpdate = true;

        /// <summary>
        /// The type chart control.
        /// </summary>
        private TypeChart typeChart;

        /// <summary>
        /// Initialise types.
        /// </summary>
        public PokemonDisplay()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                SetTypes(Type.Unknown);

                mainWindow = (MainWindow) Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Loaded += (s, a) => FindTypeChart();
                }

                settings = Settings.Default;

                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(SEARCH_DELAY)
                };
                timer.Tick += Update;
            }
        }

        /// <summary>
        /// The display's species.
        /// </summary>
        public string Species
        {
            get => searchBox.Text.ToLower();
            set { searchBox.Text = value; }
        }

        /// <summary>
        /// Creates a reference to the type chart.
        /// </summary>
        private void FindTypeChart()
        {
            typeChart = mainWindow.typeChart;
        }

        /// <summary>
        /// Set the types in the display.
        /// </summary>
        public void SetTypes(Type t1, Type? t2 = null)
        {
            var prim = Types.Instance.TypeColours[t1];
            type1.Background = prim;
            if (t2 != null)
            {
                Grid.SetColumnSpan(type1, 1);
                type2.Visibility = Visibility.Visible;
                type2.Background = Types.Instance.TypeColours[t2.Value];
            }
            else
            {
                Grid.SetColumnSpan(type1, 2);
                type2.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Update this display.
        /// </summary>
        private async void Update(object sender, EventArgs eventArgs)
        {
            timer.Stop();

            // get pokemon data
            var pokemon = await Retrieve();

            // set types
            if (pokemon != null)
            {
#if DEBUG
                var versionGroup = await DataFetcher.GetNamedApiObject<VersionGroup>(settings.versionGroup);
                var types = await pokemon.GetTypes(versionGroup);
#else
                var types = pokemon.GetTypes();
#endif

                if (types.Length > 1)
                {
                    SetTypes(types[0], types[1]);
                }
                else
                {
                    SetTypes(types[0]);
                }

                // make name title case without updating display
                SetTitleCase();
            }
            else
            {
                SetTypes(Type.Unknown);
            }

            // set type chart row
            var row = 3 * Grid.GetRow(this) + Grid.GetColumn(this);
            typeChart.SetDefensiveMap(row, pokemon);
        }

        /// <summary>
        /// Retrieve data for the Pokemon in the text box.
        /// </summary>
        private async Task<Pokemon> Retrieve()
        {
            if (!string.IsNullOrEmpty(Species))
            {
                try
                {
                    Console.WriteLine($@"Retrieve '{Species}'...");
                    var pokemon = await TryGetPokemon(Species);

                    Console.WriteLine(pokemon != null
                        ? $@"Retrieved '{pokemon.Name}'."
                        : $@"Retrieved no data for '{Species}'.");

                    return pokemon;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the Pokemon if it's in the local dex of the selected version group.
        /// Otherwise returns null.
        /// </summary>
        private async Task<Pokemon> TryGetPokemon(string species)
        {
            var pokemon = await DataFetcher.GetNamedApiObject<Pokemon>(species);
            var versionGroup = settings.versionGroup;

            var tasks = pokemon.GameIndices.Select(async gi => await gi.Version.GetObject());
            var versions = await Task.WhenAll(tasks);
            var valid = versions.Any(version => version.VersionGroup.Name == versionGroup);

            return valid ? pokemon : null;
        }

        /// <summary>
        /// Returns the Pokemon if it's in the local dex of the selected version group.
        /// Otherwise returns null.
        /// </summary>
        public async Task<bool> TrySetPokemon(VersionGroup versionGroup)
        {
            var pokemon = await TryGetPokemon(Species);
            if (pokemon != null)
            {
                var types = await pokemon.GetTypes(versionGroup);
                if (types.Length > 1)
                {
                    SetTypes(types[0], types[1]);
                }
                else if (types.Any())
                {
                    SetTypes(types[0]);
                }

                SetTitleCase();
                return true;
            }
            
            SetTypes(Type.Unknown);
            SetTitleCase(false);
            return false;
        }

        /// <summary>
        /// Sets the search box content to be in title case or lower case without triggering an update.
        /// </summary>
        private void SetTitleCase(bool isTitleCase = true)
        {
            var idx = searchBox.CaretIndex;
            shouldUpdate = false;

            searchBox.Text = isTitleCase ? searchBox.Text.ToTitle() : searchBox.Text.ToLower();

            shouldUpdate = true;
            searchBox.CaretIndex = idx;
        }

        /// <summary>
        /// Reset search timer whenever the text changes.
        /// </summary>
        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (shouldUpdate)
            {
                timer.Stop();
                timer.Start();
            }
        }
    }
}