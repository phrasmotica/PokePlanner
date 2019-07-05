using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using PokeAPI;

namespace PokePlanner.Controls
{
    /// <summary>
    /// Combo box for displaying version groups.
    /// </summary>
    public class VersionComboBox : ComboBox
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public VersionComboBox()
        {
            LoadVersionGroupData();
        }

        /// <summary>
        /// Loads version groups into the combo box.
        /// </summary>
        public async void LoadVersionGroupData()
        {
            var groups = await GetVersionGroups();
            ItemsSource = groups.Select(g => g.Name);
        }

        /// <summary>
        /// Returns all version groups from PokeAPI.
        /// </summary>
        public static async Task<ResourceList<NamedApiResource<VersionGroup>, VersionGroup>> GetVersionGroups()
        {
            Console.WriteLine(@"Getting version groups...");
            var versionGroups = await DataFetcher.GetResourceList<NamedApiResource<VersionGroup>, VersionGroup>();
            Console.WriteLine($@"Got {versionGroups.Count} version groups.");

            var groupList = versionGroups.ToList();
            Console.WriteLine($@"Got {groupList.Count} version groups:");
            for (var i = 0; i < groupList.Count; i++)
            {
                var group = groupList[i];
                Console.WriteLine(i + " " + group.Name);
            }

            return versionGroups;
        }
    }
}