﻿using PokeAPI;

namespace PokePlanner.Util
{
    /// <summary>
    /// Singleton for caching data during the application session.
    /// </summary>
    public class SessionCache
    {
        /// <summary>
        /// The selected version group.
        /// </summary>
        public VersionGroup VersionGroup;

        /// <summary>
        /// The selected version group's generation.
        /// </summary>
        public Generation Generation;

        /// <summary>
        /// Singleton constructor;
        /// </summary>
        private SessionCache() { }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static SessionCache Instance { get; set; } = new SessionCache();
    }
}
