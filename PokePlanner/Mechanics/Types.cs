using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using PokeAPI;
using PokePlanner.Util;

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
        /// Internal storage for Type effectiveness.
        /// </summary>
        private Dictionary<Type, Dictionary<Type, double>> Eff;

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
            LoadTypeData();
        }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static Types Instance { get; } = new Types();

        /// <summary>
        /// Reads Type data into Eff.
        /// </summary>
        private void LoadTypeData()
        {
            var typeNames = Enum.GetValues(typeof(Type)).Cast<Type>()
                                                        .Select(t => t.ToString().ToLower());
            var typeArray = new List<PokemonType>();
            Parallel.ForEach(typeNames, async name =>
            {
                Console.WriteLine($@"Getting {name} type data...");
                var typeObj = await DataFetcher.GetNamedApiObject<PokemonType>(name);
                Console.WriteLine($@"Got {name} type data.");

                typeArray.Add(typeObj);
            });

            Eff = new Dictionary<Type, Dictionary<Type, double>>();
            foreach (var type in typeArray)
            {
                // initialise dictionary for this type
                var typeName = type.Name.ToEnum<Type>();
                Eff[typeName] = new Dictionary<Type, double>();

                // populate damage relations for this type
                var damageRelations = type.DamageRelations;

                foreach (var x in damageRelations.DoubleDamageTo)
                {
                    var typeTo = x.Name.ToEnum<Type>();
                    Eff[typeName].Add(typeTo, 2);
                }

                foreach (var x in damageRelations.HalfDamageTo)
                {
                    var typeTo = x.Name.ToEnum<Type>();
                    Eff[typeName].Add(typeTo, 0.5);
                }

                foreach (var x in damageRelations.NoDamageTo)
                {
                    var typeTo = x.Name.ToEnum<Type>();
                    Eff[typeName].Add(typeTo, 0);
                }
            }
        }

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
        /// Returns the effectiveness of a move of a given type on a single-typed Pokémon.
        /// </summary>
        public double GetEff(Type offType, Type defType)
        {
            if (!Eff.ContainsKey(offType))
            {
                return 1;
            }

            var offDict = Eff[offType];
            if (!offDict.ContainsKey(defType))
            {
                return 1;
            }

            return offDict[defType];
        }

        /// <summary>
        /// Returns the effectiveness of a move of a given type on a dual-typed Pokémon.
        /// </summary>
        public double GetEff(Type offType, Type defType1, Type defType2)
        {
            return GetEff(offType, defType1) * GetEff(offType, defType2);
        }

        /// <summary>
        /// Returns a solid colour brush of the supplied hex value.
        /// </summary>
        private static SolidColorBrush CreateBrush(string hex)
        {
            var converted = ColorConverter.ConvertFromString(hex);
            return converted == null ? Brushes.Black : new SolidColorBrush((Color) converted);
        }

        /// <summary>
        /// Returns a brush for the given string representing a type.
        /// </summary>
        public Brush GetTypeBrush(string type)
        {
            return GetTypeBrush(type.ToEnum<Type>());
        }

        /// <summary>
        /// Returns a brush for the given type.
        /// </summary>
        public Brush GetTypeBrush(Type type)
        {
            return TypeColours.ContainsKey(type) ? TypeColours[type] : null;
        }
    }
}
