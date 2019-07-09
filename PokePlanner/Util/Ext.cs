using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PokeAPI;

#if DEBUG
using Type = PokePlanner.Mechanics.Type;
#endif

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
                return default;
            }

            return (T) Enum.Parse(typeof(T), st, true);
        }

        /// <summary>
        /// Returns the string in title case.
        /// </summary>
        public static string ToTitle(this string st)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(st.ToLower());
        }

        /// <summary>
        /// Adds a border to this Control.
        /// </summary>
        public static void AddBorder(this Control element)
        {
            element.BorderThickness = new Thickness(2);
            element.BorderBrush = Brushes.White;
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
        /// Returns the child element at the given position in the grid.
        /// </summary>
        public static UIElement GetChild(this Grid grid, int col, int row)
        {
            return grid.Children.Cast<UIElement>()
                       .FirstOrDefault(e => Grid.GetColumn(e) == col && Grid.GetRow(e) == row);
        }
        
        /// <summary>
        /// Creates a brush from this colour.
        /// </summary>
        public static Brush MakeBrush(this Color c)
        {
            return new SolidColorBrush(c);
        }

        /// <summary>
        /// Initialises a finite dictionary with initial key-value pairs.
        /// </summary>
        public static void Initialise<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<TKey> keys,
            TValue initValue) where TKey : Enum
        {
            foreach (var key in keys)
            {
                dict[key] = initValue;
            }
        }

        /// <summary>
        /// Returns a dictionary mapping each item to its index in the collection plus an offset.
        /// </summary>
        public static IDictionary<T, int> ToIndexMap<T>(this IEnumerable<T> items, int offset = 0)
        {
            return items.Select((item, index) => new { item, index })
                        .ToDictionary(pair => pair.item, pair => pair.index + offset);
        }

        /// <summary>
        /// Returns a new dictionary whose values are the products of the two given dictionaries.
        /// </summary>
        public static IDictionary<TKey, double> Product<TKey>(this IDictionary<TKey, double> dict1,
            IDictionary<TKey, double> dict2)
        {
            var ret = dict1.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            foreach (var kvp in dict2)
            {
                var key = kvp.Key;
                if (ret.ContainsKey(key))
                {
                    ret[key] *= kvp.Value;
                }
                else
                {
                    ret[key] = kvp.Value;
                }
            }

            return ret;
        }
    }

    /// <summary>
    /// Extensions methods for PokeAPI types.
    /// </summary>
    public static class PokeAPIExtensions
    {
        /// <summary>
        /// Returns the English title-case name of this API object.
        /// </summary>
        public static string GetName(this NamedApiObject obj)
        {
            return obj.Names.GetEnglishName();
        }

        /// <summary>
        /// Returns the name of this Pokemon.
        /// </summary>
        public static async Task<string> GetName(this Pokemon pokemon)
        {
            var species = await pokemon.Species.GetObject();
            return species.Names.GetEnglishName();
        }

        /// <summary>
        /// Returns the English name from the set of names.
        /// </summary>
        public static string GetEnglishName(this IEnumerable<ResourceName> names)
        {
            return names.FirstOrDefault(n => n.Language.Name == "en").Name;
        }

#if DEBUG
        /// <summary>
        /// Returns this Pokemon's types in the given generation.
        /// </summary>
        public static async Task<Type[]> GetPastTypes(this Pokemon pokemon, Generation generation)
        {
            var pastTypes = pokemon.PastTypes;
            var tasks = pastTypes.Select(async t => await t.Generation.GetObject());
            var pastTypeGenerations = await Task.WhenAll(tasks);

            if (pastTypeGenerations.Any())
            {
                var genNameToUse = pastTypeGenerations.SingleOrDefault(g => g.ID >= generation.ID)?.Name;
                if (genNameToUse != null)
                {
                    return pastTypes.Single(p => p.Generation.Name == genNameToUse)
                                    .Types
                                    .Select(t => t.Type.Name.ToEnum<Type>())
                                    .ToArray();
                }

                return pokemon.Types
                              .Select(t => t.Type.Name.ToEnum<Type>())
                              .ToArray();
            }

            return new Type[0];
        }
#endif

        /// <summary>
        /// Returns the name of this version group.
        /// </summary>
        public static async Task<string> GetName(this VersionGroup vg)
        {
            var versionNames = new List<string>();
            foreach (var version in vg.Versions)
            {
                var gv = await version.GetObject();
                versionNames.Add(gv.GetName());
            }

            return string.Join("/", versionNames);
        }
    }
}