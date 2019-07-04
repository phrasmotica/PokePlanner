﻿using System;
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
        /// Internal storage for defensive Type effectiveness.
        /// Keys are defensive types, values are offensive types mapped to their effectivenesses against the key.
        /// </summary>
        private Dictionary<Type, Dictionary<Type, double>> DefensiveEff;

        /// <summary>
        /// Map from a type to its colour.
        /// </summary>
        public Dictionary<Type, Brush> TypeColours = new Dictionary<Type, Brush>(AllTypes.Count);

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
        /// Returns an array of all types.
        /// </summary>
        public static List<Type> AllTypes => Enum.GetValues(typeof(Type)).Cast<Type>().ToList();

        /// <summary>
        /// Returns an array of all types a Pokemon can have.
        /// </summary>
        public static List<Type> PokemonTypes => AllTypes.Where(t => t != Type.Unknown).ToList();

        /// <summary>
        /// Returns an array of all types a Move can have.
        /// </summary>
        public static List<Type> MoveTypes => AllTypes.Where(t => t != Type.Shadow).ToList();

        /// <summary>
        /// Returns an array of all concrete types.
        /// </summary>
        public static List<Type> ConcreteTypes => AllTypes.Where(t => t != Type.Shadow && t != Type.Unknown).ToList();

        /// <summary>
        /// Reads Type data into the defensive effectiveness map.
        /// </summary>
        public async void LoadTypeData()
        {
            DefensiveEff = new Dictionary<Type, Dictionary<Type, double>>();
            
            var tasks = ConcreteTypes.Select(async defType =>
            {
                // retrieve type object from PokeAPI
                var typeName = defType.ToString().ToLower();
                Console.WriteLine($@"Getting {typeName} type data...");
                var typeObj = await DataFetcher.GetNamedApiObject<PokemonType>(typeName);
                Console.WriteLine($@"Got {typeName} type data.");
            
                // now set its defensive effectivenesses
                Console.WriteLine($@"Setting defensive {typeName} effectiveness data...");

                // initialise dictionary
                DefensiveEff[defType] = new Dictionary<Type, double>();
                DefensiveEff[defType].Initialise(ConcreteTypes, 1);

                // populate defensive damage relations
                var damageRelations = typeObj.DamageRelations;

                foreach (var x in damageRelations.DoubleDamageFrom)
                {
                    var typeFrom = x.Name.ToEnum<Type>();
                    DefensiveEff[defType][typeFrom] = 2;
                }

                foreach (var x in damageRelations.HalfDamageFrom)
                {
                    var typeFrom = x.Name.ToEnum<Type>();
                    DefensiveEff[defType][typeFrom] = 0.5;
                }

                foreach (var x in damageRelations.NoDamageFrom)
                {
                    var typeFrom = x.Name.ToEnum<Type>();
                    DefensiveEff[defType][typeFrom] = 0;
                }

                Console.WriteLine($@"Set defensive {typeName} effectiveness data.");
            });
            await Task.WhenAll(tasks);
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
            if (!DefensiveEff.ContainsKey(offType))
            {
                return 1;
            }

            var offDict = DefensiveEff[offType];
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