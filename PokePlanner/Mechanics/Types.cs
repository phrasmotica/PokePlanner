using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace PokePlanner.Mechanics
{
    /// <summary>
    /// Enumeration of Pokemon types.
    /// </summary>
    public enum Type
    {
        Unknown,
        Normal, Fighting, Flying, Poison, Ground, Rock, Bug, Ghost, Steel,
        Fire, Water, Grass, Electric, Psychic, Ice, Dragon, Dark, Fairy,
        Shadow
    }

    /// <summary>
    /// For Type-related information and calculations.
    /// </summary>
    public class Types
    {
        /// <summary>
        /// Map from a type to its colour.
        /// </summary>
        public Dictionary<Type, Brush> TypeColours = new Dictionary<Type, Brush>(Enum.GetValues(typeof(Type)).Length);

        /// <summary>
        /// Private constructor.
        /// </summary>
        private Types()
        {
            SetTypeColours();
        }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static Types Instance { get; } = new Types();

        /// <summary>
        /// Sets colours for each type.
        /// </summary>
        private void SetTypeColours()
        {
            TypeColours[Type.Normal] = CreateBrush("#A8A878");
            TypeColours[Type.Fighting] = CreateBrush("#C03028");
            TypeColours[Type.Flying] = CreateBrush("#A890F0");
            TypeColours[Type.Poison] = CreateBrush("#A040A0");
            TypeColours[Type.Ground] = CreateBrush("#E0C068");
            TypeColours[Type.Rock] = CreateBrush("#B8A038");
            TypeColours[Type.Bug] = CreateBrush("#A8B820");
            TypeColours[Type.Ghost] = CreateBrush("#705898");
            TypeColours[Type.Steel] = CreateBrush("#B8B8D0");
            TypeColours[Type.Fire] = CreateBrush("#F08030");
            TypeColours[Type.Water] = CreateBrush("#6890F0");
            TypeColours[Type.Grass] = CreateBrush("#78C850");
            TypeColours[Type.Electric] = CreateBrush("#F8D030");
            TypeColours[Type.Psychic] = CreateBrush("#F85888");
            TypeColours[Type.Ice] = CreateBrush("#98D8D8");
            TypeColours[Type.Dragon] = CreateBrush("#7038F8");
            TypeColours[Type.Dark] = CreateBrush("#705848");
            TypeColours[Type.Fairy] = CreateBrush("#EE99AC");
            TypeColours[Type.Unknown] = CreateBrush("#68A090");
        }

        /// <summary>
        /// Returns a solid colour brush of the supplied hex value.
        /// </summary>
        private static SolidColorBrush CreateBrush(string hex)
        {
            var converted = ColorConverter.ConvertFromString(hex);
            return converted == null ? Brushes.Black : new SolidColorBrush((Color) converted);
        }
    }
}
