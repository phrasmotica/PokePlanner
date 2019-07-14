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
        /// List of API object names for each item.
        /// </summary>
        private List<string> apiNames;

        /// <summary>
        /// The settings file.
        /// </summary>
        private readonly Settings settings;

#if DEBUG
        /// <summary>
        /// The main window.
        /// </summary>
        private readonly MainWindow mainWindow;
#endif

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
#if DEBUG
                mainWindow = (MainWindow) Application.Current.MainWindow;
#endif

                LoadVersionGroupData();
                settings = Settings.Default;
                SelectionChanged += OnSelectionChanged;
            }
        }

        /// <summary>
        /// Loads version groups into the combo box.
        /// </summary>
        public async void LoadVersionGroupData()
        {
            ItemsSource = new List<string>();
            var itemList = (List<string>) ItemsSource;
            apiNames = new List<string>();

            var groups = await GetVersionGroups();
            foreach (var group in groups)
            {
                apiNames.Add(group.Name);
                var name = await group.GetName();
                itemList.Add(name);
            }

            var savedVersionGroup = settings.versionGroup;
            SelectedIndex = string.IsNullOrEmpty(savedVersionGroup) ? 0 : apiNames.IndexOf(savedVersionGroup);
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
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var apiName = apiNames[SelectedIndex];
            settings.versionGroup = apiName;

#if DEBUG
            mainWindow.UpdateTypes();
#endif
        }
    }
}