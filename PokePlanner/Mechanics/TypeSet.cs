using System;
using System.Collections.Generic;
using System.Linq;

namespace PokePlanner.Mechanics
{
    /// <summary>
    /// Represents a set of types used by a game.
    /// </summary>
    public class TypeSet : Dictionary<Type, ActiveInfo>
    {
        /// <summary>
        /// Returns all active types.
        /// </summary>
        public Type[] ActiveTypes => Keys.Where(GetActive).ToArray();

        /// <summary>
        /// Returns whether the given type is enabled.
        /// </summary>
        public bool GetActive(Type type) => this[type].IsActive;

        /// <summary>
        /// Enables/disables the given type.
        /// </summary>
        public void SetActive(Type type, bool active)
        {
            var t = this[type];
            t.IsActive = active;
            var desc = active ? "active" : "inactive";
            Console.WriteLine($@"{type} type is now {desc}.");
        }
    }

    /// <summary>
    /// Represents the activity of a type in a type set.
    /// </summary>
    public struct ActiveInfo
    {
        public ActiveInfo(bool isActive, int column)
        {
            IsActive = isActive;
            Column = column;
        }

        public bool IsActive { get; set; }
        public int Column { get; set; }
    }
}