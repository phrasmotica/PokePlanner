using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using PokeApiNet.Models;
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
        /// The team member slot this display represents.
        /// </summary>
        public int Slot;

        /// <summary>
        /// The settings file.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// Timer for updating the display.
        /// </summary>
        private readonly DispatcherTimer timer;

        /// <summary>
        /// The HM chart control.
        /// </summary>
        private HMChart hmChart;

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

                var mainWindow = (MainWindow) Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Loaded += (s, a) =>
                    {
                        typeChart = mainWindow.typeChart;
                        hmChart = mainWindow.optionsPanel.hmChart;
                    };
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
        /// Returns the Pokemon.
        /// </summary>
        public Pokemon Pokemon { get; set; }

        /// <summary>
        /// Returns whether the Pokemon is valid.
        /// </summary>
        public bool PokemonIsValid { get; set; }

        /// <summary>
        /// Returns the effective team member in this display.
        /// </summary>
        public TeamMember TeamMember => new TeamMember
        {
            Pokemon = Pokemon,
            IsValid = PokemonIsValid,
            Slot = Slot
        };

        /// <summary>
        /// Returns the sprite of the Pokemon being displayed.
        /// </summary>
        private ImageSource Sprite => Pokemon.GetSprite();

        /// <summary>
        /// Tries to set the display to the Pokemon named in the search box.
        /// Returns true if an update was needed.
        /// </summary>
        public async Task<bool> SetPokemon(string oldVersionGroup = null, string newVersionGroup = null)
        {
            var updated = await GetPokemon(oldVersionGroup, newVersionGroup);
            await SetDisplay(Pokemon);
            return updated;
        }

        /// <summary>
        /// Displays the Pokemon regardless of validity.
        /// </summary>
        public async Task ShowPokemon()
        {
            await SetDisplay(Pokemon);
        }

        /// <summary>
        /// Hides the Pokemon regardless of validity.
        /// </summary>
        public async Task HidePokemon()
        {
            await SetDisplay(null);
        }
        
        /// <summary>
        /// Returns true if the Pokemon can be obtained in the given version group.
        /// </summary>
        public async Task<bool> HasValidPokemon(VersionGroup versionGroup)
        {
            PokemonSpecies pokemonSpecies;
            if (Pokemon != null)
            {
                pokemonSpecies = await SessionCache.Get(Pokemon.Species);
            }
            else
            {
                pokemonSpecies = await SessionCache.Get<PokemonSpecies>(Species);
            }

            return pokemonSpecies.IsValid(versionGroup);
        }

        /// <summary>
        /// Update this display.
        /// </summary>
        private async void Update(object sender, EventArgs eventArgs)
        {
            timer.Stop();

            // set the display
            await SetPokemon();

            // set type chart row
            var row = 3 * Grid.GetRow(this) + Grid.GetColumn(this);
            typeChart.SetDefensiveMap(row, TeamMember.Pokemon);

            // update HM coverage
            var hmMoves = SessionCache.Instance.HMMoves;
            if (hmMoves != null)
            {
                var canLearn = TeamMember.Pokemon.CanLearn(hmMoves.Select(m => m.Name).ToArray());
                hmChart.SetCanLearn(row, canLearn);
                hmChart.UpdateHMCoverage(hmMoves.Count);
            }
        }

        /// <summary>
        /// Retrieve data for the Pokemon in the search box.
        /// Returns true if a new Pokemon was retrieved.
        /// </summary>
        private async Task<bool> GetPokemon(string oldVersionGroup = null, string newVersionGroup = null)
        {
            if (oldVersionGroup == newVersionGroup && Pokemon?.Name == Species)
            {
                return false;
            }
            
            if (!string.IsNullOrEmpty(Species))
            {
                try
                {
                    Console.WriteLine($@"Retrieve '{Species}'...");

                    Pokemon = await SessionCache.Get<Pokemon>(Species);
                    if (settings.restrictToVersion)
                    {
                        PokemonIsValid = await Pokemon.IsValid(SessionCache.Instance.VersionGroup);
                        if (PokemonIsValid)
                        {
                            await ShowPokemon();
                        }
                    }
                    else
                    {
                        PokemonIsValid = true;
                        await ShowPokemon();
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine(Pokemon != null
                    ? $@"Retrieved '{Pokemon.Name}'."
                    : $@"Retrieved no data for '{Species}'.");
            }

            TeamManager.Instance.Team[Slot] = TeamMember;
            return true;
        }

        /// <summary>
        /// Displays the given Pokemon.
        /// </summary>
        private async Task SetDisplay(Pokemon pokemon)
        {
            if (pokemon != null)
            {
                if (!settings.restrictToVersion || PokemonIsValid)
                {
                    var types = await pokemon.GetTypes();
                    if (types.Length > 1)
                    {
                        SetTypes(types[0], types[1]);
                    }
                    else if (types.Any())
                    {
                        SetTypes(types[0]);
                    }

                    SetTitleCase();
                    ShowSprite();
                    ToolTip = null;
                }
                else
                {
                    SetTypes(Type.Unknown);
                    SetTitleCase(false);
                    ShowSprite(false);
                    ToolTip = "Unobtainable in this game version!";
                }
            }
            else
            {
                SetTypes(Type.Unknown);
                SetTitleCase(false);
                ShowSprite(false);
                ToolTip = "Unobtainable in this game version!";
            }
        }

        /// <summary>
        /// Set the types in the display.
        /// </summary>
        private void SetTypes(Type t1, Type? t2 = null)
        {
            var prim = Types.Instance.TypeColours[t1];
            type1.Background = prim;
            if (t2 != null)
            {
                Grid.SetColumnSpan(type1, 2);
                type2.Visibility = Visibility.Visible;
                type2.Background = Types.Instance.TypeColours[t2.Value];
            }
            else
            {
                Grid.SetColumnSpan(type1, 4);
                type2.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Show or hide the Pokemon's sprite.
        /// </summary>
        private void ShowSprite(bool show = true)
        {
            if (show)
            {
                spriteImage.Source = Sprite;
                spriteBackground.Background = Brushes.White;
            }
            else
            {
                spriteImage.Source = null;
                spriteBackground.Background = TypeBrushes.Unknown;
            }
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