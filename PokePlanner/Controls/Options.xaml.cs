using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PokePlanner.Properties;
using PokePlanner.Util;

namespace PokePlanner.Controls
{
    /// <summary>
    /// Interaction logic for Options.xaml.
    /// </summary>
    public partial class Options
    {
        /// <summary>
        /// The main window.
        /// </summary>
        private readonly MainWindow mainWindow;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Options()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                mainWindow = (MainWindow) Application.Current.MainWindow;
                restrictCheckBox.IsChecked = Settings.Default.restrictToVersion;
            }
        }

        /// <summary>
        /// Updates the team's types and their rows in the type chart.
        /// </summary>
        private async Task UpdateTeam()
        {
            if (mainWindow != null)
            {
                await mainWindow.ValidateTeam(Settings.Default.versionGroup);

                // update HM coverage
                var hmMoves = SessionCache.Instance.HMMoves;
                if (hmMoves != null)
                {
                    var moveNames = hmMoves.Select(m => m.Name).ToArray();
                    var team = mainWindow.Team;
                    for (var row = 0; row < team.Length; row++)
                    {
                        var canLearn = team[row].CanLearn(moveNames);
                        hmChart.SetCanLearn(row, canLearn);
                    }

                    hmChart.UpdateHMCoverage(hmMoves.Count);
                }
            }
        }

        private async void RestrictCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox) sender;
            Settings.Default.restrictToVersion = checkBox.IsChecked ?? false;
            await UpdateTeam();
        }
    }
}