namespace PokePlanner.Util
{
    /// <summary>
    /// Class for managing the selected team of Pokemon.
    /// </summary>
    public class TeamManager
    {
        /// <summary>
        /// Singleton constructor.
        /// </summary>
        private TeamManager() { }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static TeamManager Instance { get; set; } = new TeamManager();
    }
}