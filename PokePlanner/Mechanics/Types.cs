using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
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
                var typeObj = await SessionCache.Get<PokeApiNet.Models.Type>(typeName);
                Console.WriteLine($@"Got {typeName} type data.");
            
                // now set its defensive effectivenesses
                Console.WriteLine($@"Setting defensive {typeName} effectiveness data...");

                // initialise dictionary
                DefensiveEff[defType] = new Dictionary<Type, double>();
                DefensiveEff[defType].Initialise(ConcreteTypes, 1);

                // populate defensive damage relations
                var damageRelations = await typeObj.GetDamageRelations();

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
            TypeColours[Type.Unknown] = TypeBrushes.Unknown;
            TypeColours[Type.Normal] = TypeBrushes.Normal;
            TypeColours[Type.Fighting] = TypeBrushes.Fighting;
            TypeColours[Type.Flying] = TypeBrushes.Flying;
            TypeColours[Type.Poison] = TypeBrushes.Poison;
            TypeColours[Type.Ground] = TypeBrushes.Ground;
            TypeColours[Type.Rock] = TypeBrushes.Rock;
            TypeColours[Type.Bug] = TypeBrushes.Bug;
            TypeColours[Type.Ghost] = TypeBrushes.Ghost;
            TypeColours[Type.Steel] = TypeBrushes.Steel;
            TypeColours[Type.Fire] = TypeBrushes.Fire;
            TypeColours[Type.Water] = TypeBrushes.Water;
            TypeColours[Type.Grass] = TypeBrushes.Grass;
            TypeColours[Type.Electric] = TypeBrushes.Electric;
            TypeColours[Type.Psychic] = TypeBrushes.Psychic;
            TypeColours[Type.Ice] = TypeBrushes.Ice;
            TypeColours[Type.Dragon] = TypeBrushes.Dragon;
            TypeColours[Type.Dark] = TypeBrushes.Dark;
            TypeColours[Type.Fairy] = TypeBrushes.Fairy;
            TypeColours[Type.Shadow] = TypeBrushes.Shadow;
        }

        /// <summary>
        /// Returns the effectiveness dictionary for the given defensive type.
        /// </summary>
        public IDictionary<Type, double> GetEffDict(Type defType)
        {
            return DefensiveEff[defType];
        }

        /// <summary>
        /// Returns the effectiveness dictionary for the given defensive types.
        /// </summary>
        public IDictionary<Type, double> GetEffDict(Type defType1, Type defType2)
        {
            return GetEffDict(defType1).Product(GetEffDict(defType2));
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