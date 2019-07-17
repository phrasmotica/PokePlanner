using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PokeAPI;
using PokePlanner.Mechanics;
using Type = PokePlanner.Mechanics.Type;

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
        /// Returns the number of rows in the grid.
        /// </summary>
        public static int RowCount(this Grid grid) => grid.RowDefinitions.Count;
        
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
        /// Returns a type set from a list of types with a column index offset.
        /// </summary>
        public static TypeSet ToTypeSet(this IEnumerable<Type> items, bool active = false, int offset = 0)
        {
            var typeSet = new TypeSet();
            foreach (var item in items.Select((type, index) => new { type, index }))
            {
                typeSet[item.type] = new ActiveInfo(active, item.index + offset);
            }

            return typeSet;
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
        /// Returns this Pokemon's type data for the given generation, if any.
        /// </summary>
        public static async Task<Type[]> GetPastTypes(this Pokemon pokemon, Generation generation)
        {
            var pastTypes = pokemon.PastTypes;
            var tasks = pastTypes.Select(async t => await t.Generation.GetObject());
            var pastTypeGenerations = await Task.WhenAll(tasks);

            if (pastTypeGenerations.Any())
            {
                var genNameToUse = pastTypeGenerations.SingleOrDefault(g => g.ID >= generation.ID)?.Name;
                if (!string.IsNullOrEmpty(genNameToUse))
                {
                    return pastTypes.Single(p => p.Generation.Name == genNameToUse)
                                    .Types
                                    .ToTypes();
                }

                return pokemon.GetCurrentTypes();
            }

            return new Type[0];
        }
#endif

        /// <summary>
        /// Returns this Pokemon's types in the given generation.
        /// </summary>
#if DEBUG
        public static async Task<Type[]> GetTypes(this Pokemon pokemon, VersionGroup versionGroup)
        {
            var generation = await versionGroup.Generation.GetObject();
            var pastTypes = await pokemon.GetPastTypes(generation);
            return pastTypes.Any() ? pastTypes : pokemon.GetCurrentTypes();
        }
#else
        public static Type[] GetTypes(this Pokemon pokemon)
        {
            return pokemon.GetCurrentTypes();
        }
#endif

        /// <summary>
        /// Returns this Pokemon's latest type data.
        /// </summary>
        public static Type[] GetCurrentTypes(this Pokemon pokemon)
        {
            return pokemon.Types.ToTypes();
        }

        /// <summary>
        /// Returns this type map as an array of Type enum values.
        /// </summary>
        public static Type[] ToTypes(this IEnumerable<PokemonTypeMap> typeMap)
        {
            return typeMap.Select(t => t.Type.Name.ToEnum<Type>())
                          .ToArray();
        }

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

        /// <summary>
        /// Returns true if this generation uses the given type.
        /// </summary>
        public static async Task<bool> HasType(this Generation generation, Type type)
        {
            var typeObj = await DataFetcher.GetNamedApiObject<PokemonType>(type.ToString().ToLower());
            var generationIntroduced = await typeObj.Generation.GetObject();
            return generationIntroduced.ID <= generation.ID;
        }
    }
}