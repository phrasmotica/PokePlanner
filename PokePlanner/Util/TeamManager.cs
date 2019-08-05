using System.Threading.Tasks;
using PokePlanner.Properties;

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

        /// <summary>
        /// List of all team members.
        /// </summary>
        public TeamMember[] Team;

        /// <summary>
        /// Update types in the team display and effectiveness chart.
        /// </summary>
        public async Task<bool> Validate(int slot, string newVersionGroup)
        {
            if (Settings.Default.restrictToVersion)
            {
                return true;
            }

            await Team[slot].Validate();
            return Team[slot].IsValid;
        }
    }
}