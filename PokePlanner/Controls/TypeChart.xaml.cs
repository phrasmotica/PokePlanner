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
                CreateTypeColumns();

                pokemonEff = new IDictionary<Type, double>[6];
            }
        }

        /// <summary>
        /// Creates columns in the grid for each type.
        /// </summary>
        private void CreateTypeColumns()
        {
            var types = Types.ConcreteTypes;
            typeColumns = types.ToIndexMap();

            for (var i = 0; i < types.Count; i++)
            {
                // create column for type
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    MinWidth = 60
                });

                // create label for type
                var type = types[i];
                var label = new Label
                {
                    Content = type,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Background = Types.Instance.GetTypeBrush(type)
                };

                // add label to grid's header row
                label.AddToGrid(grid, i, 0);

                // create labels for each row
                for (int j = 1; j < grid.RowDefinitions.Count - 1; j++)
                {
                    new Label
                    {
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Foreground = Brushes.White
                    }.AddToGrid(grid, i, j);
                }
            }
        }

        /// <summary>
        /// Set the defensive effectivenesses of the Pokemon in the given row of the chart.
        /// </summary>
        public void SetDefensiveMap(int row, Pokemon pokemon)
        {
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
        /// Updates the given row of the chart.
        /// </summary>
        private void UpdateRow(int row)
        {
            foreach (var kvp in pokemonEff[row])
            {
                var col = typeColumns[kvp.Key];
                var label = (Label) grid.GetChild(col, row + 1);

                var eff = kvp.Value;
                label.Content = GetEffDescription(eff);
                label.Background = GetEffBrush(eff);
            }
        }

        /// <summary>
        /// Returns a textual description of the effectiveness multiplier.
        /// </summary>
        private static string GetEffDescription(double eff)
        {
            return eff == 0 ? "immune" : eff.ToString("0.##'x'");
        }

        /// <summary>
        /// Returns a brush for the given effectiveness multiplier.
        /// </summary>
        private static SolidColorBrush GetEffBrush(double mult)
        {
            if (mult == 0)
            {
                // grey
                return "#787878".ToBrush();
            }

            if (mult == 1)
            {
                // blue
                return "#00AAFF".ToBrush();
            }

            if (mult > 1)
            {
                // red
                return "#FF0000".ToBrush();
            }

            if (mult < 1)
            {
                // green
                return "#008000".ToBrush();
            }

            return null;
        }
    }
}