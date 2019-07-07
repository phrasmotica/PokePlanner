using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PokeAPI;
using PokePlanner.Util;

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
            MinWidth = 185;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                LoadVersionGroupData();
            }
        }

        /// <summary>
        /// Loads version groups into the combo box.
        /// </summary>
        public async void LoadVersionGroupData()
        {
            ItemsSource = new List<string>();
            var itemList = (List<string>) ItemsSource;

            var groups = await GetVersionGroups();
            foreach (var group in groups)
            {
                var name = await group.GetName();
                itemList.Add(name);
            }

            SelectedIndex = 0;
        }

        /// <summary>
        /// Returns all version groups from PokeAPI.
        /// </summary>
        public static async Task<IEnumerable<VersionGroup>> GetVersionGroups()
        {
            var vgResources = await DataFetcher.GetResourceList<NamedApiResource<VersionGroup>, VersionGroup>();
            var tasks = vgResources.Select(async vg => await vg.GetObject());
            return await Task.WhenAll(tasks);
        }
    }
}