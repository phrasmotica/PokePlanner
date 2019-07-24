using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PokeAPI;
using PokePlanner.Properties;
using PokePlanner.Util;

namespace PokePlanner.Controls
{
    /// <summary>
    /// Combo box for displaying version groups.
    /// </summary>
    public class VersionComboBox : ComboBox
    {
        /// <summary>
        /// Map of version group names to generation names.
        /// </summary>
        private IDictionary<string, string> generationMap;

        /// <summary>
        /// The settings file.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// The main window.
        /// </summary>
        private readonly MainWindow mainWindow;

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
                mainWindow = (MainWindow) Application.Current.MainWindow;

                LoadVersionGroupData();
                settings = Settings.Default;
                SelectionChanged += OnSelectionChanged;
            }
        }

        /// <summary>
        /// Returns all version group names.
        /// </summary>
        private IList<string> VersionGroups => generationMap.Keys.ToList();

        /// <summary>
        /// Loads version groups into the combo box.
        /// </summary>
        public async void LoadVersionGroupData()
        {
            ItemsSource = new List<string>();
            var itemList = (List<string>) ItemsSource;
            generationMap = new Dictionary<string, string>();

            var groups = await GetVersionGroups();
            foreach (var group in groups)
            {
                generationMap[group.Name] = group.Generation.Name;
                var name = await group.GetName();
                itemList.Add(name);
            }

            var savedVersionGroup = settings.versionGroup;
            SelectedIndex = string.IsNullOrEmpty(savedVersionGroup) ? 0 : VersionGroups.IndexOf(savedVersionGroup);
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

        /// <summary>
        /// Save version group in settings file.
        /// </summary>
        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vgName = VersionGroups[SelectedIndex];
            settings.versionGroup = vgName;

            var versionGroup = await DataFetcher.GetNamedApiObject<VersionGroup>(vgName);
            SessionCache.Instance.VersionGroup = versionGroup;
            SessionCache.Instance.Generation = await versionGroup.Generation.GetObject();
            
            mainWindow.UpdateTypes();
            mainWindow.UpdateHMs();
        }
    }
}