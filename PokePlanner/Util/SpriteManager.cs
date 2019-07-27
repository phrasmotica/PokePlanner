using System;
using System.IO;

namespace PokePlanner.Util
{
    /// <summary>
    /// Singleton for managing Pokemon sprites.
    /// </summary>
    public class SpriteManager
    {
        /// <summary>
        /// Local path for saving sprites.
        /// </summary>
        public string SpriteDir;

        /// <summary>
        /// Singleton constructor.
        /// </summary>
        private SpriteManager()
        {
            var appDataPath = Environment.GetEnvironmentVariable("LocalAppData") + @"\PokePlanner";
            SpriteDir = appDataPath + @"\sprites";
            Directory.CreateDirectory(SpriteDir);
        }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static SpriteManager Instance { get; set; } = new SpriteManager();
    }
}