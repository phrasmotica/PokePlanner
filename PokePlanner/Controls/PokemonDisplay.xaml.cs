using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using PokeAPI;
using PokePlanner.Mechanics;
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
        /// The type chart control.
        /// </summary>
        private TypeChart typeChart;

        /// <summary>
        /// Initialise types.
        /// </summary>
        public PokemonDisplay()
        {
            InitializeComponent();
            SetTypes(Type.Unknown);

            mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.Loaded += (s, a) => FindTypeChart();

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(SEARCH_DELAY)
            };
            timer.Tick += Update;
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
            type1.Fill = prim;
            type2.Fill = t2 == null ? prim : Types.Instance.TypeColours[t2.Value];
        }

        /// <summary>
        /// Update this display.
        /// </summary>
        private async void Update(object sender, EventArgs eventArgs)
        {
            timer.IsEnabled = false;

            // get pokemon data
            var pokemon = await Retrieve();
            var name = pokemon?.Name ?? "nothing";
            Console.WriteLine($@"Retrieved '{name}'.");

            // set types
            if (pokemon != null)
            {
                var types = pokemon.Types.OrderBy(t => t.Slot)
                                   .Select(t => t.Type.Name.ToEnum<Type>())
                                   .ToArray();
                if (types.Length > 1)
                {
                    SetTypes(types[0], types[1]);
                }
                else
                {
                    SetTypes(types[0]);
                }

                // set type chart row
                var row = 3 * Grid.GetRow(this) + Grid.GetColumn(this);
                typeChart.SetDefensiveMap(row, pokemon);
            }
            else
            {
                SetTypes(Type.Unknown);
            }
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
                    return await DataFetcher.GetNamedApiObject<Pokemon>(Species);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return null;
        }

        /// <summary>
        /// Reset search timer whenever the text changes.
        /// </summary>
        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            timer.Stop();

            timer.IsEnabled = true;
            timer.Start();
        }
    }
}