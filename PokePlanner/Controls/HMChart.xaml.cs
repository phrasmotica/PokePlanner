using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using PokePlanner.Util;

namespace PokePlanner.Controls
{
    /// <summary>
    /// Interaction logic for HMChart.xaml.
    /// </summary>
    public partial class HMChart
    {
        /// <summary>
        /// The main window.
        /// </summary>
        private readonly MainWindow mainWindow;

        /// <summary>
        /// Matrix indicating whether the pokemon in each slot can learn each HM.
        /// FIrst index (col) for a Pokemon, second index (row) for an HM.
        /// </summary>
        public bool[][] CanLearnMatrix;

        /// <summary>
        /// Create a label for each HM move.
        /// </summary>
        public HMChart()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                for (int i = 0; i < AllLabels.Count; i++)
                {
                    var label = AllLabels[i];
                    label.Prefix = $@"HM{i + 1:D2}: ";
                }

                CanLearnMatrix = new bool[Constants.TEAM_SIZE][];
                for (int i = 0; i < CanLearnMatrix.Length; i++)
                {
                    CanLearnMatrix[i] = new bool[Constants.NUMBER_OF_HMS];
                }

                mainWindow = (MainWindow) Application.Current.MainWindow;
            }
        }

        /// <summary>
        /// Returns all HM labels.
        /// </summary>
        private IList<HMLabel> AllLabels => new List<HMLabel>
        {
            hmLabel1, hmLabel2, hmLabel3, hmLabel4, hmLabel5, hmLabel6, hmLabel7, hmLabel8
        };

        /// <summary>
        /// Fetch all HM moves for the version group and display them.
        /// </summary>
        public async void UpdateHMs()
        {
            var versionGroup = SessionCache.Instance.VersionGroup;
            var vgName = await versionGroup.GetName();
            Console.WriteLine($@"Retrieving HM moves for {vgName}...");

            SessionCache.Instance.HMMoves = await versionGroup.GetHMMoves();
            var hmMoves = SessionCache.Instance.HMMoves;

            var hmCount = hmMoves.Count;
            for (var i = 0; i < AllLabels.Count; i++)
            {
                var label = AllLabels[i];
                if (i < hmCount)
                {
                    label.SetContent(hmMoves[i].GetName());
                    label.Visibility = Visibility.Visible;
                }
                else
                {
                    label.SetContent(string.Empty);
                    label.Visibility = Visibility.Hidden;
                }
            }

            Console.WriteLine($@"Retrieved {hmCount} HM moves for {vgName}.");

            // set columns of learn matrix
            var team = mainWindow.AllDisplays.Select(d => d.Pokemon).ToArray();
            var moveNames = hmMoves.Select(m => m.Name).ToArray();
            for (var col = 0; col < team.Length; col++)
            {
                var canLearn = team[col].CanLearn(moveNames);
                SetCanLearn(col, canLearn, false);
            }

            // update coverage once, after every matrix column has been updated
            UpdateHMCoverage();
        }

        /// <summary>
        /// Returns whether the team can learn the given HM.
        /// </summary>
        public bool GetCanLearn(int hmRowIndex)
        {
            return CanLearnMatrix.Any(pokemonCol => pokemonCol[hmRowIndex]);
        }

        /// <summary>
        /// Sets the given column in the learn matrix and updates the HM labels accordingly.
        /// </summary>
        public void SetCanLearn(int pokemonColIndex, bool[] canLearn, bool updateChart = true)
        {
            CanLearnMatrix[pokemonColIndex] = canLearn;

            if (updateChart)
            {
                UpdateHMCoverage();
            }
        }

        /// <summary>
        /// Update chart according to the HMs that can be learnt by the team.
        /// </summary>
        private void UpdateHMCoverage()
        {
            Console.WriteLine(@"Updating HM move coverage...");

            // activate/deactivate labels accordingly
            for (var hmRowIndex = 0; hmRowIndex < AllLabels.Count; hmRowIndex++)
            {
                if (GetCanLearn(hmRowIndex))
                {
                    AllLabels[hmRowIndex].Activate();
                }
                else
                {
                    AllLabels[hmRowIndex].Deactivate();
                }
            }

            Console.WriteLine(@"Updated HM move coverage.");
        }
    }
}