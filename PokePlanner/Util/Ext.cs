using System;
using System.Windows;
using System.Windows.Controls;

namespace PokePlanner.Util
{
    /// <summary>
    /// Support class for extension methods.
    /// </summary>
    public static class Ext
    {
        /// <summary>
        /// Returns the string as an enum value.
        /// </summary>
        public static T ToEnum<T>(this string st)
        {
            if (string.IsNullOrEmpty(st))
            {
                return default(T);
            }

            return (T) Enum.Parse(typeof(T), st, true);
        }

        /// <summary>
        /// Adds a Control to the given Grid in the given cell.
        /// </summary>
        public static void AddToGrid(this UIElement element, Grid grid, int col, int row)
        {
            Grid.SetColumn(element, col);
            Grid.SetRow(element, row);
            grid.Children.Add(element);
        }

        /// <summary>
        /// Adds a Control to the given Grid in the given cell with the given spans.
        /// </summary>
        public static void AddToGrid(this UIElement element, Grid grid, int col, int row, int colspan, int rowspan)
        {
            Grid.SetColumnSpan(element, colspan);
            Grid.SetRowSpan(element, rowspan);
            AddToGrid(element, grid, col, row);
        }
    }
}
