using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PokeAPI;
using PokePlanner.Mechanics;
using PokePlanner.Util;

namespace PokePlanner.Controls
{
    /// <summary>
    /// Interaction logic for TypeChart.xaml.
    /// </summary>
    public partial class TypeChart
    {
        /// <summary>
        /// Effectiveness data for all Pokemon.
        /// </summary>
        private readonly IDictionary<Type, double>[] pokemonEff;

        /// <summary>
        /// Current Pokemon names.
        /// </summary>
        private readonly string[] pokemonNames;

        /// <summary>
        /// Map from a type to its column in the chart.
        /// </summary>
        private IDictionary<Type, int> typeColumns;

        /// <summary>
        /// Create type columns.
        /// </summary>
        public TypeChart()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                CreatePokemonLabels();
                CreateTypeColumns();

                pokemonEff = new IDictionary<Type, double>[6];
                pokemonNames = new string[6];
            }
        }

        /// <summary>
        /// Creates a label for each Pokemon in the grid's leftmost column.
        /// </summary>
        private void CreatePokemonLabels()
        {
            for (var i = 1; i <= 6; i++)
            {
                // create label for pokemon
                new Label
                {
                    Content = "-",
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Background = Brushes.White
                }.AddToGrid(grid, 0, i);
            }
        }

        /// <summary>
        /// Creates columns in the grid for each type.
        /// </summary>
        private void CreateTypeColumns()
        {
            var types = Types.ConcreteTypes;
            typeColumns = types.ToIndexMap(1);

            for (var i = 1; i <= types.Count; i++)
            {
                // create column for type
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                // create label for type name
                var type = types[i - 1];
                new Label
                {
                    Content = type,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Background = Types.Instance.GetTypeBrush(type)
                }.AddToGrid(grid, i, 0);

                // create labels for each row
                for (int j = 1; j < grid.RowDefinitions.Count - 2; j++)
                {
                    var effLabel = new Label
                    {
                        Content = "-",
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Foreground = Brushes.Black,
                        FontSize = 16
                    };
                    ToolTipService.SetInitialShowDelay(effLabel, 1000);
                    effLabel.AddToGrid(grid, i, j);
                }

                // weakness count label
                var weakLabel = new Label
                {
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.Black,
                    Background = GetEffBrush(1),
                    Content = 0
                };
                ToolTipService.SetInitialShowDelay(weakLabel, 1000);
                weakLabel.AddToGrid(grid, i, 7);

                // resistance count label
                var resistLabel = new Label
                {
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.Black,
                    Background = GetEffBrush(1),
                    Content = 0
                };
                ToolTipService.SetInitialShowDelay(resistLabel, 1000);
                resistLabel.AddToGrid(grid, i, 8);
            }
        }

        /// <summary>
        /// Set the defensive effectivenesses of the Pokemon in the given row of the chart.
        /// </summary>
        public void SetDefensiveMap(int row, Pokemon pokemon)
        {
            pokemonNames[row] = pokemon.GetName();
            var pokemonLabel = (Label) grid.GetChild(0, row + 1);
            if (pokemon == null)
            {
                pokemonLabel.Content = "-";
                RemoveDefensiveMap(row);
            }
            else
            {
                pokemonLabel.Content = pokemon.GetName();

                var types = pokemon.Types.Select(t => t.Type.Name.ToEnum<Type>()).ToArray();
                if (types.Length > 1)
                {
                    SetDefensiveMap(row, types[0], types[1]);
                }
                else
                {
                    SetDefensiveMap(row, types[0]);
                }
            }
        }

        /// <summary>
        /// Set the defensive effectivenesses of the types in the given row of the chart.
        /// </summary>
        public void SetDefensiveMap(int row, Type type1, Type? type2 = null)
        {
            if (type2 != null)
            {
                pokemonEff[row] = Types.Instance.GetEffDict(type1, type2.Value);
            }
            else
            {
                pokemonEff[row] = Types.Instance.GetEffDict(type1);
            }

            UpdateRow(row);
        }

        /// <summary>
        /// Removes the defensive effectivenesses in the given row of the chart.
        /// </summary>
        public void RemoveDefensiveMap(int row)
        {
            pokemonEff[row] = null;
            UpdateRow(row);
        }

        /// <summary>
        /// Updates the given row of the chart.
        /// </summary>
        private void UpdateRow(int row)
        {
            var effMap = pokemonEff[row];
            foreach (var type in Types.ConcreteTypes)
            {
                var col = typeColumns[type];
                var effLabel = (Label) grid.GetChild(col, row + 1);

                var eff = effMap?[type];
                if (eff.HasValue)
                {
                    effLabel.FontWeight = eff > 2 || eff < 0.5 ? FontWeights.Bold : FontWeights.Normal;
                    effLabel.Content = GetEffDescription(eff.Value);
                    effLabel.Background = GetEffBrush(eff.Value);

                    var name = pokemonNames[row];
                    var desc = eff > 1 ? "super effective" : eff < 1 ? "not very effective" : "normal";
                    effLabel.ToolTip = $"{name} takes {desc} damage from {type}-type moves";
                }
                else
                {
                    effLabel.FontWeight = FontWeights.Normal;
                    effLabel.Content = "-";
                    effLabel.Background = Brushes.White;
                    effLabel.ToolTip = null;
                }

                // count up weaknesses and resistances
                var typeEffs = pokemonEff.Select(d => d?[type] ?? 1).ToArray();

                var totalWeak = typeEffs.Count(x => x > 1);
                var weakLabel = (Label) grid.GetChild(col, 7);
                weakLabel.Content = totalWeak;
                weakLabel.ToolTip = $"{totalWeak} Pokemon weak to {type}-type moves";
                if (totalWeak > 3)
                {
                    weakLabel.FontWeight = FontWeights.Bold;
                    weakLabel.Background = GetEffBrush(2);
                }
                else
                {
                    weakLabel.FontWeight = FontWeights.Normal;
                    weakLabel.Background = GetEffBrush(0.5);
                }

                var totalResist = typeEffs.Count(x => x < 1);
                var resistLabel = (Label) grid.GetChild(col, 8);
                resistLabel.Content = totalResist;
                resistLabel.ToolTip = $"{totalResist} Pokemon resistant to {type}-type moves";
                if (totalResist < 1)
                {
                    resistLabel.FontWeight = FontWeights.Bold;
                    resistLabel.Background = GetEffBrush(2);
                }
                else
                {
                    resistLabel.FontWeight = FontWeights.Normal;
                    resistLabel.Background = GetEffBrush(0.5);
                }
            }
        }

        /// <summary>
        /// Returns a textual description of the effectiveness multiplier.
        /// </summary>
        private static string GetEffDescription(double eff)
        {
            return eff.ToString("0.##'x'");
        }

        /// <summary>
        /// Returns a brush for the given effectiveness multiplier.
        /// </summary>
        private static SolidColorBrush GetEffBrush(double mult)
        {
            if (mult == 0)
            {
                // grey
                return "#DBDBDB".ToBrush();
            }

            if (mult == 1)
            {
                // blue
                return "#BAE8FF".ToBrush();
            }

            if (mult > 1)
            {
                // red
                return "#FFA1A1".ToBrush();
            }

            if (mult < 1)
            {
                // green
                return "#C7FFC7".ToBrush();
            }

            return null;
        }
    }
}