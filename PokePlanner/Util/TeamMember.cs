using System.Threading.Tasks;
using PokeApiNet.Models;
using PokePlanner.Properties;

namespace PokePlanner.Util
{
    /// <summary>
    /// Class representing a Pokemon on a team.
    /// </summary>
    public class TeamMember
    {
        /// <summary>
        /// Whether the Pokemon is valid on the team.
        /// </summary>
        public bool IsValid;

        /// <summary>
        /// The slot that this team member occupies.
        /// </summary>
        public int Slot;

        /// <summary>
        /// Gets or sets the Pokemon.
        /// </summary>
        public Pokemon Pokemon
        {
            get => IsValid ? pokemon : null;
            set { pokemon = value; }
        }
        private Pokemon pokemon;

        /// <summary>
        /// Returns this team member's name.
        /// </summary>
        public async Task<string> GetName()
        {
            return IsValid ? await Pokemon.GetName() : string.Empty;
        }

        /// <summary>
        /// Validates this Pokemon.
        /// </summary>
        public async Task Validate()
        {
            IsValid = await Pokemon.IsValid(SessionCache.Instance.VersionGroup);
        }
    }
}