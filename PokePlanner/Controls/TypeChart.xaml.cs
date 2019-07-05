using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PokeAPI;
using PokePlanner.Mechanics;
using PokePlanner.Util;
using Type = PokePlanner.Mechanics.Type;

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
                    new Label
                    {
                        Content = "-",
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Foreground = Brushes.Black,
                        FontSize = 16
                    }.AddToGrid(grid, i, j);
                }

                // weakness count label
                new Label
                {
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.Black,
                    Background = GetEffBrush(1),
                    Content = 0
                }.AddToGrid(grid, i, 7);

                // resistance count label
                new Label
                {
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.Black,
                    Background = GetEffBrush(1),
                    Content = 0
                }.AddToGrid(grid, i, 8);
            }
        }

        /// <summary>
        /// Set the defensive effectivenesses of the Pokemon in the given row of the chart.
        /// </summary>
        public void SetDefensiveMap(int row, Pokemon pokemon)
        {
            var pokemonLabel = (Label) grid.GetChild(0, row + 1);
            if (pokemon == null)
            {
                pokemonLabel.Content = "-";
                RemoveDefensiveMap(row);
            }
            else
            {
                pokemonLabel.Content = pokemon.Name.ToTitle();

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
                }
                else
                {
                    effLabel.FontWeight = FontWeights.Normal;
                    effLabel.Content = "-";
                    effLabel.Background = Brushes.White;
                }

                // count up weaknesses and resistances
                var typeEffs = pokemonEff.Select(d => d?[type] ?? 1).ToArray();

                var totalWeak = typeEffs.Count(x => x > 1);
                var weakLabel = (Label) grid.GetChild(col, 7);
                weakLabel.Content = totalWeak;
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