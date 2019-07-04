using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        /// Create type columns.
        /// </summary>
        public TypeChart()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                CreateTypeColumns();
            }
        }

        /// <summary>
        /// Creates columns in the grid for each type.
        /// </summary>
        private void CreateTypeColumns()
        {
            var types = Types.ConcreteTypes;
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
    }
}